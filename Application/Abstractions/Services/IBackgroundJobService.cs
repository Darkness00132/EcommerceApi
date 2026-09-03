using System.Linq.Expressions;
using Application.Constants;

namespace Application.Abstractions.Services;

public interface IBackgroundJobService
{
    void Enqueue<TService>(Expression<Func<TService, Task>> methodCall
        , string priority = BackgroundJobQueuesPriority.Default);
}
