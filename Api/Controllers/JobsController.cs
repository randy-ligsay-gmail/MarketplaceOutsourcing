using MarketplaceOutsourcing.Api.Dtos;
using MarketplaceOutsourcing.Api.Mapping;
using MarketplaceOutsourcing.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceOutsourcing.Api.Controllers;

[ApiController]
[Route("jobs")]
public class JobsController : ControllerBase
{
    private readonly JobService _jobService;

    public JobsController(JobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<JobResponse>> GetAll()
    {
        var jobs = _jobService.ListJobs().Select(j => j.ToResponse());
        return Ok(jobs);
    }

    [HttpGet("search/{searchTerm}")]
    public ActionResult<IEnumerable<JobResponse>> SearchOpen(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length > 100)
        {
            return BadRequest("Search term is required and must be at most 100 characters.");
        }

        var jobs = _jobService.SearchOpenJobs(searchTerm).Select(j => j.ToResponse());
        return Ok(jobs);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<JobResponse> GetById(Guid id)
    {
        var job = _jobService.GetJob(id);
        return job is null ? NotFound() : Ok(job.ToResponse());
    }

    [HttpPost]
    public ActionResult<JobResponse> Create([FromBody] CreateJobRequest request)
    {
        var (success, job, errorMessage) = _jobService.CreateJob(
            request.Title,
            request.Description,
            request.CustomerId,
            request.StartDate,
            request.DueDate,
            request.Budget);

        if (!success || job is null)
        {
            return BadRequest(errorMessage ?? "Could not create job.");
        }

        return CreatedAtAction(nameof(GetById), new { id = job.Id }, job.ToResponse());
    }

    [HttpPut("{id:guid}")]
    public ActionResult<JobResponse> Update(Guid id, [FromBody] UpdateJobRequest request)
    {
        var (success, job, errorMessage) = _jobService.UpdateJob(
            id,
            request.Title,
            request.Description,
            request.StartDate,
            request.DueDate,
            request.Budget);

        if (!success || job is null)
        {
            return errorMessage == "Job not found." ? NotFound() : BadRequest(errorMessage);
        }

        return Ok(job.ToResponse());
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var (success, errorMessage) = _jobService.DeleteJob(id);

        if (!success)
        {
            return errorMessage == "Job not found." ? NotFound() : BadRequest(errorMessage);
        }

        return NoContent();
    }
}
