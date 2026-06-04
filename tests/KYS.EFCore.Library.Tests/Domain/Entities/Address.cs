namespace KYS.EFCore.Library.Tests.Domain.Entities;

public class Address : ValueObject
{
    public string AddressLine1 { get; }
    public string? AddressLine2 { get; }
    public string City { get; }

    public Address(string addressLine1, string city)
    {
        AddressLine1 = addressLine1;
        City = city;
    }

    public Address(string addressLine1, string addressLine2, string city)
    {
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AddressLine1;
        yield return AddressLine2;
        yield return City;
    }

    #region Relational operators
    public static bool operator ==(Address? left, Address? right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(Address? left, Address? right)
    {
        return NotEqualOperator(left, right);
    }
    #endregion
}
