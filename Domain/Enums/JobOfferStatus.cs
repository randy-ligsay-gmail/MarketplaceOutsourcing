namespace MarketplaceOutsourcing.Domain.Enums;

public enum JobOfferStatus
{
    Pending,    // submitted, awaiting the customer's decision
    Accepted,   // chosen by the customer
    Rejected,   // a different offer was accepted instead
    Withdrawn   // the contractor pulled their own offer back
}