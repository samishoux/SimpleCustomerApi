namespace SimpleCustomerApi.Filters;

public class PaginationFilter
{
    public PaginationFilter()
    {
        PageNumber = 1;
        PageSize = 100;
    }

    public PaginationFilter(int pageSize)
    {
        PageNumber = 1;
        PageSize = pageSize;
    }

    public PaginationFilter(int pageNumber, int pageSize)
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

    public int Take => PageSize <= 0 ? 100 : PageSize;

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