using System.Net;
using System.Net.Http.Json;
using Bogus;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using SimpleCustomerApi.Dto.Customer;
using Xunit;

namespace SimpleCustomerApi.Tests.Integration.CustomerController;

public class CreateCustomerControllerTest : IClassFixture<SimpleCustomerApiFactory>
{
    private readonly HttpClient _httpClient;

    private readonly Faker<CustomerRequestDto> _faker = new Faker<CustomerRequestDto>()
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.FirstName, faker => faker.Person.FirstName)
        .RuleFor(x => x.LastName, faker => faker.Person.LastName)
        .RuleFor(x => x.Enabled, faker => faker.Random.Bool());

    public CreateCustomerControllerTest(SimpleCustomerApiFactory apiFactory)
    {
        _httpClient = apiFactory.CreateClient();
    }

    [Fact]
    public async Task Create_ShouldCreateUser_WhenDataIsValid()
    {
        // Arrange
        var customer = _faker.Generate();

        // Act
        var response = await _httpClient.PostAsJsonAsync("Customer", customer);

        // Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponseDto>();
        customerResponse.Should().NotBeNull();
        customerResponse.Should().BeEquivalentTo(customer);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().Be($"/Customer/{customerResponse!.Id}");
    }

    [Fact]
    public async Task Create_ShouldReturnValidationError_WhenEmailIsInvalid()
    {
        // Arrange
        const string invalidEmail = "not A Real Email";
        var customer = _faker.Clone()
            .RuleFor(x => x.Email, invalidEmail)
            .Generate();

        // Act
        var response = await _httpClient.PostAsJsonAsync("Customer", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errorResponse.Should().NotBeNull();
        errorResponse!.Status.Should().Be(400);
        errorResponse.Title.Should().Be("One or more validation errors occurred.");
        errorResponse.Errors["Email"].Should().HaveCount(1);
        errorResponse.Errors["Email"].First().Should().Be("The Email field is not a valid e-mail address.");
    }
}