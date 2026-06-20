using MarketplaceOutsourcing.Api.Dtos;
using MarketplaceOutsourcing.Api.Mapping;
using MarketplaceOutsourcing.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceOutsourcing.Api.Controllers;

[ApiController]
[Route("joboffers")]
public class JobOffersController : ControllerBase
{
    private readonly JobOfferService _jobOfferService;

    public JobOffersController(JobOfferService jobOfferService)
    {
        _jobOfferService = jobOfferService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<JobOfferResponse>> GetAll()
    {
        var offers = _jobOfferService.ListOffers().Select(o => o.ToResponse());
        return Ok(offers);
    }

    [HttpGet("job/{jobId:guid}")]
    public ActionResult<IEnumerable<JobOfferResponse>> GetByJob(Guid jobId)
    {
        var offers = _jobOfferService.GetOffersForJob(jobId).Select(o => o.ToResponse());
        return Ok(offers);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<JobOfferResponse> GetById(Guid id)
    {
        var offer = _jobOfferService.GetOffer(id);
        return offer is null ? NotFound() : Ok(offer.ToResponse());
    }

    [HttpPost]
    public ActionResult<JobOfferResponse> Create([FromBody] CreateJobOfferRequest request)
    {
        var (success, offer, errorMessage) = _jobOfferService.CreateOffer(
            request.JobId,
            request.ContractorId,
            request.Price);

        if (!success || offer is null)
        {
            return BadRequest(errorMessage ?? "Could not create offer.");
        }

        return CreatedAtAction(nameof(GetById), new { id = offer.Id }, offer.ToResponse());
    }

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
