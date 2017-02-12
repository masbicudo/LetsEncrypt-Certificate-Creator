using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace LetsEncryptAcmeReg
{
    public class BindableFinder<T> : ExpressionVisitor
    {
        [CanBeNull]
        private readonly Bindable<T> bindable;

        private readonly Action<T> action;
        private Func<T> expr;
        private readonly LambdaExpression exprLambda;
        private readonly bool init;

        public BindResult BindResult { get; private set; }

        public List<Expression> BindablesFound { get; } = new List<Expression>();

        public BindableFinder(Action<T> action, Expression<Func<T>> exprLambda, bool init)
        {
            this.action = action;
            this.exprLambda = exprLambda;
            this.init = init;
        }

        public BindableFinder([CanBeNull] Bindable<T> bindable, Expression<Func<T>> exprLambda, bool init)
        {
            this.bindable = bindable;
            this.exprLambda = exprLambda;
            this.init = init;
        }

        public BindableFinder(Expression<Action> exprLambda, bool init)
        {
            this.bindable = null;
            this.exprLambda = exprLambda;
            this.init = init;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var member = typeof(Bindable<T>).GetMember(nameof(Bindable<T>.Value))[0];
            if (node.Member.MetadataToken == member.MetadataToken)
            {
                // Bindable<T2> refBindable = node.Expression
                var refBindable = node.Expression;
                var boundExpression = this.exprLambda.Body;

                if (this.IsConstant(refBindable))
                {
                    this.BindablesFound.Add(refBindable);

                    var declType = refBindable.Type; // typeof(Bindable<T2>)
                    var genericType = declType.GetGenericArguments()[0]; // typeof(T2)
                    var setterType = typeof(Action<>).MakeGenericType(genericType); // typeof(Action<T2>)

                    // building the lambda:
                    //  - if bindable is null: (Action<T2>)(x => boundExpression)
                    //  - if has action:       (Action<T2>)(x => this.action.Invoke(boundExpression))
                    //  - otherwise:           (Action<T2>)(x => this.bindable.Value = boundExpression)
                    var lambda = Expression.Lambda(
                        setterType,
                        this.bindable == null ? boundExpression :
                        this.action != null ? Expression.Call(Expression.Constant(this.action), "Invoke", new[] { genericType }, boundExpression) :
                        Expression.Assign(Expression.MakeMemberAccess(Expression.Constant(this.bindable), member), boundExpression) as Expression,
                        Expression.Parameter(genericType, "x"));

                    // looking for instance method:
                    //      Action Bind(Action<T2> setter, bool init)
                    var method = declType.GetMethod(nameof(this.bindable.Bind), new[] { setterType, typeof(bool) });

                    // calling the Bind method passing the lambda:
                    //      var fn = () => refBindable.Bind(x => ..., this.init);
                    //      this.InitAction += fn();
                    var call = Expression.Call(refBindable, method, lambda, Expression.Constant(this.init));
                    var fn = Expression.Lambda<Func<BindResult>>(call).Compile();
                    this.BindResult += fn();
                }
            }

            return base.VisitMember(node);
        }

        private bool IsConstant(Expression expr)
        {
            return expr is ConstantExpression || expr is MemberExpression && IsConstant(((MemberExpression)expr).Expression);
        }
    }
}