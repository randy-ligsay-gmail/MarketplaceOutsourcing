using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Application.Services;
using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Infrastructure.Caching;
using MarketplaceOutsourcing.Tests.Helpers;
using Moq;

namespace MarketplaceOutsourcing.Tests.Application;

public class JobServiceTests
{
    private readonly Mock<IJobRepository> _jobRepository = new();
    private readonly LruCache _cache = TestFixtures.CreateCache();

    private JobService CreateService() => new(_jobRepository.Object, _cache);

    [Fact]
    public void CreateJob_WithValidInput_PersistsAndReturnsJob()
    {
        var service = CreateService();
        var customerId = Guid.NewGuid();
        var start = DateTime.UtcNow.Date;
        var due = start.AddDays(10);

        var (success, job, errorMessage) = service.CreateJob(
            "Data analysis",
            "EDA and ML pipeline",
            customerId,
            start,
            due,
            9200m);

        Assert.True(success);
        Assert.Null(errorMessage);
        Assert.NotNull(job);
        Assert.Equal(customerId, job!.CustomerId);
        _jobRepository.Verify(r => r.Add(It.IsAny<Job>()), Times.Once);
    }

    [Fact]
    public void CreateJob_WithInvalidBudget_ReturnsError()
    {
        var service = CreateService();

        var (success, job, errorMessage) = service.CreateJob(
            "Title",
            "Description",
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(1),
            0m);

        Assert.False(success);
        Assert.Null(job);
        Assert.Equal("Budget must be greater than zero.", errorMessage);
        _jobRepository.Verify(r => r.Add(It.IsAny<Job>()), Times.Never);
    }

    [Fact]
    public void ListJobs_SecondCall_UsesCachedResultWithoutSecondRepositoryRead()
    {
        var jobs = new List<Job> { TestFixtures.CreateOpenJob() };
        _jobRepository.Setup(r => r.GetAll()).Returns(jobs);

        var service = CreateService();
        var first = service.ListJobs();
        var second = service.ListJobs();

        Assert.Same(first, second);
        _jobRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void DeleteJob_WhenJobNotFound_ReturnsError()
    {
        var jobId = Guid.NewGuid();
        _jobRepository.Setup(r => r.GetById(jobId)).Returns((Job?)null);

        var service = CreateService();
        var (success, errorMessage) = service.DeleteJob(jobId);

        Assert.False(success);
        Assert.Equal("Job not found.", errorMessage);
    }

    [Fact]
    public void SearchOpenJobs_WithBlankTerm_ReturnsEmptyWithoutQueryingRepository()
    {
        var service = CreateService();

        var results = service.SearchOpenJobs("   ");

        Assert.Empty(results);
        _jobRepository.Verify(r => r.SearchOpenJobs(It.IsAny<string>()), Times.Never);
    }
}
