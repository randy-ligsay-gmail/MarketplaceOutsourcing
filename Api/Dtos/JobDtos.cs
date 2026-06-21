namespace MarketplaceOutsourcing.Api.Dtos;

public record JobResponse(
    Guid Id,
    Guid CustomerId,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime DueDate,
    decimal Budget,
    string Status,
    Guid? AcceptedBy);

public record CreateJobRequest(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime DueDate,
    decimal Budget);

public record UpdateJobRequest(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime DueDate,
    decimal Budget);
