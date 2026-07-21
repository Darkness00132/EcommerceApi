using Domain.Enums;
using System.Linq.Expressions;

namespace Application.Interfaces.Services
{
    public interface IBackgroundJobService
    {
        string Enqueue(Expression<Func<Task>> methodCall, BackgroundJopPriority priorty);
    }
}
