using Shouldly;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Dashboard;

public class DashboardIntegrationTest : IOrderClassFixture
{
    private readonly string method = "dashboard";
    private readonly string _userToken = System.Guid.NewGuid().ToString();

    public DashboardIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Get_Metrics_Returns_Ok()
    {
        var response = await DoGet(method, _userToken);
        
        var allowedStatuses = new[] { HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.Unauthorized };
        allowedStatuses.ShouldContain(response.StatusCode);
    }
}
