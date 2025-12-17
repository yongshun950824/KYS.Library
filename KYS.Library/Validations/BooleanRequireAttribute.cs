using System;
using System.ComponentModel.DataAnnotations;

namespace KYS.Library.Validations
{
    /// <summary>
    /// Validation attribute for checking the <see cref="bool"> field is mandatory.
    /// <br />
    /// Alternative: <code>[Range(typeof(bool), "true", "true"]</code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class BooleanRequireAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initialize new instance of <see cref="BooleanRequireAttribute" /> class.
        /// </summary>
        public BooleanRequireAttribute() : base()
        {

        }

        /// <summary>
        /// Initialize new instance of <see cref="BooleanRequireAttribute" /> class.
        /// </summary>
        /// <param name="errorMessage">The displayed error message when the validation fails.</param>
        public BooleanRequireAttribute(string errorMessage) : base(errorMessage)
        {

        }

        /// <summary>
        /// Determines whether the specified value of the object is a <see cref="bool"/> and <see langword="true"/>.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns><see langword="true"/> if the specified value is valid; otherwise, <see langword="false"/>.</returns>
        public override bool IsValid(object value)
            => value is bool
                && (bool)value;
    }
}
