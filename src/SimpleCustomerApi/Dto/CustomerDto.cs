using System.ComponentModel.DataAnnotations;

namespace SimpleCustomerApi.Dto;

public abstract class CustomerDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [EmailAddress] public string Email { get; set; }
    public bool Enabled { get; set; }
}