using KYS.Library.Extensions;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace KYS.Library.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IEnumerableRequireAttribute : ValidationAttribute
    {
        public IEnumerableRequireAttribute()
        {

        }

        public IEnumerableRequireAttribute(string errorMessage) : base(errorMessage)
        {

        }

        public override bool IsValid(object value)
        {
            var enumerable = value as IEnumerable;

            return !enumerable.IsNullOrEmpty();
        }
    }
}
