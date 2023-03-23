using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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
