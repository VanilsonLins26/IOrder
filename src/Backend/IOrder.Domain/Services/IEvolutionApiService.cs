namespace IOrder.Domain.Services;

public interface IEvolutionApiService
{
    Task SendTextAsync(string phoneNumber, string message);
}
