using System.Linq.Expressions;

namespace Domain.Specifications;

public class DirectSpecification<T> : BaseSpecification<T>
{
    public DirectSpecification(Expression<Func<T, bool>> criteria)
        : base(criteria)
    {
    }
}
