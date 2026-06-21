using MarketplaceOutsourcing.Api.Dtos;
using MarketplaceOutsourcing.Api.Mapping;
using MarketplaceOutsourcing.Application.Interfaces;
using MarketplaceOutsourcing.Application.Services;
using MarketplaceOutsourcing.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceOutsourcing.Api.Controllers;

[ApiController]
[Route("joboffers")]
public class JobOffersController : ControllerBase
{
    private readonly JobOfferService _jobOfferService;
    private readonly ICurrentUserService _currentUser;

    public JobOffersController(JobOfferService jobOfferService, ICurrentUserService currentUser)
    {
        _jobOfferService = jobOfferService;
        _currentUser = currentUser;
    }

    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<JobOfferResponse>> GetAll()
    {
        var offers = _jobOfferService.ListOffers().Select(o => o.ToResponse());
        return Ok(offers);
    }

    [AllowAnonymous]
    [HttpGet("job/{jobId:guid}")]
    public ActionResult<IEnumerable<JobOfferResponse>> GetByJob(Guid jobId)
    {
        var offers = _jobOfferService.GetOffersForJob(jobId).Select(o => o.ToResponse());
        return Ok(offers);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public ActionResult<JobOfferResponse> GetById(Guid id)
    {
        var offer = _jobOfferService.GetOffer(id);
        return offer is null ? NotFound() : Ok(offer.ToResponse());
    }

    [Authorize(Roles = AppRoles.Contractor)]
    [HttpPost]
    public ActionResult<JobOfferResponse> Create([FromBody] CreateJobOfferRequest request)
    {
        if (_currentUser.ContractorId is null)
        {
            return Forbid();
        }

        var (success, offer, errorMessage) = _jobOfferService.CreateOffer(
            request.JobId,
            _currentUser.ContractorId.Value,
            request.Price);

        if (!success || offer is null)
        {
            return BadRequest(errorMessage ?? "Could not create offer.");
        }

        return CreatedAtAction(nameof(GetById), new { id = offer.Id }, offer.ToResponse());
    }

    [Authorize(Roles = AppRoles.Contractor)]
    [HttpPut("{id:guid}")]
    public ActionResult<JobOfferResponse> Update(Guid id, [FromBody] UpdateJobOfferRequest request)
    {
        var (success, offer, errorMessage) = _jobOfferService.UpdateOffer(id, request.Price);

        if (!success || offer is null)
        {
            return errorMessage == "Offer not found." ? NotFound() : BadRequest(errorMessage);
        }

        return Ok(offer.ToResponse());
    }

    [Authorize(Roles = AppRoles.Contractor)]
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var (success, errorMessage) = _jobOfferService.DeleteOffer(id);

        if (!success)
        {
            return errorMessage == "Offer not found." ? NotFound() : BadRequest(errorMessage);
        }

        return NoContent();
    }

    [Authorize(Roles = AppRoles.Customer)]
    [HttpPost("{id:guid}/accept")]
    public ActionResult<AcceptOfferResponse> Accept(Guid id)
    {
        var (success, job, errorMessage) = _jobOfferService.AcceptOffer(id);

        if (!success || job is null)
        {
            return errorMessage switch
            {
                "Offer not found." => NotFound(),
                "Job not found." => NotFound(),
                _ => BadRequest(errorMessage)
            };
        }

        return Ok(new AcceptOfferResponse(
            job.Id,
            id,
            job.AcceptedContractorId!.Value,
            job.Status.ToString()));
    }
}
