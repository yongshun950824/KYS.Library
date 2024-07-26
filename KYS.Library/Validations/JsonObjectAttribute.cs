using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;

namespace KYS.Library.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonObjectAttribute : ValidationAttribute
    {
        public bool IsAllowNullOrEmpty { get; set; }
        public JsonObjectAttribute() : base("{0} is an invalid JSON object.")
        {
            IsAllowNullOrEmpty = true;
        }

        public JsonObjectAttribute(string errorMessage) : base(errorMessage)
        {
            IsAllowNullOrEmpty = true;
        }

        public JsonObjectAttribute(bool isAllowNullOrEmpty, string errorMessage) : base(errorMessage)
        {
            IsAllowNullOrEmpty = isAllowNullOrEmpty;
        }

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
