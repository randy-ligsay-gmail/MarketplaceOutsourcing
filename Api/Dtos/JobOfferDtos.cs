namespace MarketplaceOutsourcing.Api.Dtos;

public record JobOfferResponse(
    Guid Id,
    Guid JobId,
    Guid ContractorId,
    decimal Price,
    string Status);

public record CreateJobOfferRequest(
    Guid JobId,
    decimal Price);

public record UpdateJobOfferRequest(
    decimal Price);

public record AcceptOfferResponse(
    Guid JobId,
    Guid AcceptedOfferId,
    Guid AcceptedBy,
    string JobStatus);
