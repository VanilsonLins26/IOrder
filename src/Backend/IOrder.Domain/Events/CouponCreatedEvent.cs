using IOrder.Domain.Entities.Enums;
using IOrder.Domain.SeedWork;
using System;

namespace IOrder.Domain.Events;

public class CouponCreatedEvent : IDomainEvent
{
    public Guid CouponId { get; }
    public string Code { get; }
    public CouponDiscountType DiscountType { get; }
    public decimal DiscountValue { get; }
    public DateTime OccurredOn { get; }

    public CouponCreatedEvent(Guid couponId, string code, CouponDiscountType discountType, decimal discountValue)
    {
        CouponId = couponId;
        Code = code;
        DiscountType = discountType;
        DiscountValue = discountValue;
        OccurredOn = DateTime.UtcNow;
    }
}
