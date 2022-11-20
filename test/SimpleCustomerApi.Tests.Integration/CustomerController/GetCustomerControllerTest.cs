using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SimpleCustomerApi.Dto.Customer;
using Xunit;

namespace SimpleCustomerApi.Tests.Integration.CustomerController;

public class GetCustomerControllerTest : IClassFixture<SimpleCustomerApiFactory>
{
    private readonly HttpClient _httpClient;

    public GetCustomerControllerTest(SimpleCustomerApiFactory apiFactory)
    {
        _httpClient = apiFactory.CreateClient();
    }

    [Fact]
    public async Task Get_ShouldReturnACustomer_WhenCustomerExists()
    {
        // Arrange
        CustomerResponseDto expectedCustomer = new CustomerResponseDto()
        {
            Email = "Warren.MacGyver@yahoo.com",
            LastName = "MacGyver",
            FirstName = "Warren",
            Id = Guid.Parse("00f6a7d3-1929-41d0-881c-9bb304049dc3"),
            Enabled = false,
            CreatedAt = DateTime.Parse("2020-12-18T00:08:39.000789Z").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2022-03-07T14:09:19.150616Z").ToUniversalTime()
        };

        // Act
        var response = await _httpClient.GetAsync($"Customer/{expectedCustomer.Id}");

        // Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponseDto>();
        customerResponse.Should().NotBeNull();
        customerResponse.Should().BeEquivalentTo(expectedCustomer);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_ShouldCustomerNotFound_WhenCustomerDoesNotExistExists()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        // Act
        var response = await _httpClient.GetAsync($"Customer/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}