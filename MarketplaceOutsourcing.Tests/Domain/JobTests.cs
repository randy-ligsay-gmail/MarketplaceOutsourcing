using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Domain.Enums;
using MarketplaceOutsourcing.Domain.Exceptions;
using MarketplaceOutsourcing.Tests.Helpers;

namespace MarketplaceOutsourcing.Tests.Domain;

public class JobTests
{
    [Fact]
    public void NewJob_IsOpenWithProvidedDetails()
    {
        var customerId = Guid.NewGuid();
        var start = DateTime.UtcNow.Date;
        var due = start.AddDays(7);

        var job = new Job("Fix roof", "Seal flashing", customerId, start, due, 1200m);

        Assert.Equal(JobStatus.Open, job.Status);
        Assert.Equal(customerId, job.CustomerId);
        Assert.Equal("Fix roof", job.Title);
        Assert.Equal(1200m, job.Budget);
        Assert.Null(job.AcceptedContractorId);
    }

    [Fact]
    public void UpdateDetails_WhenJobIsNotOpen_ThrowsDomainException()
    {
        var job = TestFixtures.CreateOpenJob();
        job.Cancel();

        var ex = Assert.Throws<DomainException>(() =>
            job.UpdateDetails("New title", "New description", job.StartDate, job.DueDate, 1000m));

        Assert.Equal("Only open jobs can be updated.", ex.Message);
    }

    [Fact]
    public void Cancel_WhenJobIsOpen_SetsStatusToCancelled()
    {
        var job = TestFixtures.CreateOpenJob();

        job.Cancel();

        Assert.Equal(JobStatus.Cancelled, job.Status);
    }

    [Fact]
    public void AcceptOffer_WhenJobIsOpen_SetsAcceptedContractor()
    {
        var job = TestFixtures.CreateOpenJob();
        var contractorId = Guid.NewGuid();

        job.AcceptOffer(contractorId);

        Assert.Equal(JobStatus.Accepted, job.Status);
        Assert.Equal(contractorId, job.AcceptedContractorId);
    }

    [Fact]
    public void AcceptOffer_WithEmptyContractorId_ThrowsDomainException()
    {
        var job = TestFixtures.CreateOpenJob();

        var ex = Assert.Throws<DomainException>(() => job.AcceptOffer(Guid.Empty));

        Assert.Equal("A valid contractor is required.", ex.Message);
    }
}
