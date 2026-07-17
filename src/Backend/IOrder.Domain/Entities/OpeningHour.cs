using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Entities;

public record OpeningHour
{
    public DayOfWeek DayOfWeek { get; init; }
    public TimeOnly OpenHour { get; init; }
    public TimeOnly CloseHour { get; init; }
}
