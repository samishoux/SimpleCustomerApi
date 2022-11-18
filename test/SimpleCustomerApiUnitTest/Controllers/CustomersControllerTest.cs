using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SimpleCustomerApi.Controllers;
using SimpleCustomerApi.Dto;
using SimpleCustomerApi.Dto.Customer;
using SimpleCustomerApi.Filters;
using SimpleCustomerApi.Models;
using SimpleCustomerApi.Services;
using Xunit;

namespace SimpleCustomerApiUnitTest.Controllers;

public class CustomersControllerTest
{
    private readonly CustomersController _sut;
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly ICustomerService _customerService = Substitute.For<ICustomerService>();

    public CustomersControllerTest()
    {
        _sut = new CustomersController(_customerService, _mapper);
    }

    #region Get

    [Fact]
    public async Task Get_ShouldReturnValidResponse_WhenCustomerExist()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Customer customer = new Customer()
        {
            Email = "test@test.com",
            Enabled = true,
            Id = id,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            FirstName = "first name",
            LastName = "last name"
        };
        _customerService.GetAsync(id).Returns(customer);
        CustomerResponseDto customerResponseDto = new CustomerResponseDto()
        {
            Email = customer.Email,
            Enabled = customer.Enabled,
            FirstName = customer.FirstName,
            Id = customer.Id,
            CreatedAt = customer.CreatedAt,
            LastName = customer.LastName,
            UpdatedAt = customer.UpdatedAt
        };
        _mapper.Map<CustomerResponseDto>(customer).Returns(customerResponseDto);

        // Act
        var response = await _sut.Get(id);

        // Assert
        var okObjectResult = response.Should().BeAssignableTo<OkObjectResult>().Subject;
        var responseDto = okObjectResult.Value.Should().BeAssignableTo<CustomerResponseDto>().Subject;
        responseDto.Should().NotBeNull();
        responseDto.Id.Should().Be(id);
        responseDto.FirstName.Should().Be(customer.FirstName);
        responseDto.LastName.Should().Be(customer.LastName);
        responseDto.Email.Should().Be(customer.Email);
        responseDto.CreatedAt.Should().Be(customer.CreatedAt);
        responseDto.UpdatedAt.Should().Be(customer.UpdatedAt);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenNoCustomerExist()
    {
        // Arrange
        _customerService.GetAsync(Arg.Any<Guid>()).ReturnsNull();

        // Act
        var response = await _sut.Get(Guid.NewGuid());

        // Assert
        response.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Get_ShouldReturnBadRequest_WhenMapConversionFails()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Customer customer = new Customer()
        {
            Email = "test@test.com",
            Enabled = true,
            Id = id,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            FirstName = "first name",
            LastName = "last name"
        };
        _customerService.GetAsync(id).Returns(customer);
        _mapper.Map<CustomerResponseDto>(customer).ReturnsNull();

        // Act
        var response = await _sut.Get(id);

        // Assert
        var badRequestObjectResult = response.Should().BeAssignableTo<BadRequestObjectResult>().Subject;
        var responseDto = badRequestObjectResult.Value.Should().BeAssignableTo<string>().Subject;
        responseDto.Should().Be("Could not convert response back");
    }

    #endregion

    #region GetAll

