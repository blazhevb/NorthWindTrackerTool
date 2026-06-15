using Microsoft.AspNetCore.Mvc;
using NorthWindTracker.Application.Services;

namespace NorthWindTracker.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController(CustomerService customerService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? name)
    {
        var customers = await customerService.GetAllAsync(name);

        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var customer = await customerService.GetByIdAsync(id);

        if (customer is null) 
            return NotFound();

        return Ok(customer);
    }
}
