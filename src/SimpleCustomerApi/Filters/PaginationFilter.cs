namespace SimpleCustomerApi.Filters;

public class PaginationFilter
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    
    
    public int Skip
    {
        get
        {
            if (PageNumber < 1)
                return 0;
            
            if (PageSize < 1)
                return 0;
            
            return (PageNumber - 1) * PageSize;
        }
    }
}