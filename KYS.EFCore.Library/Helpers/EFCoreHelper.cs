using KYS.Library.Extensions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace KYS.EFCore.Library.Helpers
{
    public static class EFCoreHelper
    {
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
        public static ValueConverter<TEnum, string> EnumDescriptionMappingConverter<TEnum>()
            where TEnum : Enum
        {
            return new ValueConverter<TEnum, string>
            (
                v => v.ToDescription(),
                v => EnumExtensions.GetValueByDescription<TEnum>(v)
            );
        }

        /// <summary>
        /// <a href="https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations#the-valueconverter-class">The ValueConverter class</a>
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static ValueConverter<TEnum, string> EnumToStringConverter<TEnum>()
            where TEnum : Enum
        {
            return new ValueConverter<TEnum, string>
            (
                v => v.ToDescription(),
                v => (TEnum)Enum.Parse(typeof(TEnum), v)
            );
        }
    }
}
