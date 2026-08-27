using System.Linq.Expressions;
using Application.Abstractions.Services;
using Hangfire;

namespace Infrastructure.Services;

internal class HangfireBackgroundJobService(
    IBackgroundJobClient backgroundJobClient)
    : IBackgroundJobService
{
    public void Enqueue<TService>(
        Expression<Func<TService, Task>> methodCall)
    {
        backgroundJobClient.Enqueue(methodCall);
    }
}
