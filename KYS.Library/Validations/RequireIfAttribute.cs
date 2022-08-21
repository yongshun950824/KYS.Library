using System;
using System.ComponentModel.DataAnnotations;

namespace KYS.Library.Validations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequireIfAttribute : ValidationAttribute
    {
        string otherPropertyValue;
        string triggerValue;

        public RequireIfAttribute(string otherPropertyValue, string triggerValue)
            : base()
        {
            this.otherPropertyValue = otherPropertyValue;
            this.triggerValue = triggerValue;
        }

        public RequireIfAttribute(string otherPropertyValue, string triggerValue, string errorMessage)
            : base(errorMessage)
        {
            this.otherPropertyValue = otherPropertyValue;
            this.triggerValue = triggerValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;

            try
            {
                // Using reflection to get a reference to the other property
                var otherPropertyInfo = validationContext.ObjectType.GetProperty(this.otherPropertyValue);
                string referencePropertyValue = (string)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

                if (String.Equals(triggerValue, referencePropertyValue))
                {
                    if (String.IsNullOrEmpty((string)value))
                        validationResult = new ValidationResult(null);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return validationResult;
        }
    }
}
