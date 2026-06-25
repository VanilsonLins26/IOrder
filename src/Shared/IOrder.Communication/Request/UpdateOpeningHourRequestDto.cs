using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Request;

public class UpdateOpeningHourRequestDto
{
    public ICollection<OpeningHourRequestDto> OpeningHours { get; set; } = [];
}
