namespace MarketplaceOutsourcing.Api.Dtos;

public record ContractorResponse(
    Guid Id,
    string Name,
    decimal Rating);
