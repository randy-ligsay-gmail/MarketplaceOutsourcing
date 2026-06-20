namespace MarketplaceOutsourcing.Application.Services;

using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Domain.Enums;
using MarketplaceOutsourcing.Domain.Exceptions;

public class JobOfferService
{
    private readonly IJobRepository _jobRepository;
    private readonly IJobOfferRepository _jobOfferRepository;
    private readonly IContractorRepository _contractorRepository;

    public JobOfferService(
        IJobRepository jobRepository,
        IJobOfferRepository jobOfferRepository,
        IContractorRepository contractorRepository)
    {
        _jobRepository = jobRepository;
        _jobOfferRepository = jobOfferRepository;
        _contractorRepository = contractorRepository;
    }

    public IReadOnlyList<JobOffer> ListOffers() => _jobOfferRepository.GetAll();

    public JobOffer? GetOffer(Guid id) => _jobOfferRepository.GetById(id);

    public IReadOnlyList<JobOffer> GetOffersForJob(Guid jobId) => _jobOfferRepository.GetByJobId(jobId);

    public (bool Success, JobOffer? Offer, string? ErrorMessage) CreateOffer(
        Guid jobId,
        Guid contractorId,
        decimal price) => SubmitOffer(jobId, contractorId, price);

    public (bool Success, JobOffer? Offer, string? ErrorMessage) SubmitOffer(
        Guid jobId,
        Guid contractorId,
        decimal price)
    {
        if (jobId == Guid.Empty || contractorId == Guid.Empty)
        {
            return (false, null, "Job ID and contractor ID are required.");
        }

        if (price <= 0)
        {
            return (false, null, "Price must be greater than zero.");
        }

        var job = _jobRepository.GetById(jobId);
        if (job is null)
        {
            return (false, null, "Job not found.");
        }

        if (job.Status != JobStatus.Open)
        {
            return (false, null, "Offers can only be submitted for open jobs.");
        }

        if (_contractorRepository.GetById(contractorId) is null)
        {
            return (false, null, "Contractor not found.");
        }

        try
        {
            var offer = new JobOffer(jobId, contractorId, price);
            _jobOfferRepository.Add(offer);
            return (true, offer, null);
        }
        catch (DomainException ex)
        {
            return (false, null, ex.Message);
        }
    }

    public (bool Success, JobOffer? Offer, string? ErrorMessage) UpdateOffer(Guid id, decimal price)
    {
        var offer = _jobOfferRepository.GetById(id);
        if (offer is null)
        {
            return (false, null, "Offer not found.");
        }

        try
        {
            offer.UpdatePrice(price);
            _jobOfferRepository.SaveChanges();
            return (true, offer, null);
        }
        catch (DomainException ex)
        {
            return (false, null, ex.Message);
        }
    }

    public (bool Success, string? ErrorMessage) DeleteOffer(Guid id)
    {
        var offer = _jobOfferRepository.GetById(id);
        if (offer is null)
        {
            return (false, "Offer not found.");
        }

        try
        {
            offer.Withdraw();
            _jobOfferRepository.SaveChanges();
            return (true, null);
        }
        catch (DomainException ex)
        {
            return (false, ex.Message);
        }
    }

    public (bool Success, Job? Job, string? ErrorMessage) AcceptOffer(Guid jobOfferId)
    {
        if (jobOfferId == Guid.Empty)
        {
            return (false, null, "Offer ID is required.");
        }

        var offer = _jobOfferRepository.GetById(jobOfferId);
        if (offer is null)
        {
            return (false, null, "Offer not found.");
        }

        var job = _jobRepository.GetById(offer.JobId);
        if (job is null)
        {
            return (false, null, "Job not found.");
        }

        try
        {
            job.AcceptOffer(offer.ContractorId);
            offer.Accept();

            foreach (var otherOffer in _jobOfferRepository.GetByJobId(job.Id))
            {
                if (otherOffer.Id == offer.Id)
                {
                    continue;
                }

                if (otherOffer.Status == JobOfferStatus.Pending)
                {
                    otherOffer.Reject();
                }
            }

            _jobRepository.SaveChanges();
            _jobOfferRepository.SaveChanges();

            return (true, job, null);
        }
        catch (DomainException ex)
        {
            return (false, null, ex.Message);
        }
    }
}