    [Fact]
    public async Task GetAll_ShouldReturnValidCustomers_WhenRequestIsValid()
    {
        // Arrange
        PaginationFilter paginationFilter = new();
        CustomerFilter customerFilter = new();
        List<Customer> customers = new();
        List<CustomerResponseDto> customerDtoResponse = new();
        _mapper.Map<IEnumerable<CustomerResponseDto>>(customers).Returns(customerDtoResponse);
        _customerService.GetAllAsync(paginationFilter, customerFilter).Returns(customers);

        // Act
        var response = await _sut.GetAll(new PaginationQueryDto(), new CustomerQueryDto());

        // Assert
        var okObjectResult = response.Should().BeAssignableTo<OkObjectResult>().Subject;
        var responseDto = okObjectResult.Value.Should().BeAssignableTo<IEnumerable<CustomerResponseDto>>().Subject;
        responseDto.Should().NotBeNull();
        responseDto.Should().BeEmpty();
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_ShouldReturnValidCustomers_WhenRequestIsValid()
    {
        // Arrange
        CustomerRequestDto customerRequestDto = new CustomerRequestDto()
        {
            Email = "valid@email.com",
            Enabled = true,
            FirstName = "firstname",
            LastName = "lastname"
        };
        Customer customer = new Customer()
        {
            Email = customerRequestDto.Email,
            Enabled = customerRequestDto.Enabled,
            FirstName = customerRequestDto.FirstName,
            LastName = customerRequestDto.LastName,
        };
        CustomerResponseDto customerResponseDto = new CustomerResponseDto()
        {
            Email = customer.Email,
            Enabled = customer.Enabled,
            Id = customer.Id,
            CreatedAt = customer.CreatedAt,
            FirstName = customer.FirstName,
            LastName = customer.FirstName,
            UpdatedAt = customer.UpdatedAt
        };
        _mapper.Map<Customer>(customerRequestDto).Returns(customer);
        _customerService.CreateAsync(customer).Returns(1);
        _mapper.Map<CustomerResponseDto>(customer).Returns(customerResponseDto);
        _sut.Url = Substitute.For<IUrlHelper>();
        _sut.Url.Action("Get", $"/{customer.Id}").Returns($"/{customer.Id}");

        // Act
        var response = await _sut.Create(customerRequestDto);

        // Assert
        var okObjectResult = response.Should().BeAssignableTo<CreatedResult>().Subject;
        var responseDto = okObjectResult.Value.Should().BeAssignableTo<CustomerResponseDto>().Subject;
        responseDto.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenCustomerDtoCannotBeMapped()
    {
        // Arrange
        CustomerRequestDto customerRequestDto = new CustomerRequestDto()
        {
            Email = "valid@email.com",
            Enabled = true,
            FirstName = "firstname",
            LastName = "lastname"
        };

        // Act
        var response = await _sut.Create(customerRequestDto);

        // Assert
        response.Should().BeAssignableTo<BadRequestResult>();
    }

    [Fact]
    public async Task Create_ShouldBadRequestWithMessage_WhenNoCustomerWasCreated()
    {
        // Arrange
        CustomerRequestDto customerRequestDto = new CustomerRequestDto()
        {
            Email = "valid@email.com",
            Enabled = true,
            FirstName = "firstname",
            LastName = "lastname"
        };
        Customer customer = new Customer()
        {
            Email = customerRequestDto.Email,
            Enabled = customerRequestDto.Enabled,
            FirstName = customerRequestDto.FirstName,
            LastName = customerRequestDto.LastName,
        };
        _mapper.Map<Customer>(customerRequestDto).Returns(customer);
        _customerService.CreateAsync(customer).Returns(0);

        // Act
        var response = await _sut.Create(customerRequestDto);

        // Assert
        var okObjectResult = response.Should().BeAssignableTo<BadRequestObjectResult>().Subject;
        var responseDto = okObjectResult.Value.Should().BeAssignableTo<string>().Subject;
        responseDto.Should().Be("No Customer was created");
    }

    [Fact]
    public async Task Create_ShouldBadRequestWithMessage_WhenCreatedCustomerCannotBeConvertedBackToResponseDto()
    {
        // Arrange
        CustomerRequestDto customerRequestDto = new CustomerRequestDto()
        {
            Email = "valid@email.com",
            Enabled = true,
            FirstName = "firstname",
            LastName = "lastname"
        };
        Customer customer = new Customer()
        {
            Email = customerRequestDto.Email,
            Enabled = customerRequestDto.Enabled,
            FirstName = customerRequestDto.FirstName,
            LastName = customerRequestDto.LastName,
        };
        _mapper.Map<Customer>(customerRequestDto).Returns(customer);
        _customerService.CreateAsync(customer).Returns(1);

        // Act
        var response = await _sut.Create(customerRequestDto);

        // Assert
        var okObjectResult = response.Should().BeAssignableTo<BadRequestObjectResult>().Subject;
        var responseDto = okObjectResult.Value.Should().BeAssignableTo<string>().Subject;
        responseDto.Should().Be("Could not convert response back");
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_ShouldReturnValidCustomersResponse_WhenRequestIsValid()
    {
        // Arrange
        CustomerRequestDto customerRequestDto = new CustomerRequestDto()
        {
            Email = "valid@email.com",
            Enabled = true,
            FirstName = "firstname",
            LastName = "lastname"
        };
        Customer customer = new Customer()
        {
            Email = customerRequestDto.Email,
            Enabled = customerRequestDto.Enabled,
            FirstName = customerRequestDto.FirstName,
            LastName = customerRequestDto.LastName,
        };
        CustomerResponseDto customerResponseDto = new CustomerResponseDto()
        {
            Email = customer.Email,
            Enabled = customer.Enabled,
            Id = customer.Id,
            CreatedAt = customer.CreatedAt,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            UpdatedAt = customer.UpdatedAt
        };
        _customerService.GetAsync(customer.Id).Returns(customer);
        _customerService.UpdateAsync(customer).Returns(1);
        _mapper.Map<CustomerResponseDto>(customer).Returns(customerResponseDto);

        // Act
        var response = await _sut.Update(customer.Id, customerRequestDto);

        // Assert
        var okObjectResult = response.Should().BeAssignableTo<OkObjectResult>().Subject;
        var responseDto = okObjectResult.Value.Should().BeAssignableTo<CustomerResponseDto>().Subject;
        responseDto.Should().NotBeNull();
        responseDto.Id.Should().Be(customer.Id);
        responseDto.FirstName.Should().Be(customer.FirstName);
        responseDto.LastName.Should().Be(customer.LastName);
        responseDto.Email.Should().Be(customer.Email);
        responseDto.CreatedAt.Should().Be(customer.CreatedAt);
        responseDto.UpdatedAt.Should().Be(customer.UpdatedAt);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        _customerService.GetAsync(Arg.Any<Guid>()).ReturnsNull();

        // Act
        var response = await _sut.Update(Guid.NewGuid(), new CustomerRequestDto());

        // Assert
        response.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequestWithMessage_WhenNoCustomerWasUpdated()
    {
        // Arrange
        CustomerRequestDto customerRequestDto = new CustomerRequestDto()
        {
            Email = "valid@email.com",
            Enabled = true,
            FirstName = "firstname",
            LastName = "lastname"
        };
        Customer customer = new Customer()
        {
            Email = customerRequestDto.Email,
            Enabled = customerRequestDto.Enabled,
            FirstName = customerRequestDto.FirstName,
            LastName = customerRequestDto.LastName,
        };
        _customerService.GetAsync(customer.Id).Returns(customer);
        _customerService.UpdateAsync(customer).Returns(0);

        // Act
        var response = await _sut.Update(customer.Id, customerRequestDto);

        // Assert
        var okObjectResult = response.Should().BeAssignableTo<BadRequestObjectResult>().Subject;
        var responseDto = okObjectResult.Value.Should().BeAssignableTo<string>().Subject;
        responseDto.Should().Be("No Customer was updated");
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenUpdatedCustomerCannotBeConvertedBackToResponseDto()
    {
        // Arrange
        CustomerRequestDto customerRequestDto = new CustomerRequestDto()
        {
            Email = "valid@email.com",
            Enabled = true,
            FirstName = "firstname",
            LastName = "lastname"
        };
        Customer customer = new Customer()
        {
            Email = customerRequestDto.Email,
            Enabled = customerRequestDto.Enabled,
            FirstName = customerRequestDto.FirstName,
            LastName = customerRequestDto.LastName,
        };
        _customerService.GetAsync(customer.Id).Returns(customer);
        _customerService.UpdateAsync(customer).Returns(1);

        // Act
        var response = await _sut.Update(customer.Id, customerRequestDto);

        // Assert
        var okObjectResult = response.Should().BeAssignableTo<BadRequestObjectResult>().Subject;
        var responseDto = okObjectResult.Value.Should().BeAssignableTo<string>().Subject;
        responseDto.Should().Be("Could not convert response back");
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_ShouldOkStatus_WhenRequestIsValid()
    {
        // Arrange
        _customerService.DeleteAsync(Arg.Any<Guid>()).Returns(1);

        // Act
        var response = await _sut.Delete(Guid.NewGuid());

        // Assert
        response.Should().BeAssignableTo<OkResult>();
    }

    [Fact]
    public async Task Delete_ShouldNotFound_WhenNoCustomerWasDeleted()
    {
        // Arrange
        _customerService.DeleteAsync(Arg.Any<Guid>()).Returns(0);

        // Act
        var response = await _sut.Delete(Guid.NewGuid());

        // Assert
        response.Should().BeAssignableTo<NotFoundResult>();
    }

    #endregion
}