using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IOrder.Application.Services.Cache;
using IOrder.Domain.Repositories.Store;
using IOrder.infrastructure.Repositories.Store;
using Moq;
using Xunit;

namespace UseCases.Test.Infrastructure.Cache;

public class CachedStoreReadOnlyRepositoryTest
{
    private readonly Mock<IStoreReadOnlyRepository> _decoratedMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly CachedStoreReadOnlyRepository _cachedRepository;

    public CachedStoreReadOnlyRepositoryTest()
    {
        _decoratedMock = new Mock<IStoreReadOnlyRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _cachedRepository = new CachedStoreReadOnlyRepository(_decoratedMock.Object, _cacheServiceMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenInCache_ShouldReturnFromCache_AndNotCallDecorated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var key = CacheKeys.StoreById(id);
        var cachedStore = new IOrder.Domain.Entities.Store { Id = id, Name = "Cached Store", UserId = "user1" };

        _cacheServiceMock.Setup(c => c.GetAsync<IOrder.Domain.Entities.Store>(key)).ReturnsAsync(cachedStore);

        // Act
        var result = await _cachedRepository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Cached Store");
        _decoratedMock.Verify(d => d.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotInCache_ShouldCallDecorated_AndSetCache()
    {
        // Arrange
        var id = Guid.NewGuid();
        var key = CacheKeys.StoreById(id);
        var dbStore = new IOrder.Domain.Entities.Store { Id = id, Name = "DB Store", UserId = "user1" };

        _cacheServiceMock.Setup(c => c.GetAsync<IOrder.Domain.Entities.Store>(key)).ReturnsAsync((IOrder.Domain.Entities.Store)null);
        _decoratedMock.Setup(d => d.GetByIdAsync(id)).ReturnsAsync(dbStore);

        // Act
        var result = await _cachedRepository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("DB Store");
        _decoratedMock.Verify(d => d.GetByIdAsync(id), Times.Once);
        _cacheServiceMock.Verify(c => c.SetAsync(key, dbStore, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenInCache_ShouldReturnFromCache()
    {
        // Arrange
        var userId = "user1";
        var key = CacheKeys.StoreByUserId(userId);
        var cachedStore = new IOrder.Domain.Entities.Store { Id = Guid.NewGuid(), Name = "Cached Store User", UserId = userId };

        _cacheServiceMock.Setup(c => c.GetAsync<IOrder.Domain.Entities.Store>(key)).ReturnsAsync(cachedStore);

        // Act
        var result = await _cachedRepository.GetByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Cached Store User");
        _decoratedMock.Verify(d => d.GetByUserIdAsync(It.IsAny<string>()), Times.Never);
    }
}
