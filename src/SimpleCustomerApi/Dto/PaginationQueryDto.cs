namespace SimpleCustomerApi.Dto;

/// <summary>
/// Pagination query received by the client
/// </summary>
public class PaginationQueryDto
{
    public PaginationQueryDto()
    {
    }

    public PaginationQueryDto(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;

    }

   
    public int? PageNumber { get; set; }
    
    
    public int? PageSize { get; set; }
}