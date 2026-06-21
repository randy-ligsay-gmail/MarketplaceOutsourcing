namespace MarketplaceOutsourcing.Application.Services;

using MarketplaceOutsourcing.Application.Caching;
using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Domain.Enums;
using MarketplaceOutsourcing.Domain.Exceptions;

public class JobService
{
    private readonly IJobRepository _jobRepository;
    private readonly ILruCache _cache;

    public JobService(IJobRepository jobRepository, ILruCache cache)
    {
        _jobRepository = jobRepository;
        _cache = cache;
    }

    public IReadOnlyList<Job> ListJobs()
    {
        if (_cache.TryGet(CacheKeys.JobsAll, out IReadOnlyList<Job>? cached) && cached is not null)
        {
            return cached;
        }

        var jobs = _jobRepository.GetAll();
        _cache.Set(CacheKeys.JobsAll, jobs);
        return jobs;
    }

    public Job? GetJob(Guid id)
    {
        var cacheKey = CacheKeys.JobById(id);
        if (_cache.TryGet(cacheKey, out Job? cached))
        {
            return cached;
        }

        var job = _jobRepository.GetById(id);
        if (job is not null)
        {
            _cache.Set(cacheKey, job);
        }

        return job;
    }

    public IReadOnlyList<Job> SearchOpenJobs(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Array.Empty<Job>();
        }

        var normalizedTerm = searchTerm.Trim();
        var cacheKey = CacheKeys.JobsSearch(normalizedTerm);
        if (_cache.TryGet(cacheKey, out IReadOnlyList<Job>? cached) && cached is not null)
        {
            return cached;
        }

        var jobs = _jobRepository.SearchOpenJobs(normalizedTerm);
        _cache.Set(cacheKey, jobs);
        return jobs;
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
        InvalidateJobCache();
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
            InvalidateJobCache();
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
            InvalidateJobCache();
            return (true, null);
        }
        catch (DomainException ex)
        {
            return (false, ex.Message);
        }
    }

    private void InvalidateJobCache()
    {
        _cache.RemoveByPrefix(CacheKeys.JobsPrefix);
        _cache.RemoveByPrefix(CacheKeys.JobOffersPrefix);
    }
}
