using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SimpleCustomerApi.Tests.Integration.CustomerController;

public class CreateCustomerControllerTest: IClassFixture<SimpleCustomerApiFactory>
{
    private readonly SimpleCustomerApiFactory _apiFactory;

    public CreateCustomerControllerTest(SimpleCustomerApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }
}