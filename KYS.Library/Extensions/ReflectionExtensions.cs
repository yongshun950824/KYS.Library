using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KYS.Library.Extensions
{
    public static class ReflectionExtensions
    {
        public static string ToName(this PropertyInfo propertyInfo)
        {
            try
            {
                DisplayAttribute[] attributes = propertyInfo.GetCustomAttributes<DisplayAttribute>(false)
                   .ToArray();

                if (!attributes.IsNullOrEmpty())
                    return attributes[0].Name;

                DisplayNameAttribute[] displayNameAttributes = propertyInfo
                    .GetCustomAttributes<DisplayNameAttribute>(false)
                    .ToArray();

                if (!displayNameAttributes.IsNullOrEmpty())
                    return displayNameAttributes[0].DisplayName;

                return propertyInfo.Name;
            }
            catch
            {
                return propertyInfo.Name;
            }
        }

        public static string ToName(this MemberInfo memberInfo)
        {
            try
            {
                DisplayAttribute[] displayAttributes = memberInfo
                    .GetCustomAttributes<DisplayAttribute>()
                    .ToArray();

                if (!displayAttributes.IsNullOrEmpty())
                    return displayAttributes[0].Name;

                DisplayNameAttribute[] displayNameAttributes = memberInfo
                    .GetCustomAttributes<DisplayNameAttribute>()
                    .ToArray();

                if (!displayNameAttributes.IsNullOrEmpty())
                    return displayNameAttributes[0].DisplayName;

                return memberInfo.Name;
            }
            catch
            {
                return memberInfo.Name;
            }
        }

        /// <summary>
        /// To retrieve the value of <c>DisplayAttribute</c>/<c>DisplayNameAttribute</c> from a property of the class. 
        /// <br /><br />
        /// Usage: 
        /// <br />
        /// <c>ReflectionExtensions.GetPropertyDisplayName((Class x) =&#x3e; x.Property)</c>
        /// <br /><br />
        /// Reference: <a href="https://stackoverflow.com/a/74846301/8017690">get the value of DisplayName attribute</a>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public static string GetPropertyDisplayName<T, P>(Expression<Func<T, P>> propertyExpression)
            where T : new()
        {
            if (propertyExpression.Body is MemberExpression memberExpr
                && memberExpr.Member is PropertyInfo propInfo)
                return propInfo.ToName();

            return null;
        }
    }
}
