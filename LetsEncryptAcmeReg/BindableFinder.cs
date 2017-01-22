using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace LetsEncryptAcmeReg
{
    public class BindableFinder<T> : ExpressionVisitor
    {
        [CanBeNull]
        private readonly Bindable<T> bindable;
        private readonly LambdaExpression exprLambda;
        private readonly bool init;
        public Action InitAction { get; private set; }

        public BindableFinder([CanBeNull] Bindable<T> bindable, Expression<Func<T>> exprLamba, bool init)
        {
            this.bindable = bindable;
            this.exprLambda = exprLamba;
            this.init = init;
        }

        public BindableFinder(Expression<Action> exprLamba, bool init)
        {
            this.bindable = null;
            this.exprLambda = exprLamba;
            this.init = init;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var member = typeof(Bindable<T>).GetMember(nameof(Bindable<T>.Value))[0];
            if (node.Member.MetadataToken == member.MetadataToken)
            {
                if (this.IsConstant(node.Expression))
                {
                    var decl = node.Member.DeclaringType;
                    var generic = decl.GetGenericArguments()[0];
                    var setterType = typeof(Action<>).MakeGenericType(generic);
                    var method = decl.GetMethod(nameof(this.bindable.Bind), new[] { setterType, typeof(bool) });
                    var par = Expression.Parameter(generic);

                    var lambda = Expression.Lambda(
                        setterType,
                        this.bindable == null
                            ? exprLambda.Body
                            : Expression.Assign(Expression.MakeMemberAccess(Expression.Constant(this.bindable), member), exprLambda.Body),
                        par);

                    var call = Expression.Call(node.Expression, method, lambda, Expression.Constant(this.init));
                    var fn = Expression.Lambda<Func<Action>>(call).Compile();
                    this.InitAction += fn();
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