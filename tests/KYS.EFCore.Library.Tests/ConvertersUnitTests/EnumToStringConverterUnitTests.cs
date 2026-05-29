using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NUnit.Framework;

namespace KYS.EFCore.Library.Tests.ConvertersUnitTests;

public class EnumToStringConverterUnitTests
{
    #region Enum
    private enum TestEnum
    {
        Alpha,
        Beta
    }
    #endregion

    [Test]
    public void ConvertToProvider_ShouldReturnDescriptionForEnum()
    {
        // Arrange
        ValueConverter<TestEnum, string> conv = new EnumToStringConverter<TestEnum>();
        var toProvider = conv.ConvertToProviderExpression.Compile();

        // Act / Assert
        Assert.AreEqual(TestEnum.Alpha.ToString(), toProvider(TestEnum.Alpha));
        Assert.AreEqual(TestEnum.Beta.ToString(), toProvider(TestEnum.Beta));
    }

    [Test]
    public void ConvertFromProvider_ShouldReturnEnumForDescription()
    {
        // Arrange
        ValueConverter<TestEnum, string> conv = new EnumToStringConverter<TestEnum>();
        var fromProvider = conv.ConvertFromProviderExpression.Compile();

        // Act & Assert
        Assert.AreEqual(TestEnum.Alpha, fromProvider(TestEnum.Alpha.ToString()));
        Assert.AreEqual(TestEnum.Beta, fromProvider(TestEnum.Beta.ToString()));
    }
}
