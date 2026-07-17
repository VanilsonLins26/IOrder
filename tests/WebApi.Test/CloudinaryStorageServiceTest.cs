using IOrder.infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Shouldly;

namespace WebApi.Test;

public class CloudinaryStorageServiceTest
{
    [Theory]
    [InlineData("https://res.cloudinary.com/demo/image/upload/v1234567890/products/image.jpg", "products/image")]
    [InlineData("https://res.cloudinary.com/demo/image/upload/products/image.jpg", "products/image")]
    [InlineData("https://res.cloudinary.com/demo/image/upload/v1234567890/products/subdir/image.png", "products/subdir/image")]
    public void ExtractPublicId_Should_Parse_Correctly(string url, string expected)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Cloudinary:CloudName"] = "demo",
            ["Cloudinary:ApiKey"] = "key",
            ["Cloudinary:ApiSecret"] = "secret"
        }).Build();

        var service = new CloudinaryStorageService(config);

        var result = service.ExtractPublicId(url);

        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-cloudinary-url")]
    public void ExtractPublicId_Should_Return_Null_For_Invalid_Urls(string url)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Cloudinary:CloudName"] = "demo",
            ["Cloudinary:ApiKey"] = "key",
            ["Cloudinary:ApiSecret"] = "secret"
        }).Build();

        var service = new CloudinaryStorageService(config);

        var result = service.ExtractPublicId(url);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteImageAsync_Should_Not_Throw_When_Url_Is_Null()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Cloudinary:CloudName"] = "demo",
            ["Cloudinary:ApiKey"] = "key",
            ["Cloudinary:ApiSecret"] = "secret"
        }).Build();

        var service = new CloudinaryStorageService(config);

        await service.DeleteImageAsync(null!);
    }

    [Fact]
    public async Task DeleteImageAsync_Should_Not_Throw_When_Url_Is_Empty()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Cloudinary:CloudName"] = "demo",
            ["Cloudinary:ApiKey"] = "key",
            ["Cloudinary:ApiSecret"] = "secret"
        }).Build();

        var service = new CloudinaryStorageService(config);

        await service.DeleteImageAsync(string.Empty);
    }

    [Fact]
    public async Task DeleteImageAsync_Should_Not_Throw_When_Url_Is_Invalid()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Cloudinary:CloudName"] = "demo",
            ["Cloudinary:ApiKey"] = "key",
            ["Cloudinary:ApiSecret"] = "secret"
        }).Build();

        var service = new CloudinaryStorageService(config);

        await service.DeleteImageAsync("not-a-valid-cloudinary-url");
    }
}
