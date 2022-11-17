using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpleCustomerApi.Dto;
using SimpleCustomerApi.Models;
using SimpleCustomerApi.Services;

namespace SimpleCustomerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController: ControllerBase
{
    private readonly IDataService<Customer> _customerService;
    private readonly IMapper _mapper;

    public CustomersController(IDataService<Customer> customerService, IMapper mapper)
    {
        _customerService = customerService;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQueryDto paginationQueryDto)
    {
        IEnumerable<Customer> customers = await _customerService.GetAllAsync();
        var response = _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        if (response is null)
            return BadRequest();
        
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
            return BadRequest();
        
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CustomerRequestDto customerResponseDto)
    {
        Customer? customer = _mapper.Map<Customer>(customerResponseDto);
        
        if (customer is null)
            return BadRequest();
        
        var customerResponse = await _customerService.CreateAsync(customer);
        if (customerResponse == 0)
            return BadRequest("No Customer was created");
        
        var response = _mapper.Map<CustomerResponseDto>(customer);
        return Ok(response);
    }
}