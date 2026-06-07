namespace KYS.EFCore.Library.Tests.Domain.Entities;

public class Address : ValueObject, IEquatable<Address>
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
        yield return AddressLine2 ?? String.Empty;
        yield return City;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Address);
    }

    public bool Equals(Address? other)
    {
        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
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
