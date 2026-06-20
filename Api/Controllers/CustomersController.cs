using MarketplaceOutsourcing.Api.Dtos;
using MarketplaceOutsourcing.Api.Mapping;
using MarketplaceOutsourcing.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceOutsourcing.Api.Controllers;

[ApiController]
[Route("customers")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CustomerResponse>> GetAll()
    {
        var customers = _customerService.ListCustomers().Select(c => c.ToResponse());
        return Ok(customers);
    }

    [HttpGet("{searchTerm}")]
    public ActionResult<IEnumerable<CustomerResponse>> Search(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length > 100)
        {
            return BadRequest("Search term is required and must be at most 100 characters.");
        }

        var customers = _customerService.SearchCustomers(searchTerm).Select(c => c.ToResponse());
        return Ok(customers);
    }

    [HttpPost]
    public ActionResult<CustomerResponse> Create([FromBody] CreateCustomerRequest request)
    {
        var (success, customer) = _customerService.CreateCustomer(request.FirstName, request.LastName);

        if (!success || customer is null)
        {
            return BadRequest("First name and last name are required.");
        }

        return CreatedAtAction(nameof(Search), new { searchTerm = customer.LastName }, customer.ToResponse());
    }
}
