using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SimpleCustomerApi.Dto.Customer;
using Xunit;

namespace SimpleCustomerApi.Tests.Integration.CustomerController;

public class UpdateCustomerControllerTest : IClassFixture<SimpleCustomerApiFactory>
{
    private readonly HttpClient _httpClient;

    public UpdateCustomerControllerTest(SimpleCustomerApiFactory apiFactory)
    {
        _httpClient = apiFactory.CreateClient();
    }

    [Fact]
    private async Task Update_ShouldUpdateCustomer_WhenCustomerExist()
    {
        // Arrange
        Guid id = Guid.Parse("89502fd0-86b4-4ee6-8ae6-a4423ac50c03");
        CustomerRequestDto customer = new CustomerRequestDto()
        {
            Email = "Joshua45@yahoo.com",
            FirstName = "Joshua updated",
            Enabled = false,
            LastName = "Brekke updated"
        };
        
        // Act
        var response = await _httpClient.PutAsJsonAsync($"Customer/{id}", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponseDto>();
        customerResponse.Should().BeEquivalentTo(customer);
        customerResponse!.UpdatedAt.Should().BeAfter(DateTime.Parse("2022-08-29").ToUniversalTime());
    }
    
    [Fact]
    private async Task Update_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        CustomerRequestDto customer = new CustomerRequestDto()
        {
            Email = "Joshua45@yahoo.com",
            FirstName = "Joshua updated",
            Enabled = false,
            LastName = "Brekke updated"
        };
        
        // Act
        var response = await _httpClient.PutAsJsonAsync($"Customer/{Guid.NewGuid()}", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}