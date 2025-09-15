using KYS.Library.Extensions;
using KYS.Library.Helpers;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace KYS.Library.Validations
{
    /// <summary>
    /// RequiredIf attribute v2 supported for compare operators. (Temporarily named as: `RequiredIf2`)
    /// <br /> <br />
    /// <remarks>
    /// Usage:  <br />
    /// <c>
    /// [RequiredIf2(    <br />
	///	otherPropertyName: nameof(PropertyOne),     <br />
	///	matchedValue: 1,    <br />
	///	operator: CompareOperator.CompareOperatorConstants.NotEqual)]   <br />
    ///	</c>
    /// <br /> <br/>
    /// <a href="https://dotnetfiddle.net/39SX03">Test case</a>
    /// </remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIf2Attribute : ValidationAttribute
    {
        readonly string _otherPropertyName;
        readonly object _matchedValue;
        readonly CompareOperator.CompareOperatorConstants _operator = CompareOperator.CompareOperatorConstants.Equal;
        readonly string _errorMessage;

        public RequiredIf2Attribute(string otherPropertyName, object matchedValue) : base()
        {
            _otherPropertyName = otherPropertyName;
            _matchedValue = matchedValue;
        }

        public RequiredIf2Attribute(string otherPropertyName, object matchedValue, string errorMessage) : base(errorMessage)
        {
            _otherPropertyName = otherPropertyName;
            _matchedValue = matchedValue;
            _errorMessage = errorMessage;
        }

        public RequiredIf2Attribute(
            string otherPropertyName,
            object matchedValue,
            CompareOperator.CompareOperatorConstants @operator) : base()
        {
            _otherPropertyName = otherPropertyName;
            _matchedValue = matchedValue;
            _operator = @operator;
        }

        public RequiredIf2Attribute(
            string otherPropertyName,
            object matchedValue,
            CompareOperator.CompareOperatorConstants @operator,
            string errorMessage) : base(errorMessage)
        {
            _otherPropertyName = otherPropertyName;
            _matchedValue = matchedValue;
            _operator = @operator;
            _errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;
            try
            {
                // Using reflection to get a reference to the other property
                var otherPropertyInfo = validationContext.ObjectType.GetProperty(this._otherPropertyName);
                Type otherPropertyType = otherPropertyInfo.PropertyType;

                // Return Validation.Success for unmatched type
                if (_matchedValue.GetType() != otherPropertyType)
                    return validationResult;

                var referencePropertyValue = Convert.ChangeType(otherPropertyInfo.GetValue(validationContext.ObjectInstance, null), otherPropertyType);

                Func<IComparable, IComparable, bool> operatorFunc = CompareOperator.GetCompareOperatorFunc(_operator);
                bool isValid = operatorFunc.Invoke((IComparable)referencePropertyValue, (IComparable)_matchedValue);

                if (isValid)
                {
                    bool isNull = value == null;
                    bool isEmptyString = value is string str && String.IsNullOrEmpty(str);
                    bool isValueTypeWithDefault = value != null
                        && value.GetType().IsValueType
                        && value.Equals(Activator.CreateInstance(value.GetType()));
                    bool hasParameterlessConstructor = value?.GetType().GetConstructor(Type.EmptyTypes) != null;
                    bool matchesDefaultInstance = hasParameterlessConstructor
                        && value.ToString() == Activator.CreateInstance(value.GetType()).ToString();

                    if (isNull || isEmptyString || isValueTypeWithDefault || matchesDefaultInstance)
                        validationResult = new ValidationResult(null, new string[] { validationContext.MemberName });
                }
            }
            catch
            {
                throw;
            }

            return validationResult;
        }

        public override string FormatErrorMessage(string name)
        {
            if (!String.IsNullOrEmpty(_errorMessage))
                return _errorMessage;

            // Default Error Message: "{Current Property} must be provided when {Other Property} is {operator desc} {value}."
            return $"{name} must be provided when {_otherPropertyName} is {_operator.ToDescription().ToLower()} {_matchedValue}.";
        }
    }
}
