using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Linq;

using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class Store : EntityBase, IAggregateRoot
{
    public string? Name { get; set; }
    public Address? Address { get; set; }
    public string? About { get; set; }
    public string ImageUrl { get; set; }

    private readonly List<OpeningHour> _openingHours = [];
    public IReadOnlyCollection<OpeningHour> OpeningHours => _openingHours.AsReadOnly();
    public Guid CategoryId { get; set; }
    public String UserId { get; set; } = string.Empty;
    public string? OwnerEmail { get; set; }
    public string? OwnerPhone { get; set; }
    public StoreCategory? Category { get; set; }

    // Delivery & Location Fields
    public decimal BaseDeliveryFee { get; set; } = 5.0m;
    public decimal FeePerKm { get; set; } = 1.5m;
    public double MaxDeliveryDistanceKm { get; set; } = 15.0;
    public NetTopologySuite.Geometries.Point? Location { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public double? DistanceKm { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public decimal? DeliveryFee { get; set; }

    public bool IsOpen()
    {
        var brazilTime = DateTime.UtcNow.AddHours(-3);
        var currentDay = brazilTime.DayOfWeek;
        var previousDay = currentDay == DayOfWeek.Sunday ? DayOfWeek.Saturday : currentDay - 1;
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

    public static Expression<Func<Store, bool>> IsOpenExpression()
    {
        var brazilTime = DateTime.UtcNow.AddHours(-3);
        var currentDay = brazilTime.DayOfWeek;
        var previousDay = currentDay == DayOfWeek.Sunday ? DayOfWeek.Saturday : currentDay - 1;
        var currentTime = TimeOnly.FromDateTime(brazilTime);

        return s => s.OpeningHours.Any(oh =>
            (oh.DayOfWeek == currentDay && oh.OpenHour <= oh.CloseHour && currentTime >= oh.OpenHour && currentTime <= oh.CloseHour)
            ||
            (oh.DayOfWeek == currentDay && oh.OpenHour > oh.CloseHour && currentTime >= oh.OpenHour)
            ||
            (oh.DayOfWeek == previousDay && oh.OpenHour > oh.CloseHour && currentTime <= oh.CloseHour)
        );
    }
    public void UpdateOpeningHours(IEnumerable<OpeningHour> newHours)
    {
        _openingHours.Clear();
        _openingHours.AddRange(newHours);
    }
}
