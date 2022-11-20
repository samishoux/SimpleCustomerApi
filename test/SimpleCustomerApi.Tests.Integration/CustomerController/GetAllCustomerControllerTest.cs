using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using FluentAssertions;
using SimpleCustomerApi.Dto.Customer;
using Xunit;

namespace SimpleCustomerApi.Tests.Integration.CustomerController;

public class GetAllCustomerControllerTest : IClassFixture<SimpleCustomerApiFactory>
{
    private readonly HttpClient _httpClient;

    public GetAllCustomerControllerTest(SimpleCustomerApiFactory apiFactory)
    {
        _httpClient = apiFactory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldGetAllCustomers_WhenNoFilterArePresent()
    {
        // Act
        var response = await _httpClient.GetAsync("Customer");

        // Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<List<CustomerResponseDto>>();
        customerResponse.Should().NotBeNull();
        customerResponse.Should().HaveCount(1000);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Theory]
    [InlineData(1, 10, 10)]
    [InlineData(10, 10, 10)]
    [InlineData(1, 40, 40)]
    [InlineData(1, 150, 150)]
    [InlineData(-1, 10, 10)]
    [InlineData(0, 10, 10)]
    [InlineData(0, 0, 100)]
    [InlineData(0, -1, 100)]
    [InlineData(0, -100, 100)]
    public async Task GetAll_ShouldOnlyGetSpecifiedAmountOfCustomers_WhenPagingFilterIsPresent(int pageNumber,
        int pageSize, int expectedCount)
    {
        // Arrange
        var url = $"Customer?PageNumber={pageNumber}&PageSize={pageSize}";

        // Act
        var response = await _httpClient.GetAsync(url);

        // Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<List<CustomerResponseDto>>();
        customerResponse.Should().NotBeNull();
        customerResponse.Should().HaveCount(expectedCount);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Theory]
    [InlineData(10, 10)]
    [InlineData(40, 40)]
    [InlineData(150, 150)]
    [InlineData(0, 100)]
    [InlineData(-1, 100)]
    [InlineData(-100, 100)]
    public async Task GetAll_ShouldOnlyGetSpecifiedAmountOfCustomers_WhenPartialPagingFilterIsPresent(int pageSize,
        int expectedCount)
    {
        // Arrange
        var url = $"Customer?PageSize={pageSize}";

        // Act
        var response = await _httpClient.GetAsync(url);

        // Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<List<CustomerResponseDto>>();
        customerResponse.Should().NotBeNull();
        customerResponse.Should().HaveCount(expectedCount);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_ShouldContainEnableAndDisabledCustomer_WhenNoEnabledFilterIsPresent()
    {
        // Arrange
        var url = $"Customer";

        // Act
        var response = await _httpClient.GetAsync(url);

        // Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<List<CustomerResponseDto>>();
        customerResponse.Should().NotBeNull();
        customerResponse.Should().Contain(x => x.Enabled);
        customerResponse.Should().Contain(x => !x.Enabled);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetAll_ShouldOnlyContainEnableOrDisabledCustomer_WhenEnabledFilterIsPresent(bool enabled)
    {
        // Arrange
        var url = $"Customer?Enabled={enabled}";

        // Act
        var response = await _httpClient.GetAsync(url);

        // Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<List<CustomerResponseDto>>();
        customerResponse.Should().NotBeNull();
        customerResponse.Should().AllSatisfy(x => x.Enabled.Should().Be(enabled));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}