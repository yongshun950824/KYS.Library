using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace KYS.Library.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetEnumValues<T>() where T : Enum
        {
            T valueType = default(T);

            return typeof(T).GetFields()
                .Select(fieldInfo => (T)fieldInfo.GetValue(valueType))
                .Distinct();
        }

        public static IEnumerable<string> GetEnumNames<T>() where T : Enum
        {
            return typeof(T).GetFields()
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
