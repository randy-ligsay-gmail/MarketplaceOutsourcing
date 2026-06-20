using MarketplaceOutsourcing.Domain.Common;
using MarketplaceOutsourcing.Domain.Enums;
using MarketplaceOutsourcing.Domain.Exceptions;

namespace MarketplaceOutsourcing.Domain.Entities;

public class Job : BaseEntity
{
    public Guid CustomerId { get; private set; }

    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Budget { get; set; }

    public JobStatus Status { get; private set; }
    public Guid? AcceptedContractorId { get; private set; }

    private Job() { }

    public Job(
        string title,
        string description,
        Guid customerId,
        DateTime startDate,
        DateTime dueDate,
        decimal budget)
    {
        Title = title;
        Description = description;
        CustomerId = customerId;
        StartDate = startDate;
        DueDate = dueDate;
        Budget = budget;
        Status = JobStatus.Open;
    }

    public void UpdateDetails(
        string title,
        string description,
        DateTime startDate,
        DateTime dueDate,
        decimal budget)
    {
        if (Status != JobStatus.Open)
        {
            throw new DomainException("Only open jobs can be updated.");
        }

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Title and description are required.");
        }

        if (budget <= 0)
        {
            throw new DomainException("Budget must be greater than zero.");
        }

        if (dueDate < startDate)
        {
            throw new DomainException("Due date cannot be before start date.");
        }

        Title = title.Trim();
        Description = description.Trim();
        StartDate = startDate;
        DueDate = dueDate;
        Budget = budget;
    }

    public void Cancel()
    {
        if (Status != JobStatus.Open)
        {
            throw new DomainException("Only open jobs can be cancelled.");
        }

        Status = JobStatus.Cancelled;
    }

    public void AcceptOffer(Guid contractorId)
    {
        if (Status != JobStatus.Open)
        {
            throw new DomainException("Only open jobs can accept an offer.");
        }

        if (contractorId == Guid.Empty)
        {
            throw new DomainException("A valid contractor is required.");
        }

        Status = JobStatus.Accepted;
        AcceptedContractorId = contractorId;
    }

    public string GetDetail()
    {
        var acceptedBy = AcceptedContractorId?.ToString() ?? "none";
        return $"{Id} | {Title} | ${Budget} | {Status} | accepted by {acceptedBy} | due {DueDate:yyyy-MM-dd}";
    }
}
