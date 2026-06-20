namespace MarketplaceOutsourcing.Application.Services;

using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Domain.Enums;
using MarketplaceOutsourcing.Domain.Exceptions;

public class JobService
{
    private readonly IJobRepository _jobRepository;

    public JobService(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public IReadOnlyList<Job> ListJobs() => _jobRepository.GetAll();

    public Job? GetJob(Guid id) => _jobRepository.GetById(id);

    public IReadOnlyList<Job> SearchOpenJobs(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Array.Empty<Job>();
        }

        return _jobRepository.SearchOpenJobs(searchTerm.Trim());
    }

    public (bool Success, Job? Job, string? ErrorMessage) CreateJob(
        string title,
        string description,
        Guid customerId,
        DateTime startDate,
        DateTime dueDate,
        decimal budget)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
        {
            return (false, null, "Title and description are required.");
        }

        if (customerId == Guid.Empty)
        {
            return (false, null, "Customer ID is required.");
        }

        if (budget <= 0)
        {
            return (false, null, "Budget must be greater than zero.");
        }

        if (dueDate < startDate)
        {
            return (false, null, "Due date cannot be before start date.");
        }

        var job = new Job(title, description, customerId, startDate, dueDate, budget);
        _jobRepository.Add(job);
        return (true, job, null);
    }

    public (bool Success, Job? Job, string? ErrorMessage) UpdateJob(
        Guid id,
        string title,
        string description,
        DateTime startDate,
        DateTime dueDate,
        decimal budget)
    {
        var job = _jobRepository.GetById(id);
        if (job is null)
        {
            return (false, null, "Job not found.");
        }

        try
        {
            job.UpdateDetails(title, description, startDate, dueDate, budget);
            _jobRepository.SaveChanges();
            return (true, job, null);
        }
        catch (DomainException ex)
        {
            return (false, null, ex.Message);
        }
    }

    public (bool Success, string? ErrorMessage) DeleteJob(Guid id)
    {
        var job = _jobRepository.GetById(id);
        if (job is null)
        {
            return (false, "Job not found.");
        }

        try
        {
            job.Cancel();
            _jobRepository.SaveChanges();
            return (true, null);
        }
        catch (DomainException ex)
        {
            return (false, ex.Message);
        }
    }
}
