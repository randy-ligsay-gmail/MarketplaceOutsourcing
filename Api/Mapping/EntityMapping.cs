using MarketplaceOutsourcing.Api.Dtos;
using MarketplaceOutsourcing.Domain.Entities;

namespace MarketplaceOutsourcing.Api.Mapping;

public static class EntityMapping
{
    public static CustomerResponse ToResponse(this Customer customer) =>
        new(customer.Id, customer.FirstName, customer.LastName);

    public static ContractorResponse ToResponse(this Contractor contractor) =>
        new(contractor.Id, contractor.Name, contractor.Rating);

    public static JobResponse ToResponse(this Job job) =>
        new(
            job.Id,
            job.CustomerId,
            job.Title,
            job.Description,
            job.StartDate,
            job.DueDate,
            job.Budget,
            job.Status.ToString(),
            job.AcceptedContractorId);

    public static JobOfferResponse ToResponse(this JobOffer offer) =>
        new(
            offer.Id,
            offer.JobId,
            offer.ContractorId,
            offer.Price,
            offer.Status.ToString());
}
