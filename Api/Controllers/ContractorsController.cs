using MarketplaceOutsourcing.Api.Dtos;
using MarketplaceOutsourcing.Api.Mapping;
using MarketplaceOutsourcing.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceOutsourcing.Api.Controllers;

[ApiController]
[Route("contractors")]
public class ContractorsController : ControllerBase
{
    private readonly ContractorService _contractorService;

    public ContractorsController(ContractorService contractorService)
    {
        _contractorService = contractorService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ContractorResponse>> GetAll()
    {
        var contractors = _contractorService.ListContractors().Select(c => c.ToResponse());
        return Ok(contractors);
    }

    [HttpGet("{searchTerm}")]
    public ActionResult<IEnumerable<ContractorResponse>> Search(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length > 100)
        {
            return BadRequest("Search term is required and must be at most 100 characters.");
        }

        var contractors = _contractorService.SearchContractors(searchTerm).Select(c => c.ToResponse());
        return Ok(contractors);
    }
}
