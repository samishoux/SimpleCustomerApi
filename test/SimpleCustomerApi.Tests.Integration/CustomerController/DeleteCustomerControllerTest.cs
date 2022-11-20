using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Xunit;

namespace SimpleCustomerApi.Tests.Integration.CustomerController;

public class DeleteCustomerControllerTest : IClassFixture<SimpleCustomerApiFactory>
{
    private readonly HttpClient _httpClient;


    public DeleteCustomerControllerTest(SimpleCustomerApiFactory apiFactory)
    {
        _httpClient = apiFactory.CreateClient();
    }

    [Fact]
    public async Task Delete_ShouldDeleteCustomer_WhenCustomerExist()
    {
        // Arrange
        Guid id = Guid.Parse("89502fd0-86b4-4ee6-8ae6-a4423ac50c03");

        // Act
        var response = await _httpClient.DeleteAsync($"Customer/{id}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    
    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Act
        var response = await _httpClient.DeleteAsync($"Customer/{Guid.NewGuid()}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}