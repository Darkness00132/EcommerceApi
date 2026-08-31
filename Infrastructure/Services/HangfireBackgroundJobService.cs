using System.Linq.Expressions;
using Application.Abstractions.Services;
using Application.Constants;
using Hangfire;

namespace Infrastructure.Services;

public class HangfireBackgroundJobService : IBackgroundJobService
{
    private readonly IBackgroundJobClient _jobClient;

    public HangfireBackgroundJobService(IBackgroundJobClient jobClient)
    {
        _jobClient = jobClient;
    }

    public void Enqueue<TService>(
        Expression<Func<TService, Task>> methodCall,
        string priority = BackgroundJobQueuesPriority.Default)
    {
        _jobClient.Enqueue<TService>(priority, methodCall);
    }
}
