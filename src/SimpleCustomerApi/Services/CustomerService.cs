﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SimpleCustomerApi.Data;
using SimpleCustomerApi.Filters;
using SimpleCustomerApi.Models;

namespace SimpleCustomerApi.Services;

public class CustomerService : ICustomerService
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

    public async Task<IEnumerable<Customer>> GetAllAsync(PaginationFilter? filter = null)
    {
        if (filter is null)
            return await _context.Customers.ToListAsync();

        return await _context.Customers
            .Skip(filter.Skip)
            .Take(filter.Take)
            .ToListAsync();
    }

    public async Task<int> CreateAsync(Customer data)
    {
        await _context.Customers.AddAsync(data);
        return await _context.SaveChangesAsync();
        ;
    }

    public async Task<int> UpdateAsync(Customer data)
    {
        _context.Update(data);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        Customer customer = new Customer()
        {
            Id = id
        };

        _context.Attach(customer);
        _context.Remove(customer);
        return await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistAsync(Guid id)
    {
        return await _context.Customers.AnyAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(PaginationFilter? paginationFilter = null,
        CustomerFilter? customerFilter = null)
    {
        IQueryable<Customer> partialQuery = _context.Customers;
        if (customerFilter is not null)
        {
            partialQuery = partialQuery.Where(x => x.Enabled == customerFilter.Enabled);
        }

        if (paginationFilter is not null)
        {
            partialQuery = partialQuery
                .Skip(paginationFilter.Skip)
                .Take(paginationFilter.Take);
        }

        return await partialQuery.ToListAsync();
    }
}