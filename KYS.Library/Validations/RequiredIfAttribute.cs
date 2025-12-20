using System;
using System.ComponentModel.DataAnnotations;

namespace KYS.Library.Validations
{
    /// <summary>
    /// Validation attribute for performing the required validation on the current property when other property fulfills the specific value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        readonly string _otherPropertyName;
        readonly string _matchedValue;

        /// <summary>
        /// Initialize new instance of <see cref="RequiredIfAttribute"/> class.
        /// </summary>
        /// <param name="otherPropertyName">The name of property which the property is used to obtain its value and compare with <see cref="matchedValue"/>.</param>
        /// <param name="matchedValue">The value which must be matched with the value of <see cref="otherPropertyName"/> to perform the validation.</param>
        public RequiredIfAttribute(string otherPropertyName, string matchedValue)
            : base()
        {
            _otherPropertyName = otherPropertyName;
            _matchedValue = matchedValue;
        }

        /// <summary>
        /// Initialize new instance of <see cref="RequiredIfAttribute"/> class.
        /// </summary>
        /// <param name="otherPropertyName">The name of property which the property is used to obtain its value and compare with <see cref="matchedValue"/>.</param>
        /// <param name="matchedValue">The value which must be matched with the value of <see cref="otherPropertyName"/> to perform the validation.</param>
        /// <param name="errorMessage">The displayed error message when the validation fails.</param>
        public RequiredIfAttribute(string otherPropertyName, string matchedValue, string errorMessage)
            : base(errorMessage)
        {
            _otherPropertyName = otherPropertyName;
            _matchedValue = matchedValue;
        }

        /// <summary>
        /// Determines whether the value from the current property is valid (neither null nor empty)
        /// if the value from the <c>otherPropertyInfo</c> is fulfilled, represents the current property is mandatory.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <param name="validationContext">The <see cref="ValidationContext"/> instance.</param>
        /// <returns>The <see cref="ValidationResult"/> instance containing the result after validation.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;

            // Using reflection to get a reference to the other property
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(this._otherPropertyName);
            string referencePropertyValue = (string)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (String.Equals(_matchedValue, referencePropertyValue)
                && String.IsNullOrEmpty((string)value))
            {
                validationResult = new ValidationResult(null);
            }

            return validationResult;
        }
    }
}
