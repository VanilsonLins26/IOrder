namespace IOrder.Domain.Entities;

public record Address
{
    public string ZipCode { get; init; } = string.Empty;     
    public string Street { get; init; } = string.Empty;     
    public string Number { get; init; } = string.Empty;      
    public string Complement { get; init; } = string.Empty;   
    public string Neighborhood { get; init; } = string.Empty; 
    public string City { get; init; } = string.Empty;         
    public string State { get; init; } = string.Empty;        

}
