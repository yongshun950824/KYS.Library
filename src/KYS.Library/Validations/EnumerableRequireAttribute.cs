using KYS.Library.Extensions;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace KYS.Library.Validations
{
    /// <summary>
    /// Validation attribute for checking the <see cref="IEnumerable"/> field is neither null or empty.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EnumerableRequireAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initialize new instance of <see cref="EnumerableRequireAttribute"/> class.
        /// </summary>
        public EnumerableRequireAttribute()
        {

        }

        /// <summary>
        /// Initialize new instance of <see cref="EnumerableRequireAttribute"/> class.
        /// </summary>
        /// <param name="errorMessage">The displayed error message when the validation fails.</param>
        public EnumerableRequireAttribute(string errorMessage) : base(errorMessage)
        {

        }

        /// <summary>
        /// Determines whether the specified value of the object is a <see cref="IEnumerable"/> and neither null nor empty.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns><see langword="true"/> if the specified value is valid; otherwise, <see langword="false"/>.</returns>
        public override bool IsValid(object value)
        {
            var enumerable = value as IEnumerable;

            return !enumerable.IsNullOrEmpty();
        }
    }
}
