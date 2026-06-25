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
    public int CategoryId { get; set; }
    public String UserId { get; set; } = string.Empty;
    //public Category? Category { get; set; }

    public bool IsOpen()
    {
        var currentDay = (int)DateTime.UtcNow.DayOfWeek;
        var previousDay = currentDay == 0 ? 6 : currentDay - 1;
        var currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);

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

            var yesterdayHours = OpeningHours.FirstOrDefault(oh => oh.DayOfWeek == previousDay);
            if (yesterdayHours != null)
            {
                isOvernight = yesterdayHours.OpenHour > yesterdayHours.CloseHour;

                if (isOvernight)
                {

                    if (currentTime <= yesterdayHours.CloseHour) return true;
                }
            }

        }
        return false;
    }
  
}
