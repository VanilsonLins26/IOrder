using IOrder.Domain.Services;
using Moq;

namespace CommomTestUtilities.Services;

public class GeocodingServiceBuilder
{
    private readonly Mock<IGeocodingService> _service;

    public GeocodingServiceBuilder()
    {
        _service = new Mock<IGeocodingService>();
    }

    public GeocodingServiceBuilder GetCoordinatesAsync(double lat, double lon)
    {
        _service.Setup(s => s.GetCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((lat, lon));
        return this;
    }

    public IGeocodingService Build()
    {
        return _service.Object;
    }
}
