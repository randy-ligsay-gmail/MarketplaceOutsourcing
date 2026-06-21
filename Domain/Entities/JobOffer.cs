using MarketplaceOutsourcing.Domain.Common;
using MarketplaceOutsourcing.Domain.Enums;
using MarketplaceOutsourcing.Domain.Exceptions;

namespace MarketplaceOutsourcing.Domain.Entities;

public class JobOffer : BaseEntity
{
    public Guid JobId { get; private set; }
    public Guid ContractorId { get; private set; }
    public decimal Price { get; private set; }
    public JobOfferStatus Status { get; private set; }

    private JobOffer() { }

    public JobOffer(Guid jobId, Guid contractorId, decimal price)
    {
        if (price <= 0)
        {
            throw new DomainException("Offer price must be greater than zero.");
        }

        JobId = jobId;
        ContractorId = contractorId;
        Price = price;
        Status = JobOfferStatus.Pending;
    }

    public void Accept()
    {
        if (Status != JobOfferStatus.Pending)
        {
            throw new DomainException("Only pending offers can be accepted.");
        }

        Status = JobOfferStatus.Accepted;
    }

    public void Reject()
    {
        if (Status == JobOfferStatus.Accepted)
        {
            throw new DomainException("Accepted offers cannot be rejected.");
        }

        Status = JobOfferStatus.Rejected;
    }

    public void Withdraw()
    {
        if (Status != JobOfferStatus.Pending)
        {
            throw new DomainException("Only pending offers can be withdrawn.");
        }

        Status = JobOfferStatus.Withdrawn;
    }

    public void UpdatePrice(decimal price)
    {
        if (Status != JobOfferStatus.Pending)
        {
            throw new DomainException("Only pending offers can be updated.");
        }

        if (price <= 0)
        {
            throw new DomainException("Offer price must be greater than zero.");
        }

        Price = price;
    }

    public string GetDetail() => $"{Id} | job {JobId} | {CurrencyFormatter.Format(Price)} | {Status}";
}
