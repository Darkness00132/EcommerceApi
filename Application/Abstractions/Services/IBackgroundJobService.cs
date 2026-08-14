using System.Linq.Expressions;

namespace Application.Abstractions.Services;

public interface IBackgroundJobService
{
    void Enqueue<TService>(Expression<Func<TService, Task>> methodCall);
}
