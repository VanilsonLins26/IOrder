using Bogus;
using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using System;
using System.Collections.Generic;

namespace CommomTestUtilities.Entities;

public class DeliveryAssignmentBuilder
{
    public static DeliveryAssignment Build(string? courierUserId = null, AssignmentStatus status = AssignmentStatus.Pending, Guid? orderId = null)
    {
        var assignment = new Faker<DeliveryAssignment>()
            .CustomInstantiator(f => new DeliveryAssignment())
            .RuleFor(d => d.Id, f => f.Random.Guid())
            .RuleFor(d => d.OrderId, f => orderId ?? f.Random.Guid())
            .RuleFor(d => d.CourierUserId, f => courierUserId ?? f.Internet.UserName())
            .RuleFor(d => d.AssignedAt, f => f.Date.Recent())
            .RuleFor(d => d.CourierNotes, f => f.Lorem.Sentence())
            .Generate();
        
        if (status != AssignmentStatus.Pending)
        {
            var prop = typeof(DeliveryAssignment).GetProperty("Status");
            prop?.SetValue(assignment, status);
        }

        return assignment;
    }

    public static List<DeliveryAssignment> BuildList(int count, string? courierUserId = null, AssignmentStatus status = AssignmentStatus.Pending, Guid? orderId = null)
    {
        var list = new List<DeliveryAssignment>();
        for (int i = 0; i < count; i++)
        {
            list.Add(Build(courierUserId, status, orderId));
        }
        return list;
    }
}
