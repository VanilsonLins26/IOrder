using IOrder.Communication.Request;
using System;
using System.Collections.Generic;

namespace CommomTestUtilities.Requests.Category;

public class AddProductsToCategoryRequestBuilder
{
    public static AddProductsToCategoryRequestDto Build(List<Guid> productIds)
    {
        return new AddProductsToCategoryRequestDto
        {
            ProductIds = productIds
        };
    }
}
