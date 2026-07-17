using Bogus;
using IOrder.Communication.Request;
using System;
using System.Collections.Generic;

namespace CommomTestUtilities.Requests.Store;

public class OpeningHourRequestBuilder
{
    public static OpeningHourRequestDto Build()
    {
        return new Faker<OpeningHourRequestDto>("pt_BR")
            .RuleFor(o => o.DayOfWeek, f => f.PickRandom<DayOfWeek>())
            .RuleFor(o => o.OpenHour, f => new TimeOnly(f.Random.Int(6, 12), 0))
            .RuleFor(o => o.CloseHour, f => new TimeOnly(f.Random.Int(18, 23), 0));
    }

    public static IList<OpeningHourRequestDto> BuildList(int count = 3)
    {
        return new Faker<OpeningHourRequestDto>("pt_BR")
            .RuleFor(o => o.DayOfWeek, f => f.PickRandom<DayOfWeek>())
            .RuleFor(o => o.OpenHour, f => new TimeOnly(f.Random.Int(6, 12), 0))
            .RuleFor(o => o.CloseHour, f => new TimeOnly(f.Random.Int(18, 23), 0))
            .Generate(count);
    }
}
