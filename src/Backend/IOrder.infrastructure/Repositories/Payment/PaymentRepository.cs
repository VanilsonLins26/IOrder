using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Payment;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace IOrder.infrastructure.Repositories.Payment;

internal class PaymentRepository : IPaymentReadOnlyRepository, IPaymentWriteOnlyRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Payment?> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<Domain.Entities.Payment?> GetByMercadoPagoIdAsync(string mercadoPagoPaymentId)
    {
        return await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.MercadoPagoPaymentId == mercadoPagoPaymentId);
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

    async Task<Domain.Entities.Payment?> IPaymentWriteOnlyRepository.GetByMercadoPagoId(string mercadoPagoPaymentId)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.MercadoPagoPaymentId == mercadoPagoPaymentId);
    }
}
