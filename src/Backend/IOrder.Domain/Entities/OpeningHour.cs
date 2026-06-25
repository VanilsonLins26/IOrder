using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Entities;

public class OpeningHour : EntityBase
{
    public Store? Store { get; set; }
    public Guid StoreId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeOnly OpenHour { get; set; }
    public TimeOnly CloseHour { get; set; }
}
