using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace WebApi.Test;
public class IOrderClassFixture : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpCLient;

    public IOrderClassFixture(CustomWebApplicationFactory factory)
    {
        _httpCLient = factory.CreateClient();
    }

    protected async Task<HttpResponseMessage> DoPost(string method, object request, string token = "")
    {
        AuthorizeRequest(token);
        return await _httpCLient.PostAsJsonAsync(method, request);
    }

    protected async Task<HttpResponseMessage> DoPut(string method, object request, string token = "")
    {
        AuthorizeRequest(token);
        return await _httpCLient.PutAsJsonAsync(method, request);
    }

    protected async Task<HttpResponseMessage> DoGet(string method, string token = "")
    {
        AuthorizeRequest(token);

        return await _httpCLient.GetAsync(method);
    }

    protected async Task<HttpResponseMessage> DoDelete(string method, string token = "")
    {
        AuthorizeRequest(token);

        return await _httpCLient.DeleteAsync(method);
    }

    private void AuthorizeRequest(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        _httpCLient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}
