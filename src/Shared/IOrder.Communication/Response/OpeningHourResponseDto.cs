using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Response;

public class OpeningHourResponseDto
{
    public Guid StoreId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpenHour { get; set; }
    public TimeOnly CloseHour { get; set; }
}
