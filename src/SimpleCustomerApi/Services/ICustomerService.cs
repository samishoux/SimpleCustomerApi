using SimpleCustomerApi.Filters;
using SimpleCustomerApi.Models;

namespace SimpleCustomerApi.Services;

public interface ICustomerService : IDataService<Customer>
{
    /// <summary>
    /// Get all object
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<Customer>> GetAllAsync(PaginationFilter? paginationFilter = null,
        CustomerFilter? customerFilter = null);
}