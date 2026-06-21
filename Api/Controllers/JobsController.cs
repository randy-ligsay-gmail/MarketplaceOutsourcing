using MarketplaceOutsourcing.Api.Dtos;
using MarketplaceOutsourcing.Api.Mapping;
using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Application.Services;
using MarketplaceOutsourcing.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceOutsourcing.Api.Controllers;

[ApiController]
[Route("jobs")]
public class JobsController : ControllerBase
{
    private readonly JobService _jobService;
    private readonly ICurrentUserService _currentUser;

    public JobsController(JobService jobService, ICurrentUserService currentUser)
    {
        _jobService = jobService;
        _currentUser = currentUser;
    }

    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<JobResponse>> GetAll()
    {
        var jobs = _jobService.ListJobs().Select(j => j.ToResponse());
        return Ok(jobs);
    }

    [AllowAnonymous]
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

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public ActionResult<JobResponse> GetById(Guid id)
    {
        var job = _jobService.GetJob(id);
        return job is null ? NotFound() : Ok(job.ToResponse());
    }

    [Authorize(Roles = AppRoles.Customer)]
    [HttpPost]
    public ActionResult<JobResponse> Create([FromBody] CreateJobRequest request)
    {
        if (_currentUser.CustomerId is null)
        {
            return Forbid();
        }

        var (success, job, errorMessage) = _jobService.CreateJob(
            request.Title,
            request.Description,
            _currentUser.CustomerId.Value,
            request.StartDate,
            request.DueDate,
            request.Budget);

        if (!success || job is null)
        {
            return BadRequest(errorMessage ?? "Could not create job.");
        }

        return CreatedAtAction(nameof(GetById), new { id = job.Id }, job.ToResponse());
    }

    [Authorize(Roles = AppRoles.Customer)]
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

    [Authorize(Roles = AppRoles.Customer)]
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
