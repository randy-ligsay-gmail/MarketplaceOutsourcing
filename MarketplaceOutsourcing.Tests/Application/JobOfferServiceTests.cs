using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Application.Services;
using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Domain.Enums;
using MarketplaceOutsourcing.Infrastructure.Caching;
using MarketplaceOutsourcing.Tests.Helpers;
using Moq;

namespace MarketplaceOutsourcing.Tests.Application;

public class JobOfferServiceTests
{
    private readonly Mock<IJobRepository> _jobRepository = new();
    private readonly Mock<IJobOfferRepository> _jobOfferRepository = new();
    private readonly Mock<IContractorRepository> _contractorRepository = new();
    private readonly LruCache _cache = TestFixtures.CreateCache();

    private JobOfferService CreateService() =>
        new(_jobRepository.Object, _jobOfferRepository.Object, _contractorRepository.Object, _cache);

    [Fact]
    public void SubmitOffer_ForOpenJob_CreatesPendingOffer()
    {
        var job = TestFixtures.CreateOpenJob();
        var contractor = TestFixtures.CreateContractor();

        _jobRepository.Setup(r => r.GetById(job.Id)).Returns(job);
        _contractorRepository.Setup(r => r.GetById(contractor.Id)).Returns(contractor);

        var service = CreateService();
        var (success, offer, errorMessage) = service.SubmitOffer(job.Id, contractor.Id, 4800m);

        Assert.True(success);
        Assert.Null(errorMessage);
        Assert.NotNull(offer);
        Assert.Equal(JobOfferStatus.Pending, offer!.Status);
        _jobOfferRepository.Verify(r => r.Add(It.IsAny<JobOffer>()), Times.Once);
    }

    [Fact]
    public void SubmitOffer_ForClosedJob_ReturnsError()
    {
        var job = TestFixtures.CreateOpenJob();
        job.Cancel();

        _jobRepository.Setup(r => r.GetById(job.Id)).Returns(job);

        var service = CreateService();
        var (success, offer, errorMessage) = service.SubmitOffer(job.Id, Guid.NewGuid(), 100m);

        Assert.False(success);
        Assert.Null(offer);
        Assert.Equal("Offers can only be submitted for open jobs.", errorMessage);
    }

    [Fact]
    public void AcceptOffer_RejectsOtherPendingOffers()
    {
        var job = TestFixtures.CreateOpenJob();
        var winningContractor = Guid.NewGuid();
        var acceptedOffer = new JobOffer(job.Id, winningContractor, 1000m);
        var otherOffer = new JobOffer(job.Id, Guid.NewGuid(), 1100m);

        _jobOfferRepository.Setup(r => r.GetById(acceptedOffer.Id)).Returns(acceptedOffer);
        _jobRepository.Setup(r => r.GetById(job.Id)).Returns(job);
        _jobOfferRepository.Setup(r => r.GetByJobId(job.Id))
            .Returns(new List<JobOffer> { acceptedOffer, otherOffer });

        var service = CreateService();
        var (success, updatedJob, errorMessage) = service.AcceptOffer(acceptedOffer.Id);

        Assert.True(success);
        Assert.Null(errorMessage);
        Assert.NotNull(updatedJob);
        Assert.Equal(JobStatus.Accepted, updatedJob!.Status);
        Assert.Equal(JobOfferStatus.Accepted, acceptedOffer.Status);
        Assert.Equal(JobOfferStatus.Rejected, otherOffer.Status);
        _jobRepository.Verify(r => r.SaveChanges(), Times.Once);
        _jobOfferRepository.Verify(r => r.SaveChanges(), Times.Once);
    }
}
