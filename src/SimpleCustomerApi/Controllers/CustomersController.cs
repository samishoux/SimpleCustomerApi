using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpleCustomerApi.Dto;
using SimpleCustomerApi.Dto.Customer;
using SimpleCustomerApi.Filters;
using SimpleCustomerApi.Models;
using SimpleCustomerApi.Services;

namespace SimpleCustomerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IMapper _mapper;

    public CustomersController(ICustomerService customerService, IMapper mapper)
    {
        _customerService = customerService;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQueryDto paginationQueryDto,
        [FromQuery] CustomerQueryDto? customerQueryDto)
    {
        PaginationFilter paginationFilter = _mapper.Map<PaginationFilter>(paginationQueryDto);
        CustomerFilter? customerFilter = null;
        if(customerQueryDto?.Enabled is not null)
            customerFilter = _mapper.Map<CustomerFilter>(customerQueryDto);

        IEnumerable<Customer> customers = await _customerService.GetAllAsync(paginationFilter, customerFilter);

        var response = _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);

        return Ok(response);
    }

    [HttpGet]
    [Route("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(Guid id)
    {
        Customer? customer = await _customerService.GetAsync(id);
        if (customer is null)
            return NotFound();

        var response = _mapper.Map<CustomerResponseDto>(customer);
        if (response is null)
            return BadRequest("Could not convert response back");

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CustomerRequestDto customerRequestDto)
    {
        Customer? customer = _mapper.Map<Customer>(customerRequestDto);

        if (customer is null)
            return BadRequest();

        var customerResponse = await _customerService.CreateAsync(customer);
        if (customerResponse == 0)
            return BadRequest("No Customer was created");

        var response = _mapper.Map<CustomerResponseDto>(customer);
        if (response is null)
            return BadRequest("Could not convert response back");

        var location = Url.Action(nameof(Get), new {id = response.Id}) ?? $"/{response.Id}";
        return Created(location, response);
    }


    [HttpPut]
    [Route("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CustomerRequestDto customerRequestDto)
    {
        Customer? customer = await _customerService.GetAsync(id);
        if (customer is null)
            return NotFound();

        _mapper.Map(customerRequestDto, customer);

        var numberOfCustomerUpdated = await _customerService.UpdateAsync(customer);
        if (numberOfCustomerUpdated == 0)
            return BadRequest("No Customer was updated");

        var response = _mapper.Map<CustomerResponseDto>(customer);
        if (response is null)
            return BadRequest("Could not convert response back");

        return Ok(response);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var numberOfCustomerDeleted = await _customerService.DeleteAsync(id);

        if (numberOfCustomerDeleted == 0)
            return NotFound();

        return Ok();
    }
}