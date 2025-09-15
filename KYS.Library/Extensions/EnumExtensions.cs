using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace KYS.Library.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<TEnum> GetEnumValues<TEnum>()
            where TEnum : Enum
        {
            TEnum valueType = default(TEnum);

            return typeof(TEnum).GetFields()
                .Select(fieldInfo => (TEnum)fieldInfo.GetValue(valueType))
                .Distinct();
        }

        public static IEnumerable<string> GetEnumNames<TEnum>()
            where TEnum : Enum
        {
            return typeof(TEnum).GetFields()
                .Select(fieldInfo => fieldInfo.Attributes.ToDescription())
                .Distinct();
        }

        public static string ToDescription(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            if (attribute == null)
                return value.ToString();

            return attribute.Description;
        }

        public static Dictionary<TEnum, string> ToDictionary<TEnum>()
            where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                 .Cast<TEnum>()
                 .ToDictionary(k => k, v => v.ToDescription());
        }

        /// <summary>
        /// Get value from <c>EnumMemberAttribute</c>.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
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

            throw new ArgumentException($"{value} is not found.");
        }

        public static TEnum GetValueByDescriptionAttribute<TEnum>(string value)
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
