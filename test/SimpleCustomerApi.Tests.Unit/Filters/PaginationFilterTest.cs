using FluentAssertions;
using SimpleCustomerApi.Filters;
using Xunit;

namespace SimpleCustomerApi.Tests.Unit.Filters;

public class PaginationFilterTest
{
    [Theory]
    [InlineData(3, 10, 20)]
    [InlineData(1, 3, 0)]
    [InlineData(10, 50, 450)]
    [InlineData(500, 50, 24_950)]
    [InlineData(0, 10, 0)]
    [InlineData(-1, 10, 0)]
    [InlineData(-10, 10, 0)]
    [InlineData(-100, 10, 0)]
    [InlineData(10, 0, 0)]
    [InlineData(10, -1, 0)]
    [InlineData(10, -10, 0)]
    [InlineData(10, -100, 0)]
    public void PaginationFilterSkip_ShouldReturnProperValue_WhenPageNumberAndPageSizeExist(int pageNumber, int pageSize, int expected)
    {
        // Arrange
        var paginationFilter = new PaginationFilter()
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        
        // Act
        var result = paginationFilter.Skip;

        // Assert
        result.Should().Be(expected);
    }
}