using System;
using System.Threading.Tasks;
using FluentAssertions;
using IOrder.Application.Services.Cache;
using IOrder.Domain.Repositories.Store;
using IOrder.infrastructure.Repositories.Store;
using Moq;
using Xunit;

namespace UseCases.Test.Infrastructure.Cache;

public class CachedStoreWriteOnlyRepositoryTest
{
    private readonly Mock<IStoreWriteOnlyRepository> _decoratedMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly CachedStoreWriteOnlyRepository _cachedRepository;

    public CachedStoreWriteOnlyRepositoryTest()
    {
        _decoratedMock = new Mock<IStoreWriteOnlyRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _cachedRepository = new CachedStoreWriteOnlyRepository(_decoratedMock.Object, _cacheServiceMock.Object);
    }

    [Fact]
    public async Task Create_ShouldCallDecorated_AndInvalidateCache()
    {
        // Arrange
        var store = new IOrder.Domain.Entities.Store { Id = Guid.NewGuid(), Name = "New Store", UserId = "user1" };
        _decoratedMock.Setup(d => d.Create(store)).ReturnsAsync(store);

        // Act
        var result = await _cachedRepository.Create(store);

        // Assert
        result.Should().Be(store);
        _decoratedMock.Verify(d => d.Create(store), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync(CacheKeys.StoreById(store.Id)), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync(CacheKeys.StoreByUserId(store.UserId)), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync(CacheKeys.DashboardMetrics(store.Id)), Times.Once);
    }

    [Fact]
    public void Update_ShouldCallDecorated_AndInvalidateCache()
    {
        // Arrange
        var store = new IOrder.Domain.Entities.Store { Id = Guid.NewGuid(), Name = "Updated Store", UserId = "user2" };

        // Act
        _cachedRepository.Update(store);

        // Assert
        _decoratedMock.Verify(d => d.Update(store), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync(CacheKeys.StoreById(store.Id)), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync(CacheKeys.StoreByUserId(store.UserId)), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync(CacheKeys.DashboardMetrics(store.Id)), Times.Once);
    }

    [Fact]
    public void Delete_ShouldCallDecorated_AndInvalidateCache()
    {
        // Arrange
        var store = new IOrder.Domain.Entities.Store { Id = Guid.NewGuid(), Name = "Deleted Store", UserId = "user3" };
        _decoratedMock.Setup(d => d.Delete(store)).Returns(store);

        // Act
        var result = _cachedRepository.Delete(store);

        // Assert
        result.Should().Be(store);
        _decoratedMock.Verify(d => d.Delete(store), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync(CacheKeys.StoreById(store.Id)), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync(CacheKeys.StoreByUserId(store.UserId)), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync(CacheKeys.DashboardMetrics(store.Id)), Times.Once);
    }
}
