using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Domain.Enums;
using MarketplaceOutsourcing.Domain.Exceptions;

namespace MarketplaceOutsourcing.Tests.Domain;

public class JobOfferTests
{
    [Fact]
    public void NewOffer_IsPendingWithPrice()
    {
        var jobId = Guid.NewGuid();
        var contractorId = Guid.NewGuid();

        var offer = new JobOffer(jobId, contractorId, 950m);

        Assert.Equal(JobOfferStatus.Pending, offer.Status);
        Assert.Equal(950m, offer.Price);
        Assert.Equal(jobId, offer.JobId);
        Assert.Equal(contractorId, offer.ContractorId);
    }

    [Fact]
    public void Constructor_WithNonPositivePrice_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            new JobOffer(Guid.NewGuid(), Guid.NewGuid(), 0m));

        Assert.Equal("Offer price must be greater than zero.", ex.Message);
    }

    [Fact]
    public void Withdraw_WhenPending_SetsStatusToWithdrawn()
    {
        var offer = new JobOffer(Guid.NewGuid(), Guid.NewGuid(), 500m);

        offer.Withdraw();

        Assert.Equal(JobOfferStatus.Withdrawn, offer.Status);
    }

    [Fact]
    public void Accept_WhenNotPending_ThrowsDomainException()
    {
        var offer = new JobOffer(Guid.NewGuid(), Guid.NewGuid(), 500m);
        offer.Withdraw();

        var ex = Assert.Throws<DomainException>(() => offer.Accept());

        Assert.Equal("Only pending offers can be accepted.", ex.Message);
    }

    [Fact]
    public void UpdatePrice_WhenPending_UpdatesPrice()
    {
        var offer = new JobOffer(Guid.NewGuid(), Guid.NewGuid(), 500m);

        offer.UpdatePrice(450m);

        Assert.Equal(450m, offer.Price);
    }
}
