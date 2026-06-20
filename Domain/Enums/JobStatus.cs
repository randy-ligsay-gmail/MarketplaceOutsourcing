namespace MarketplaceOutsourcing.Domain.Enums;

public enum JobStatus
{
    Open,       // just created, open to offers
    Accepted,   // customer accepted one offer, work is assigned
    Completed,  // contractor finished and it's been marked done
    Cancelled   // customer withdrew the job before any offer was accepted
}