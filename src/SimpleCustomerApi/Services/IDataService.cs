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
    public Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Create object
    /// </summary>
    /// <param name="data">Data of the object</param>
    /// <returns>number of change saved</returns>
    public Task<int> CreateAsync(T data);

    /// <summary>
    /// Delete object
    /// </summary>
    /// <param name="data">Data of the object</param>
    /// <returns>number of change saved</returns>
    public Task<int> DeleteAsync(T data);

    /// <summary>
    /// Delete object
    /// </summary>
    /// <param name="id">Guid of the object</param>
    /// <returns>number of change saved</returns>
    public Task<int> DeleteAsync(Guid id);

}