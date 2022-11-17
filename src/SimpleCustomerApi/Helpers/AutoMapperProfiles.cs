using AutoMapper;
using SimpleCustomerApi.Dto;
using SimpleCustomerApi.Models;

namespace SimpleCustomerApi.Helpers;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Customer, CustomerResponseDto>();
        CreateMap<CustomerRequestDto, Customer>();
    }
}