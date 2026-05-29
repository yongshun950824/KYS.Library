using KYS.EFCore.Library.Converters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NUnit.Framework;

namespace KYS.EFCore.Library.Tests.ConvertersUnitTests;

public class EnumDescriptionMappingConverterUnitTests
{
    #region Enum
    private enum TestEnum
    {
        [System.ComponentModel.Description("Alpha Description")]
        Alpha,
        [System.ComponentModel.Description("Beta Description")]
        Beta
    }
    #endregion

    [Test]
    public void ConvertToProvider_ShouldReturnDescriptionForEnum()
    {
        // Arrange
        ValueConverter<TestEnum, string> conv = new EnumDescriptionMappingConverter<TestEnum>();
        var toProvider = conv.ConvertToProviderExpression.Compile();

        // Act / Assert
        Assert.AreEqual("Alpha Description", toProvider(TestEnum.Alpha));
        Assert.AreEqual("Beta Description", toProvider(TestEnum.Beta));
    }

    [Test]
    public void ConvertFromProvider_ShouldReturnEnumForDescription()
    {
        // Arrange
        ValueConverter<TestEnum, string> conv = new EnumDescriptionMappingConverter<TestEnum>();
        var fromProvider = conv.ConvertFromProviderExpression.Compile();

        // Act & Assert
        Assert.AreEqual(TestEnum.Alpha, fromProvider("Alpha Description"));
        Assert.AreEqual(TestEnum.Beta, fromProvider("Beta Description"));
    }
}

