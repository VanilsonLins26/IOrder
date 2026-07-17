using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Payment;
using Microsoft.EntityFrameworkCore;

namespace IOrder.infrastructure.Repositories.Payment;

internal class PaymentRepository : IPaymentReadOnlyRepository, IPaymentWriteOnlyRepository
{
    private readonly DataAccess.AppDbContext _context;

    public PaymentRepository(DataAccess.AppDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Payment?> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<Domain.Entities.Payment?> GetByStripeIdAsync(string stripePaymentIntentId)
    {
        return await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.StripePaymentIntentId == stripePaymentIntentId);
    }

    public async Task CreateAsync(Domain.Entities.Payment payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    public Domain.Entities.Payment Update(Domain.Entities.Payment payment)
    {
        _context.Payments.Update(payment);
        return payment;
    }

    public async Task<Domain.Entities.Payment?> GetByIdTracking(Guid id)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
    }
}
