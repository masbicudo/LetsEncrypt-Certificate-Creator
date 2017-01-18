using System;
using System.Linq.Expressions;

namespace LetsEncryptAcmeReg
{
    public class BindableFinder<T> : ExpressionVisitor
    {
        private readonly Bindable<T> bindable;
        private readonly Expression exprValue;

        public BindableFinder(Bindable<T> bindable, LambdaExpression exprValue)
        {
            this.bindable = bindable;
            this.exprValue = exprValue.Body;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var member = typeof(Bindable<T>).GetMember(nameof(Bindable<T>.Value))[0];
            if (node.Member.MetadataToken == member.MetadataToken)
            {
                if (this.IsConstant(node.Expression))
                {
                    var evt = node.Member.DeclaringType.GetEvent(nameof(Bindable<T>.Changed));
                    var assign = Expression.Assign(Expression.MakeMemberAccess(Expression.Constant(this.bindable), member), exprValue);
                    var par = Expression.Lambda(evt.EventHandlerType, assign, Expression.Parameter(evt.EventHandlerType.GetGenericArguments()[0]));
                    var call = Expression.Call(node.Expression, evt.AddMethod, par);
                    var fn = Expression.Lambda<Action>(call).Compile();
                    fn();
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