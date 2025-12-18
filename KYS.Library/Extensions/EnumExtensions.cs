using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension and helper methods for working with <see cref="Enum"/>.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Obtain all <c>TEnum</c> member(s). 
        /// </summary>
        /// <typeparam name="TEnum">The type inherits <see cref="Enum"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{TEnum}"/> that contains all the <c>TEnum</c> member(s).</returns>
        public static IEnumerable<TEnum> GetEnumValues<TEnum>()
            where TEnum : Enum
        {
            TEnum valueType = default(TEnum);

            return typeof(TEnum).GetFields()
                .Where(fieldInfo => fieldInfo.IsLiteral)
                .Select(fieldInfo => (TEnum)fieldInfo.GetValue(valueType))
                .Distinct();
        }

        /// <summary>
        /// Obtain all the name(s) from the <c>TEnum</c> member(s). 
        /// </summary>
        /// <typeparam name="TEnum">The type inherits <see cref="Enum"/>.</typeparam>
        /// <returns>An <see cref="IEnumerable{string}"/> that contains all the names from the <c>TEnum</c> member(s).</returns>
        public static IEnumerable<string> GetEnumNames<TEnum>()
            where TEnum : Enum
        {
            TEnum valueType = default(TEnum);

            return typeof(TEnum).GetFields()
                .Where(fieldInfo => fieldInfo.IsLiteral)
                .Select(fieldInfo => ToDescription((Enum)fieldInfo.GetValue(valueType)))
                .Distinct();
        }

        /// <summary>
        /// Obtain value from the <see cref="DescriptionAttribute" /> for the <see cref="Enum" /> member.
        /// <br />
        /// If the <see cref="Enum" /> member doesn't apply <see cref="DescriptionAttribute" />, returns the <see cref="Enum" /> value.
        /// </summary>
        /// <param name="value">The <see cref="Enum" /> instance this method extends.</param>
        /// <returns>Value from the <see cref="DescriptionAttribute" /> or its value for <see cref="Enum" /> member.</returns>
        public static string ToDescription(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            if (attribute == null)
                return value.ToString();

            return attribute.Description;
        }

        /// <summary>
        /// Obtain key-value pair with key: <c>TEnum</c> member and value: description of the <c>TEnum</c> member.
        /// </summary>
        /// <typeparam name="TEnum">The type inherits <see cref="Enum"/>.</typeparam>
        /// <returns>Key-value pair with key: <c>TEnum</c> member and value: description of the <c>TEnum</c> member.</returns>
        public static Dictionary<TEnum, string> ToDictionary<TEnum>()
            where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                 .Cast<TEnum>()
                 .ToDictionary(k => k, v => v.ToDescription());
        }

        /// <summary>
        /// Get value from the <see cref="EnumMemberAttribute" />.
        /// <br />
        /// If the <see cref="Enum" /> member doesn't apply <see cref="EnumMemberAttribute" />, returns the <see cref="Enum" /> value.
        /// </summary>
        /// <typeparam name="TEnum">The type inherits <see cref="Enum" />.</typeparam>
        /// <param name="enum">The <c>TEnum</c> instance this method extends.</param>
        /// <returns>Value from the <see cref="EnumMemberAttribute" /> or its value for the <c>TEnum</c> member.</returns>
        public static string GetEnumMemberValue<TEnum>(this TEnum @enum)
            where TEnum : struct, Enum
        {
            string enumMemberValue = @enum.GetType()
                .GetMember(@enum.ToString())
                .FirstOrDefault()?
                .GetCustomAttributes<EnumMemberAttribute>(false)
                .FirstOrDefault()?
                .Value;

            if (enumMemberValue == null)
                return @enum.ToString();

            return enumMemberValue;
        }

        /// <summary>
        /// Obtain the <c>TEnum</c> member by matching the value in the <see cref="EnumMemberAttribute" />.
        /// <br />
        /// If the <c>TEnum</c> member doesn't apply <see cref="EnumMemberAttribute" />, matching by its name.
        /// </summary>
        /// <typeparam name="TEnum">The type inherits <see cref="Enum" />.</typeparam>
        /// <param name="value">The <c>value</c> that is either from <see cref="EnumMemberAttribute" /> or <c>TEnum</c> member's name.</param>
        /// <returns>Matched <c>TEnum</c> member.</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public static TEnum EnumMemberValueToEnum<TEnum>(this string value)
            where TEnum : struct, Enum
        {
            foreach (var field in typeof(TEnum).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute))
                    is EnumMemberAttribute attribute && attribute.Value == value)
                {
                    return (TEnum)field.GetValue(null);
                }

                if (field.Name == value)
                    return (TEnum)field.GetValue(null);
            }

            throw new KeyNotFoundException($"{value} is not found.");
        }

        /// <summary>
        /// Get <c>TEnum</c> member by description.
        /// </summary>
        /// <typeparam name="TEnum">The type inherits <see cref="Enum" />.</typeparam>
        /// <param name="value">The <c>value</c> that is either from <see cref="DescriptionAttribute" /> or <c>TEnum</c> member's name.</param>
        /// <returns>Matched <c>TEnum</c> member.</returns>
        public static TEnum GetValueByDescription<TEnum>(string value)
            where TEnum : Enum
        {
            var enumDict = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .ToDictionary(k => k, v => v.ToDescription());

            return enumDict
                .Single(x => x.Value == value)
                .Key;
        }

        #region Private Methods
        private static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            Type type = value.GetType();
            MemberInfo[] memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);

            return (T)attributes.FirstOrDefault();
        }
        #endregion
    }
}
