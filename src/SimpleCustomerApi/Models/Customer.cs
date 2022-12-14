using System.ComponentModel.DataAnnotations;

namespace SimpleCustomerApi.Models;

public class Customer: BaseModel
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public bool Enabled { get; set; }
}