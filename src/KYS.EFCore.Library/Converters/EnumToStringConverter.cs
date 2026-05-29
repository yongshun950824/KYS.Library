using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KYS.EFCore.Library.Converters;

/// <summary>
/// <a href="https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations#the-valueconverter-class">The ValueConverter class</a>
/// </summary>
/// <typeparam name="TEnum"></typeparam>
/// <returns></returns>
public class EnumToStringConverter<TEnum> : ValueConverter<TEnum, string>
    where TEnum : Enum
{
    public EnumToStringConverter()
        : base(
            v => v.ToString(),
            v => (TEnum)Enum.Parse(typeof(TEnum), v)
        )
    {

    }
}
