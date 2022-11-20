using SimpleCustomerApi.Filters;

namespace SimpleCustomerApi.Services;

public interface IDataService<T>
{
    /// <summary>
    /// Get singe object
    /// </summary>
    /// <param name="id">Guid of the object</param>
    /// <returns></returns>
    public Task<T?> GetAsync(Guid id);

    /// <summary>
    /// Get all object
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<T>> GetAllAsync(PaginationFilter? filter = null);

    /// <summary>
    /// Create object
    /// </summary>
    /// <param name="data">Data of the object</param>
    /// <returns>number of change saved</returns>
    public Task<int> CreateAsync(T data);

    /// <summary>
    /// Update the object
    /// </summary>
    /// <param name="data">Data of the object</param>
    /// <returns>number of change saved</returns>
    public Task<int> UpdateAsync(T data);

    /// <summary>
    /// Delete object
    /// </summary>
    /// <param name="id">Guid of the object</param>
    /// <returns>number of change saved</returns>
    public Task<int> DeleteAsync(Guid id);
    
    /// <summary>
    /// Check if object exist
    /// </summary>
    /// <param name="id">Guid of the object</param>
    /// <returns></returns>
    public Task<bool> ExistAsync(Guid id);

}