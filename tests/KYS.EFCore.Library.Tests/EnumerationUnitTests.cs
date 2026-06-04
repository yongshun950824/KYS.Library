using KYS.EFCore.Library.Tests.Domain.Entities;
using NUnit.Framework;

namespace KYS.EFCore.Library.Tests;

public class EnumerationUnitTests
{
    [Test]
    public void GetAll_ShouldReturnAllDeclaredInstances()
    {
        // Act
        var colors = Enumeration.GetAll<Color>()
            .ToList();

        // Assert
        Assert.IsNotNull(colors);
        Assert.AreEqual(3, colors.Count);
        Assert.Contains(Color.Red, colors);
        Assert.Contains(Color.Green, colors);
        Assert.Contains(Color.Blue, colors);
    }

    [Test]
    public void ToString_ShouldReturnName()
    {
        // Act
        var colorName = Color.Red.ToString();

        // Assert
        Assert.AreEqual("Red", colorName);
    }

    [Test]
    public void Equals_SameTypeAndId_ShouldBeEqual()
    {
        // Arrange
        var color1 = Color.Red;
        var color2 = Color.Red;

        // Act & Assert
        Assert.IsTrue(color1.Equals(color2));
        Assert.IsTrue(color1 == color2);
    }

    [Test]
    public void Equals_WithNonEnumeration_ShouldReturnFalse()
    {
        // Arrange
        var color = Color.Red;
        var nonEnumeration = new object();

        // Act & Assert
        Assert.IsFalse(color.Equals(nonEnumeration));
    }

    [Test]
    public void Equals_WithEnumerationAsObject_ShouldReturnTrue()
    {
        // Arrange
        var color = Color.Red;
        var red = (object)Color.Red;

        // Act & Assert
        Assert.IsTrue(color.Equals(red));
    }

    [Test]
    public void IEqualityComparer_Equals_WithRightNull_ShouldReturnFalse()
    {
        // Arrange
        IEqualityComparer<Enumeration> comparer = Color.Red;

        // Act & Assert
        Assert.IsFalse(comparer.Equals(Color.Red, null));
    }

    [Test]
    public void IEqualityComparer_Equals_WithDifferentInstance_ShouldReturnFalse()
    {
        // Arrange
        IEqualityComparer<Enumeration> comparer = Color.Red;

        // Act & Assert
        Assert.IsFalse(comparer.Equals(Color.Red, Color.Green));
    }

    [Test]
    public void IEqualityComparer_Equals_WithSameTypeAndId_ShouldReturnTrue()
    {
        // Arrange
        IEqualityComparer<Enumeration> comparer = Color.Red;

        // Act & Assert
        Assert.IsTrue(comparer.Equals(Color.Red, Color.Red));
    }

    [Test]
    public void GetHashCode_ShouldReturnSameHashForSameId()
    {
        // Arrange
        var color1 = Color.Red;
        var color2 = Color.Red;

        // Act
        var hash1 = color1.GetHashCode();
        var hash2 = color2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [Test]
    public void CompareTo_ShouldReturnZeroForSameId()
    {
        // Arrange
        var color1 = Color.Red;
        var color2 = Color.Red;

        // Act
        var result = color1.CompareTo(color2);

        // Assert
        Assert.AreEqual(0, result);
    }

    [Test]
    public void RelationalOperators_ShouldCompareCorrectly()
    {
        // Arrange
        Color red = Color.Red;
        Color redTwo = Color.Red;
        Color green = Color.Green;
        Color greenTwo = Color.Green;

        // Act & Assert
        Assert.IsTrue(red < green);
        Assert.IsTrue(green > red);
        Assert.IsTrue(red <= redTwo);
        Assert.IsTrue(green >= greenTwo);
        Assert.IsTrue(red != green);
    }

    [Test]
    public void RelationalOperators_WithLeftNull_ShouldCompareCorrectly()
    {
        // Arrange
        Color? nullableColor = null;
        Color red = Color.Red;
        Color green = Color.Green;

        // Act & Assert
        Assert.IsTrue(nullableColor < green);
        Assert.IsFalse(nullableColor > green);
        Assert.IsTrue(nullableColor <= red);
        Assert.IsFalse(nullableColor >= green);
        Assert.IsTrue(nullableColor != green);
    }

    [Test]
    public void RelationalOperators_WithRightNull_ShouldCompareCorrectly()
    {
        // Arrange
        Color? nullableColor = null;
        Color red = Color.Red;
        Color green = Color.Green;

        // Act & Assert
        Assert.IsFalse(green < nullableColor);
        Assert.IsTrue(green > nullableColor);
        Assert.IsFalse(red <= nullableColor);
        Assert.IsTrue(green >= nullableColor);
        Assert.IsTrue(green != nullableColor);
    }

    [Test]
    public void RelationalOperators_WithBothNull_ShouldCompareCorrectly()
    {
        // Arrange
        Color? nullableColor = null;
        Color? nullableColorTwo = null;

        // Act & Assert
        Assert.IsFalse(nullableColor < nullableColorTwo);
        Assert.IsFalse(nullableColor > nullableColorTwo);
        Assert.IsTrue(nullableColor <= nullableColorTwo);
        Assert.IsTrue(nullableColor >= nullableColorTwo);
        Assert.IsFalse(nullableColor != nullableColorTwo);
    }
}
