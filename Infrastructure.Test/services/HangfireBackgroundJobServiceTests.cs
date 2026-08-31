using System.Linq.Expressions;
using Application.Constants;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Infrastructure.Services;
using Moq;

namespace Infrastructure.Test.services;

public class HangfireBackgroundJobServiceTests
{
    private readonly Mock<IBackgroundJobClient> _jobClientMock;
    private readonly HangfireBackgroundJobService _sut;

    public HangfireBackgroundJobServiceTests()
    {
        _jobClientMock = new Mock<IBackgroundJobClient>();
        _sut = new HangfireBackgroundJobService(_jobClientMock.Object);
    }

    [Fact]
    public void Enqueue_ShouldCallJobClientCreate()
    {
        // Arrange
        Expression<Func<ITestDummyService, Task>> expression = s => s.DoWorkAsync();

        // Act
        _sut.Enqueue(expression, BackgroundJobQueuesPriority.Critical);

        // Assert
        _jobClientMock.Verify(
            client => client.Create(
                It.IsAny<Job>(),
                It.IsAny<IState>()),
            Times.Once);
    }

    [Fact]
    public void Enqueue_WhenPriorityNotPassed_ShouldUseDefaultQueue()
    {
        // Arrange
        Expression<Func<ITestDummyService, Task>> expression = s => s.DoWorkAsync();

        // Act
        _sut.Enqueue(expression);

        // Assert
        _jobClientMock.Verify(
            client => client.Create(
                It.IsAny<Job>(),
                It.Is<EnqueuedState>(state => state.Queue == BackgroundJobQueuesPriority.Default)),
            Times.Once);
    }

    public interface ITestDummyService
    {
        Task DoWorkAsync();
    }
}
