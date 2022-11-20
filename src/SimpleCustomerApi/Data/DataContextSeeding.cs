using Bogus;
using Microsoft.EntityFrameworkCore;
using SimpleCustomerApi.Models;

namespace SimpleCustomerApi.Data;

public static class DataContextSeeding
{
    /// <summary>
    /// Seed all the data for the application db context
    /// </summary>
    /// <param name="modelBuilder"></param>
    public static void Seed(this ModelBuilder modelBuilder)
    {
        List<Customer> customers = new List<Customer>();

        DateTime createdAt = new DateTime(2021, 11, 19);
        DateTime updatedAt = new DateTime(2022, 11, 19);
        var faker = new Faker<Customer>()
            .RuleFor(prop => prop.Email, fake => fake.Person.Email)
            .RuleFor(prop => prop.FirstName, fake => fake.Person.FirstName)
            .RuleFor(prop => prop.LastName, fake => fake.Person.LastName)
            .RuleFor(prop => prop.Enabled, fake => fake.Random.Bool())
            .RuleFor(prop => prop.CreatedAt, fake => fake.Date.Past(1, createdAt).ToUniversalTime())
            .RuleFor(prop => prop.UpdatedAt, fake => fake.Date.Past(1, updatedAt).ToUniversalTime());

        for (int i = 0; i < 1_000; i++)
        {
            customers.Add(faker.Generate());
        }

        modelBuilder.Entity<Customer>().HasData(customers);
    }
}