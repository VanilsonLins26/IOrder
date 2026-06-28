using Bogus;
using IOrder.Communication.Request;
using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace CommomTestUtilities.Requests.Store;

public class StoreRequestBuilder
{
    public static StoreRequestDto Build()
    {
        var store = new Faker<StoreRequestDto>("pt_BR")
            .RuleFor(store => store.Name, f => f.Company.CompanyName())
            .RuleFor(store => store.About, f => f.Lorem.Paragraph())
            .RuleFor(store => store.ImageUrl, f => f.Image.PicsumUrl())
            .RuleFor(store => store.CategoryId, f => f.Random.Guid())
            .RuleFor(store => store.Address, f => AddressRequestBuilder.Build())
            .RuleFor(store => store.OpeningHours, f => OpeningHourRequestBuilder.BuildList(3))
            .Generate();

        var hours = store.OpeningHours.ToList();
        for (int i = 0; i < hours.Count; i++)
            hours[i].DayOfWeek = i;
        store.OpeningHours = hours;

        return store;
    }
}
