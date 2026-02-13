using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;

namespace KYS.Library.Validations
{
    /// <summary>
    /// Validation attribute for checking the provided value is a valid JSON object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonObjectAttribute : ValidationAttribute
    {
        /// <summary>
        /// Gets or sets the indicator allows the value with null or empty.
        /// </summary>
        public bool IsAllowNullOrEmpty { get; set; }

        /// <summary>
        /// Initialize new instance of <see cref="JsonObjectAttribute"/> class.
        /// </summary>
        public JsonObjectAttribute() : base("{0} is an invalid JSON object.")
        {
            IsAllowNullOrEmpty = true;
        }

        /// <summary>
        /// Initialize new instance of <see cref="JsonObjectAttribute"/> class.
        /// </summary>
        /// <param name="errorMessage">The displayed error message when the validation fails.</param>
        public JsonObjectAttribute(string errorMessage) : base(errorMessage)
        {
            IsAllowNullOrEmpty = true;
        }

        /// <summary>
        /// Initialize new instance of <see cref="JsonObjectAttribute"/> class.
        /// </summary>
        /// <param name="isAllowNullOrEmpty">The indicator allows the value is null or empty.</param>
        /// <param name="errorMessage">The displayed error message when the validation fails.</param>
        public JsonObjectAttribute(bool isAllowNullOrEmpty, string errorMessage) : base(errorMessage)
        {
            IsAllowNullOrEmpty = isAllowNullOrEmpty;
        }

        /// <summary>
        /// Determines whether the value from the current property is a valid JSON object.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <param name="validationContext">The <see cref="ValidationContext"/> instance.</param>
        /// <returns>The <see cref="ValidationResult"/> instance containing the result after validation.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (IsAllowNullOrEmpty
                    && (value == null || String.IsNullOrEmpty((string)value)))
                    return ValidationResult.Success;

                JObject.Parse((string)value);

                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult(null);
            }
        }
    }
}
