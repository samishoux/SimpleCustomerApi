using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SimpleCustomerApi.Controllers;
using SimpleCustomerApi.Dto;
using SimpleCustomerApi.Dto.Customer;
using SimpleCustomerApi.Filters;
using SimpleCustomerApi.Models;
using SimpleCustomerApi.Services;
using Xunit;

namespace SimpleCustomerApi.Tests.Unit.Controllers;

public class CustomersControllerTest
{
    private readonly CustomerController _sut;
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly ICustomerService _customerService = Substitute.For<ICustomerService>();

    public CustomersControllerTest()
    {
        _sut = new CustomerController(_customerService, _mapper);
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
        responseDto.Should().BeEquivalentTo(customer);
        await _customerService.Received(1).GetAsync(Arg.Is(id));
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenNoCustomerExist()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        _customerService.GetAsync(Arg.Any<Guid>()).ReturnsNull();

        // Act
        var response = await _sut.Get(id);

        // Assert
        response.Should().BeAssignableTo<NotFoundResult>();
        await _customerService.Received(1).GetAsync(Arg.Is(id));
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
        await _customerService.Received(1).GetAsync(Arg.Is(id));
    }

    #endregion

    #region GetAll

    [Fact]
    public async Task GetAll_ShouldReturnValidCustomers_WhenRequestIsValid()
    {
        // Arrange
        PaginationQueryDto paginationQueryDto = new()
        {
            PageNumber = 1,
            PageSize = 100
        };
        CustomerQueryDto customerQueryDto = new()
        {
            Enabled = true
        };
        PaginationFilter paginationFilter = new();
        CustomerFilter customerFilter = new()
        {
            Enabled = true
        };
        List<Customer> customers = new();
        List<CustomerResponseDto> customerDtoResponse = new();
        _mapper.Map<PaginationFilter>(Arg.Any<PaginationQueryDto>()).Returns(paginationFilter);
        _mapper.Map<CustomerFilter>(Arg.Any<CustomerQueryDto>()).Returns(customerFilter);
        _mapper.Map<IEnumerable<CustomerResponseDto>>(customers).Returns(customerDtoResponse);
        _customerService.GetAllAsync(paginationFilter, customerFilter).Returns(customers);

        // Act
        var response = await _sut.GetAll(paginationQueryDto, customerQueryDto);

        // Assert
        var okObjectResult = response.Should().BeAssignableTo<OkObjectResult>().Subject;
        var responseDto = okObjectResult.Value.Should().BeAssignableTo<IEnumerable<CustomerResponseDto>>().Subject;
        await _customerService.Received(1).GetAllAsync(Arg.Is<PaginationFilter>(x => x != null),
            Arg.Is<CustomerFilter>(x => x != null && x.Enabled));
        _mapper.Received(1).Map<CustomerFilter>(Arg.Any<CustomerQueryDto>());
        responseDto.Should().NotBeNull();
        responseDto.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReceiveNullableCustomerFilter_WhenCustomerQueryDtoEnabledWasNull()
    {
        // Arrange
        PaginationFilter paginationFilter = new();
        List<Customer> customers = new();
        _customerService.GetAllAsync(paginationFilter).Returns(customers);
        _mapper.Map<PaginationFilter>(Arg.Any<PaginationQueryDto>()).Returns(paginationFilter);

        // Act
        await _sut.GetAll(new PaginationQueryDto(), new CustomerQueryDto());

        // Assert
        await _customerService.Received(1)
            .GetAllAsync(Arg.Any<PaginationFilter>(), Arg.Is<CustomerFilter?>(x => x == null));
    }

    [Fact]
    public async Task GetAll_ShouldReceiveNullablePaginationFilter_WhenPaginationQueryDtoDoesNotHavePageSize()
    {
        // Arrange
        PaginationFilter paginationFilter = new();
        PaginationQueryDto paginationQueryDto = new PaginationQueryDto();
        List<Customer> customers = new();
        _customerService.GetAllAsync(paginationFilter).Returns(customers);
        _mapper.Map<PaginationFilter>(Arg.Any<PaginationQueryDto>()).Returns(paginationFilter);

        // Act
        await _sut.GetAll(paginationQueryDto, new CustomerQueryDto());

        // Assert
        await _customerService.Received(1)
            .GetAllAsync(Arg.Is<PaginationFilter?>(x => x == null), Arg.Any<CustomerFilter>());
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
        await _customerService.Received(1).CreateAsync(Arg.Is(customer));
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
        await _customerService.DidNotReceive().CreateAsync(Arg.Any<Customer>());
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
        await _customerService.Received(1).CreateAsync(Arg.Is(customer));
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
        await _customerService.Received(1).CreateAsync(Arg.Is(customer));
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
        responseDto.Should().BeEquivalentTo(customerRequestDto);
        await _customerService.Received(1).GetAsync(Arg.Is(customer.Id));
        await _customerService.Received(1).UpdateAsync(Arg.Is(customer));
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        _customerService.GetAsync(Arg.Any<Guid>()).ReturnsNull();

        // Act
        var response = await _sut.Update(id, new CustomerRequestDto());

        // Assert
        response.Should().BeAssignableTo<NotFoundResult>();
        await _customerService.Received(1).GetAsync(Arg.Is(id));
        await _customerService.DidNotReceive().UpdateAsync(Arg.Any<Customer>());
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
        await _customerService.Received(1).GetAsync(Arg.Is(customer.Id));
        await _customerService.Received(1).UpdateAsync(Arg.Is(customer));
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
        await _customerService.Received(1).GetAsync(Arg.Is(customer.Id));
        await _customerService.Received(1).UpdateAsync(Arg.Is(customer));
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_ShouldOkStatus_WhenRequestIsValid()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        _customerService.DeleteAsync(Arg.Any<Guid>()).Returns(1);
        _customerService.ExistAsync(Arg.Any<Guid>()).Returns(true);

        // Act
        var response = await _sut.Delete(id);

        // Assert
        response.Should().BeAssignableTo<OkResult>();
        await _customerService.Received(1).DeleteAsync(Arg.Is(id));
        await _customerService.Received(1).ExistAsync(Arg.Is(id));
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenNoCustomerWasDeleted()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        _customerService.DeleteAsync(Arg.Any<Guid>()).Returns(0);
        _customerService.ExistAsync(Arg.Any<Guid>()).Returns(true);


        // Act
        var response = await _sut.Delete(id);

        // Assert
        response.Should().BeAssignableTo<NotFoundResult>();
        await _customerService.Received(1).DeleteAsync(id);
        await _customerService.Received(1).ExistAsync(Arg.Is(id));

    }
    
    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenNoCustomerExist()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        _customerService.DeleteAsync(Arg.Any<Guid>()).Returns(0);
        _customerService.ExistAsync(Arg.Any<Guid>()).Returns(false);


        // Act
        var response = await _sut.Delete(id);

        // Assert
        response.Should().BeAssignableTo<NotFoundResult>();
        await _customerService.Received(1).ExistAsync(Arg.Is(id));
        await _customerService.DidNotReceive().DeleteAsync(id);

    }

    #endregion
}