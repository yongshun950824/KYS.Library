using System;
using System.ComponentModel.DataAnnotations;

namespace KYS.Library.Validations
{
    /// <summary>
    /// Validation attribute for checking boolean field is mandatory.
    /// <br />
    /// Alternative: <code>[Range(typeof(bool), "true", "true"]</code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class BooleanRequireAttribute : ValidationAttribute
    {
        public BooleanRequireAttribute() : base()
        {

        }

        public BooleanRequireAttribute(string errorMessage) : base(errorMessage)
        {

        }

        public override bool IsValid(object value)
            => value is bool
                && (bool)value;
    }
}
