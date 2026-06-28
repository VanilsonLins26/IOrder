using Bogus;
using IOrder.Communication.Request;

using System.Linq;

namespace CommomTestUtilities.Requests.Store;

public class UpdateOpeningHourRequestBuilder
{
    public static UpdateOpeningHourRequestDto Build()
    {
        var request = new Faker<UpdateOpeningHourRequestDto>("pt_BR")
            .RuleFor(o => o.OpeningHours, f => OpeningHourRequestBuilder.BuildList(3))
            .Generate();

        var hours = request.OpeningHours.ToList();
        for (int i = 0; i < hours.Count; i++)
            hours[i].DayOfWeek = i;
        request.OpeningHours = hours;

        return request;
    }
}
