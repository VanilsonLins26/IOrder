using Bogus;
using IOrder.Communication.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommomTestUtilities.Requests.Store;

public class AddressRequestBuilder
{
    public static AddressRequestDto Build()
    {
        return new Faker<AddressRequestDto>("pt_BR")
            .RuleFor(address => address.ZipCode, f => f.Address.ZipCode())
            .RuleFor(address => address.Street, f => f.Address.StreetName())
            .RuleFor(address => address.Number, f => f.Address.BuildingNumber())
            .RuleFor(address => address.Complement, f => f.Address.SecondaryAddress())
            .RuleFor(address => address.Neighborhood, f => f.Address.County())
            .RuleFor(address => address.City, f => f.Address.City())
            .RuleFor(address => address.State, f => f.Address.StateAbbr());
    }
}
