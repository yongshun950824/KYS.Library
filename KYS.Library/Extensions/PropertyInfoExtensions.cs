using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace KYS.Library.Extensions
{
    public static class PropertyInfoExtensions
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

                return propertyInfo.Name;
            }
            catch
            {
                return null;
            }
        }
    }
}
