using IOrder.Domain.Services;
using Moq;

namespace CommomTestUtilities.Services;

public class StorageServiceBuilder
{
    private readonly Mock<IStorageService> _mock;

    public StorageServiceBuilder()
    {
        _mock = new Mock<IStorageService>();
    }

    public StorageServiceBuilder UploadImageAsync(string imageUrl)
    {
        _mock.Setup(s => s.UploadImageAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(imageUrl);
        return this;
    }

    public StorageServiceBuilder DeleteImageAsync()
    {
        _mock.Setup(s => s.DeleteImageAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        return this;
    }

    public IStorageService Build() => _mock.Object;
}
