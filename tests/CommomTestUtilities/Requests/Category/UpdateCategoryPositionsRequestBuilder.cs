using Bogus;
using IOrder.Communication.Request;
using System;
using System.Collections.Generic;

namespace CommomTestUtilities.Requests.Category;

public class UpdateCategoryPositionsRequestBuilder
{
    public static UpdateCategoryPositionsRequestDto Build(int count = 2)
    {
        var faker = new Faker();
        var positions = new List<CategoryPositionDto>();

        for (int i = 0; i < count; i++)
        {
            positions.Add(new CategoryPositionDto
            {
                CategoryId = Guid.NewGuid(),
                Position = faker.Random.Int(0, 10)
            });
        }

        return new UpdateCategoryPositionsRequestDto
        {
            Positions = positions
        };
    }
}
