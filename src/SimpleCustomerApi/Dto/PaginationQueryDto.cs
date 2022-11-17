namespace SimpleCustomerApi.Dto;

/// <summary>
/// Pagination query received by the client
/// </summary>
public class PaginationQueryDto
{
    public PaginationQueryDto()
    {
        PageNumber = 1;
        PageSize = 100;
    }

    public PaginationQueryDto(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;

    }

    /// <summary>
    /// Used to get the page number for a given query.
    /// </summary>
    /// <remarks>
    /// Default = 1
    /// </remarks>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// Used to get the amount of data to get back
    /// </summary>
    /// <remarks>
    ///  Default = 100
    /// </remarks>
    public int PageSize { get; set; }
}