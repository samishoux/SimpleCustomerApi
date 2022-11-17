using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SimpleCustomerApi.Data;
using SimpleCustomerApi.Models;

namespace SimpleCustomerApi.Services;

public class CustomerService: IDataService<Customer>
{
    private readonly DataContext _context;

    public CustomerService(DataContext context)
    {
        _context = context;
    }
    
    public async Task<Customer?> GetAsync(Guid id)
    {
        return await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<int> CreateAsync(Customer data)
    {
        await _context.Customers.AddAsync(data);
        return await _context.SaveChangesAsync();;
    }

    public async Task<int> DeleteAsync(Customer data)
    {
        _context.Remove(data);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        Customer? customer = await GetAsync(id);
        if (customer is null)
            return 0;
        
        _context.Remove(customer);
        return await _context.SaveChangesAsync();
    }
}