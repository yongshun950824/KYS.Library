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
                object[] attributes = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

                #region Alternate
                // IEnumerable<DisplayAttribute> attributes = propertyInfo.GetCustomAttributes<DisplayAttribute>(false);
                #endregion

                if (!attributes.IsNullOrEmpty())
                    return ((DisplayAttribute)attributes[0]).Name;

                object[] displayNameAttributes = propertyInfo
                    .GetCustomAttributes(typeof(DisplayNameAttribute), false)
                    .ToArray();

                if (!displayNameAttributes.IsNullOrEmpty())
                    return ((DisplayNameAttribute)attributes[0]).DisplayName;

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
                    return displayAttributes.First()
                        .Name;

                DisplayNameAttribute[] displayNameAttributes = memberInfo
                    .GetCustomAttributes<DisplayNameAttribute>()
                    .ToArray();

                if (!displayNameAttributes.IsNullOrEmpty())
                    return displayNameAttributes.First()
                        .DisplayName;

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
        /// <c>ReflectionExtensions.GetPropertyDisplayName&#x3c;Class&#x3e;(x =&#x3e; x.Property)</c>
        /// <br /><br />
        /// Reference: <a href="https://stackoverflow.com/a/5015911">get the value of DisplayName attribute</a>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public static string GetPropertyDisplayName<T>(Expression<Func<T, object>> propertyExpression)
            where T : new()
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
                throw new ArgumentException("No property reference expression was found.", "propertyExpression");

            return memberInfo.ToName();
        }

        public static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null
                    && unaryExpr.NodeType == ExpressionType.Convert)
                    memberExpr = unaryExpr.Operand as MemberExpression;
            }

            if (memberExpr != null
                && memberExpr.Member.MemberType == MemberTypes.Property)
                return memberExpr.Member;

            return null;
        }

        /// <summary>
        /// Approach 2: To retrieve the value of <c>DisplayAttribute</c>/<c>DisplayNameAttribute</c> from a property of the class. 
        /// <br /><br />
        /// Usage: 
        /// <br />
        /// <c>ReflectionExtensions.GetPropertyDisplayName2&#x3c;Class&#x3e;(x =&#x3e; x.Property)</c>
        /// <br /><br />
        /// Reference: <a href="https://stackoverflow.com/a/74846301/8017690">get the value of DisplayName attribute</a>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public static string GetPropertyDisplayName2<T, P>(Expression<Func<T, P>> propertyExpression)
            where T : new()
        {
            MemberExpression memberExpr = propertyExpression.Body as MemberExpression;
            if (memberExpr == null)
                return null;

            return (memberExpr.Member as PropertyInfo)
                .ToName();
        }
    }
}
