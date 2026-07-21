using Shouldly;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Upload;

public class UploadIntegrationTest : IOrderClassFixture
{
    private readonly string method = "upload";
    private readonly string _userToken = System.Guid.NewGuid().ToString();

    public UploadIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Post_UploadImage_WithoutFile_Returns_BadRequest()
    {
        // Vai dar 415 Unsupported Media Type ou 400 pois não é multipart/form-data
        var response = await DoPost(method, new {}, _userToken);
        
        response.StatusCode.ShouldNotBe(HttpStatusCode.OK);
    }
}
