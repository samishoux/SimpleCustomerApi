using AutoMapper;
using SimpleCustomerApi.Dto;
using SimpleCustomerApi.Dto.Customer;
using SimpleCustomerApi.Filters;
using SimpleCustomerApi.Models;

namespace SimpleCustomerApi.Helpers;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Customer, CustomerResponseDto>();
        CreateMap<CustomerRequestDto, Customer>();
        CreateMap<PaginationQueryDto, PaginationFilter>();
        CreateMap<CustomerQueryDto, CustomerFilter>();
    }
}