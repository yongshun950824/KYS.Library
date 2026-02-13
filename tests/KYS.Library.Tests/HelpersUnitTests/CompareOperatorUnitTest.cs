using NUnit.Framework;
using System;
using System.ComponentModel;
using static KYS.Library.Helpers.CompareOperator;

namespace KYS.Library.Tests.HelpersUnitTests;

public class CompareOperatorUnitTest
{
    [TestCase(CompareOperatorConstants.Equal, 1, 1, true)]
    [TestCase(CompareOperatorConstants.Equal, 0, 1, false)]
    [TestCase(CompareOperatorConstants.NotEqual, 0, 1, true)]
    [TestCase(CompareOperatorConstants.NotEqual, 1, 1, false)]
    [TestCase(CompareOperatorConstants.LessThan, 0, 1, true)]
    [TestCase(CompareOperatorConstants.LessThan, 1, 1, false)]
    [TestCase(CompareOperatorConstants.LessThan, 1, 0, false)]
    [TestCase(CompareOperatorConstants.LessThanOrEqual, 0, 1, true)]
    [TestCase(CompareOperatorConstants.LessThanOrEqual, 1, 1, true)]
    [TestCase(CompareOperatorConstants.LessThanOrEqual, 1, 0, false)]
    [TestCase(CompareOperatorConstants.GreaterThan, 1, 0, true)]
    [TestCase(CompareOperatorConstants.GreaterThan, 1, 1, false)]
    [TestCase(CompareOperatorConstants.GreaterThan, 0, 1, false)]
    [TestCase(CompareOperatorConstants.GreaterThanOrEqual, 1, 0, true)]
    [TestCase(CompareOperatorConstants.GreaterThanOrEqual, 1, 1, true)]
    [TestCase(CompareOperatorConstants.GreaterThanOrEqual, 0, 1, false)]
    public void Compare_Generic_ShouldReturnCorrectResult(CompareOperatorConstants @operator,
        int a,
        int b,
        bool expectedResult)
    {
        // Act
        bool actualResult = Compare(@operator, a, b);

        // Assert
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void Compare_GenericAndWithInvalidOperator_ShouldThrowException()
    {
        // Arrange
        CompareOperatorConstants @operator = (CompareOperatorConstants)99;
        int a = 0;
        int b = 1;

        // Act & Assert
        Assert.Throws<InvalidEnumArgumentException>(() => Compare(@operator, a, b));
    }

    [TestCase(CompareOperatorConstants.Equal, 1, 1, true)]
    [TestCase(CompareOperatorConstants.Equal, 0, 1, false)]
    [TestCase(CompareOperatorConstants.NotEqual, 0, 1, true)]
    [TestCase(CompareOperatorConstants.NotEqual, 1, 1, false)]
    [TestCase(CompareOperatorConstants.LessThan, 0, 1, true)]
    [TestCase(CompareOperatorConstants.LessThan, 1, 1, false)]
    [TestCase(CompareOperatorConstants.LessThan, 1, 0, false)]
    [TestCase(CompareOperatorConstants.LessThanOrEqual, 0, 1, true)]
    [TestCase(CompareOperatorConstants.LessThanOrEqual, 1, 1, true)]
    [TestCase(CompareOperatorConstants.LessThanOrEqual, 1, 0, false)]
    [TestCase(CompareOperatorConstants.GreaterThan, 1, 0, true)]
    [TestCase(CompareOperatorConstants.GreaterThan, 1, 1, false)]
    [TestCase(CompareOperatorConstants.GreaterThan, 0, 1, false)]
    [TestCase(CompareOperatorConstants.GreaterThanOrEqual, 1, 0, true)]
    [TestCase(CompareOperatorConstants.GreaterThanOrEqual, 1, 1, true)]
    [TestCase(CompareOperatorConstants.GreaterThanOrEqual, 0, 1, false)]
    public void Compare_NonGeneric_ShouldReturnCorrectResult(CompareOperatorConstants @operator,
        int a,
        int b,
        bool expectedResult)
    {
        // Act
        bool actualResult = Compare(@operator, (IComparable)a, (IComparable)b);

        // Assert
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void Compare_NonGenericAndWithInvalidOperator_ShouldThrowException()
    {
        // Arrange
        CompareOperatorConstants @operator = (CompareOperatorConstants)99;
        int a = 0;
        int b = 1;

        // Act & Assert
        Assert.Throws<InvalidEnumArgumentException>(() => Compare(@operator, (IComparable)a, (IComparable)b));
    }
}
