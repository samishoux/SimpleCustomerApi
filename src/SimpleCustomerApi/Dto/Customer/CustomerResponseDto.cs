﻿namespace SimpleCustomerApi.Dto.Customer;

public class CustomerResponseDto: CustomerDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}