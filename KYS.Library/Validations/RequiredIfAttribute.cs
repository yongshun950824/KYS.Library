using System;
using System.ComponentModel.DataAnnotations;

namespace KYS.Library.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        readonly string _otherPropertyName;
        readonly string _matchedValue;

        public RequiredIfAttribute(string otherPropertyName, string matchedValue)
            : base()
        {
            _otherPropertyName = otherPropertyName;
            _matchedValue = matchedValue;
        }

        public RequiredIfAttribute(string otherPropertyName, string matchedValue, string errorMessage)
            : base(errorMessage)
        {
            _otherPropertyName = otherPropertyName;
            _matchedValue = matchedValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;

            try
            {
                // Using reflection to get a reference to the other property
                var otherPropertyInfo = validationContext.ObjectType.GetProperty(this._otherPropertyName);
                string referencePropertyValue = (string)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

                if (String.Equals(_matchedValue, referencePropertyValue)
                    && String.IsNullOrEmpty((string)value))
                {
                    validationResult = new ValidationResult(null);
                }
            }
            catch
            {
                throw;
            }

            return validationResult;
        }
    }
}
