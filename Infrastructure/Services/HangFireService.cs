using Application.Interfaces.Services;
using Domain.Enums;
using Hangfire;
using System.Linq.Expressions;

namespace Infrastructure.Services
{
    public class HangFireService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        public HangFireService(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }
        public string Enqueue(Expression<Func<Task>> methodCall, BackgroundJopPriority priority)
        {
            return _backgroundJobClient.Enqueue(priority.ToString().ToLowerInvariant(),methodCall);
        }
    }
}
