using System;
using KYS.Library.Extensions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KYS.EFCore.Library.Converters;

/// <summary>
/// Store/map Enum with value from <c>Description</c> attribute in EF Core. <br /><br />
/// References: <br />
/// <list type="bullet">
///     <item><a href="https://stackoverflow.com/q/78278757">Stack Overflow question</a></item>
///     <item><a href="https://dotnetfiddle.net/xJbloZ">.NET Fiddle Demo</a></item>
/// </list>
/// </summary>
/// <typeparam name="TEnum"></typeparam>
/// <returns></returns>
public class EnumDescriptionMappingConverter<TEnum> : ValueConverter<TEnum, string>
    where TEnum : Enum
{
    public EnumDescriptionMappingConverter()
        : base(
            v => v.ToDescription(),
            v => EnumExtensions.GetValueByDescription<TEnum>(v)
        )
    {

    }
}
