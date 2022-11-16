using System.ComponentModel.DataAnnotations;

namespace SimpleCustomerApi.Models;

public class Customer: BaseModel
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    
    public bool Enabled { get; set; }
}