using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IOrder.infrastructure.Services.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace UseCases.Test.Infrastructure.Cache;

public class RedisCacheServiceTest
{
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly RedisCacheService _cacheService;

    public RedisCacheServiceTest()
    {
        _cacheMock = new Mock<IDistributedCache>();

        _cacheService = new RedisCacheService(_cacheMock.Object);
    }

    private class TestObject
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    [Fact]
    public async Task GetAsync_WhenKeyExists_ShouldReturnDeserializedObject()
    {
        // Arrange
        var testObject = new TestObject { Name = "Test", Value = 123 };
        var json = JsonSerializer.Serialize(testObject);
        var bytes = Encoding.UTF8.GetBytes(json);

        _cacheMock.Setup(x => x.GetAsync("my-key", It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);

        // Act
        var result = await _cacheService.GetAsync<TestObject>("my-key");

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetAsync_WhenKeyDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _cacheMock.Setup(x => x.GetAsync("missing-key", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null);

        // Act
        var result = await _cacheService.GetAsync<string>("missing-key");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_ShouldSerializeAndSetInCache()
    {
        // Arrange
        var testObject = new { Name = "SetTest" };
        var expiration = TimeSpan.FromMinutes(10);

        // Act
        await _cacheService.SetAsync("set-key", testObject, expiration);

        // Assert
        _cacheMock.Verify(x => x.SetAsync(
            "set-key",
            It.Is<byte[]>(b => Encoding.UTF8.GetString(b).Contains("SetTest")),
            It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == expiration),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_ShouldCallRemoveOnCache()
    {
        // Act
        await _cacheService.RemoveAsync("remove-key");

        // Assert
        _cacheMock.Verify(x => x.RemoveAsync("remove-key", It.IsAny<CancellationToken>()), Times.Once);
    }
}
