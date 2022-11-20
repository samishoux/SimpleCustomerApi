using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleCustomerApi.Data;
using Xunit;

namespace SimpleCustomerApi.Tests.Integration;

public class SimpleCustomerApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    /// <summary>
    /// Container setup for testing with a temporary database
    /// that will get dropped once the tests are done running
    /// </summary>
    private readonly TestcontainerDatabase _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "integrationDb",
            Password = "password",
            Username = "root"
        })
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<DataContext>));
            services.AddDbContext<DataContext>(options => { options.UseNpgsql(_dbContainer.ConnectionString); });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}