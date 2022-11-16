using Microsoft.EntityFrameworkCore;
using SimpleCustomerApi.Models;

namespace SimpleCustomerApi.Data;

public class DataContext: DbContext
{
    private readonly IConfiguration _configuration;

    public DataContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_configuration.GetConnectionString("WebApiDatabase"));
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Customer> Customers { get; set; }
    
    
}