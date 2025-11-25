using System.Linq.Expressions;

namespace Domain;

public static class ExpressionComposer
{
    public static Expression<Func<T, bool>> And<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = Expression.Parameter(typeof(T), "x");

        var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], param);
        var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], param);

        var body = Expression.AndAlso(
            leftVisitor.Visit(left.Body)!,
            rightVisitor.Visit(right.Body)!
        );

        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    public static Expression<Func<T, bool>> Or<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = Expression.Parameter(typeof(T), "x");

        var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], param);
        var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], param);

        var body = Expression.OrElse(
            leftVisitor.Visit(left.Body)!,
            rightVisitor.Visit(right.Body)!
        );

        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    public static Expression<Func<T, bool>> Not<T>(
        Expression<Func<T, bool>> expression)
    {
        var param = expression.Parameters[0];
        var body = Expression.Not(expression.Body);

        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}

internal class ReplaceExpressionVisitor : ExpressionVisitor
{
    private readonly Expression _oldValue;
    private readonly Expression _newValue;

    public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override Expression? Visit(Expression? node)
    {
        return node == _oldValue ? _newValue : base.Visit(node);
    }
}
