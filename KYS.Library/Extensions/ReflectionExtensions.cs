using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="System.Reflection"/> namespace.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Obtain the property name from <see cref="DisplayAttribute"/> and <see cref="DisplayNameAttribute"/> in sequence.
        /// <br /><br />
        /// If neither attributes are applied, return the property name.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> instance this method extends.</param>
        /// <returns>Property name.</returns>
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

        /// <summary>
        /// Obtain the member name from <see cref="DisplayAttribute"/> and <see cref="DisplayNameAttribute"/> (first-come basis).
        /// <br /><br />
        /// If neither attributes are applied, return the member name.
        /// </summary>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> instance this method extends.</param>
        /// <returns>Member name.</returns>
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
        /// Retrieve the value of <see cref="DisplayAttribute"/> and <see cref="DisplayNameAttribute"/> (first-come basis) from a property of the class. 
        /// <br /><br />
        /// Usage: 
        /// <br />
        /// <c>ReflectionExtensions.GetPropertyDisplayName((Class x) =&#x3e; x.Property)</c>
        /// <br /><br />
        /// Reference: <a href="https://stackoverflow.com/a/74846301/8017690">get the value of DisplayName attribute</a>
        /// </summary>
        /// <typeparam name="T">Must be a reference type with a public parameterless constructor.</typeparam>
        /// <param name="propertyExpression">The expression references the property.</param>
        /// <returns>Property name.</returns>
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
