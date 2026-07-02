using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Request;

public class OpeningHourRequestDto
{
    public DayOfWeek? DayOfWeek { get; set; }
    public TimeOnly OpenHour { get; set; }
    public TimeOnly CloseHour { get; set; }
}
