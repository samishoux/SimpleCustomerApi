using FluentAssertions;
using SimpleCustomerApi.Filters;
using Xunit;

namespace SimpleCustomerApi.Tests.Unit.Filters;

public class PaginationFilterTest
{

    [Fact]
    public void PaginationFilter_ShouldHaveProperDefaultValues_WhenCreatedWithoutValues()
    {
        // Act
        PaginationFilter paginationFilter = new();
        
        // Assert
        paginationFilter.PageNumber.Should().Be(1);
        paginationFilter.PageSize.Should().Be(100);
    }
    
    [Fact]
    public void PaginationFilter_ShouldHaveProperDefaultValues_WhenCreatedWithPageSize()
    {
        // Act
        PaginationFilter paginationFilter = new(10);
        
        // Assert
        paginationFilter.PageNumber.Should().Be(1);
        paginationFilter.PageSize.Should().Be(10);
    }
    
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
    
    
    [Theory]
    [InlineData(3, 10, 10)]
    [InlineData(1, 3, 3)]
    [InlineData(10, 50, 50)]
    [InlineData(500, 50, 50)]
    [InlineData(500, 150, 150)]
    [InlineData(0, 10, 10)]
    [InlineData(-1, 10, 10)]
    [InlineData(-10, 10, 10)]
    [InlineData(-100, 10, 10)]
    [InlineData(10, 0, 100)]
    [InlineData(10, -1, 100)]
    [InlineData(10, -10, 100)]
    [InlineData(10, -100, 100)]
    public void PaginationFilterTake_ShouldReturnProperValue_WhenPageSizeExist(int pageNumber, int pageSize, int expected)
    {
        // Arrange
        var paginationFilter = new PaginationFilter()
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        
        // Act
        var result = paginationFilter.Take;

        // Assert
        result.Should().Be(expected);
    }
}