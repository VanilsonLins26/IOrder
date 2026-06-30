using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace IOrder.Domain.Entities;

public class Store : EntityBase
{
    public string? Name { get; set; }
    public Address? Address { get; set; }
    public string? About { get; set; }
    public string ImageUrl { get; set; }
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<OpeningHour> OpeningHours { get; set; } = [];
    public Guid CategoryId { get; set; }
    public String UserId { get; set; } = string.Empty;
    public StoreCategory? Category { get; set; }

    public bool IsOpen()
    {
        var brazilTime = DateTime.UtcNow.AddHours(-3);
        var currentDay = (int)brazilTime.DayOfWeek;
        var previousDay = currentDay == 0 ? 6 : currentDay - 1;
        var currentTime = TimeOnly.FromDateTime(brazilTime);

        var yesterdayHours = OpeningHours.FirstOrDefault(oh => oh.DayOfWeek == previousDay);
        if (yesterdayHours != null)
        {
            bool isOvernight = yesterdayHours.OpenHour > yesterdayHours.CloseHour;
            if (isOvernight && currentTime <= yesterdayHours.CloseHour)
            {
                return true;
            }
        }

        var todayHours = OpeningHours.FirstOrDefault(oh => oh.DayOfWeek == currentDay);
        if (todayHours != null)
        {
            bool isOvernight = todayHours.OpenHour > todayHours.CloseHour;
            if (isOvernight)
            {
                if (currentTime >= todayHours.OpenHour) return true;
            }
            else
            {
                if (currentTime >= todayHours.OpenHour && currentTime <= todayHours.CloseHour) return true;
            }
        }

        return false;
    }
  
}
