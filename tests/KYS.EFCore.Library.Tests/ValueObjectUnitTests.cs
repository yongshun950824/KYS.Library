using KYS.EFCore.Library.Tests.Domain.Entities;
using NUnit.Framework;

namespace KYS.EFCore.Library.Tests;

public class ValueObjectUnitTests
{
    [Test]
    public void Equals_SameValues_ShouldBeEqual()
    {
        // Arrange
        var address1 = new Address("123 Main St", "Anytown");
        var address2 = new Address("123 Main St", "Anytown");

        // Act & Assert
        Assert.IsTrue(address1.Equals(address2));
    }

    [Test]
    public void Equals_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var address1 = new Address("123 Main St", "Anytown");
        var address2 = new Address("456 Oak Ave", "Sometown");

        // Act & Assert
        Assert.IsFalse(address1.Equals(address2));
    }

    [Test]
    public void Equals_WithNull_ShouldNotBeEqual()
    {
        // Arrange
        var address1 = new Address("123 Main St", "Anytown");
        Address? address2 = null;

        // Act & Assert
        Assert.IsFalse(address1.Equals(address2));
    }

    [Test]
    public void Equals_WithNonAddress_ShouldNotBeEqual()
    {
        // Arrange
        var address1 = new Address("123 Main St", "Anytown");
        object value = new { AddressLine1 = "123 Main St", City = "Anytown" };

        // Act & Assert
        Assert.IsFalse(address1.Equals(value));
    }

    [Test]
    public void GetHashCode_SameValues_ShouldHaveSameHashCode()
    {
        // Arrange
        var address1 = new Address("123 Main St", "Saint Mary Garden", "Anytown");
        var address2 = new Address("123 Main St", "Saint Mary Garden", "Anytown");

        // Act
        var hashCode1 = address1.GetHashCode();
        var hashCode2 = address2.GetHashCode();

        // Assert
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [Test]
    public void GetHashCode_ContainingNullProperty_ShouldReturnHashCode()
    {
        // Arrange
        var address1 = new Address("123 Main St", "Anytown");

        // Act & Assert
        Assert.DoesNotThrow(() => address1.GetHashCode());
    }

    [Test]
    public void RelationalOperator_Equal_SameValues_ShouldBeEqual()
    {
        // Arrange
        var address1 = new Address("123 Main St", "Anytown");
        var address2 = new Address("123 Main St", "Anytown");

        // Act & Assert
        Assert.IsTrue(address1 == address2);
    }

    [Test]
    public void RelationalOperators_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var address1 = new Address("123 Main St", "Anytown");
        var address2 = new Address("456 Oak Ave", "Sometown");

        // Act & Assert
        Assert.IsTrue(address1 != address2);
    }

    [Test]
    public void RelationalOperator_Equal_WithNull_ShouldNotBeEqual()
    {
        // Arrange
        var address1 = new Address("123 Main St", "Anytown");
        Address? address2 = null;

        // Act & Assert
        Assert.IsFalse(address1 == address2);
    }

    [Test]
    public void RelationalOperator_Equal_WithBothNull_ShouldBeEqual()
    {
        // Arrange
        Address? address1 = null;
        Address? address2 = null;

        // Act & Assert
        Assert.IsTrue(address1 == address2);
        Assert.IsFalse(address1 != address2);
    }

    [Test]
    public void RelationalOperator_Equal_WithSameReference_ShouldBeEqual()
    {
        // Arrange
        Address address1 = new("123 Main St", "Anytown");
        Address address2 = address1;

        // Act & Assert
        Assert.IsTrue(address1 == address2);
    }

    [Test]
    public void RelationalOperator_NotEqual_WithBothNull_ShouldBeEqual()
    {
        // Arrange
        Address? address1 = null;
        Address? address2 = null;

        // Act & Assert
        Assert.IsFalse(address1 != address2);
    }
}