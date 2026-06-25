using Microsoft.EntityFrameworkCore;

namespace IOrder.Domain.Entities;

[Owned]
public class Address
{
    public string ZipCode { get; set; } = string.Empty;     
    public string Street { get; set; } = string.Empty;     
    public string Number { get; set; } = string.Empty;      
    public string Complement { get; set; } = string.Empty;   
    public string Neighborhood { get; set; } = string.Empty; 
    public string City { get; set; } = string.Empty;         
    public string State { get; set; } = string.Empty;        

}
