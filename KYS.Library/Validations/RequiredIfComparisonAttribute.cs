using KYS.Library.Extensions;
using KYS.Library.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace KYS.Library.Validations
{
    /// <summary>
    /// <see cref="RequiredIfAttribute"/> attribute v2 supported for comparing the value with operator.
    /// <br /> <br />
    /// When the value from the other property is compared and fulfilled the statement for the configured formula: <br />
    /// <c>
    /// &lt;other property's value&gt; &lt;operator&gt; &lt;matchedValue&gt;
    /// </c>
    /// <br />
    /// , represents the current property is mandatory.
    /// <br /> <br />
    /// <remarks>
    /// Usage:  <br />
    /// <c>
    /// [RequiredIfComparison(    <br />
	///	otherPropertyName: nameof(PropertyOne),     <br />
	///	matchedValue: 1,    <br />
	///	operator: CompareOperator.CompareOperatorConstants.NotEqual)]   <br />
    ///	</c>
    /// <br /> <br/>
    /// <a href="https://dotnetfiddle.net/39SX03">Test case</a>
    /// </remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfComparisonAttribute : ValidationAttribute
    {
        readonly string _otherPropertyName;
        readonly object _matchedValue;
        readonly CompareOperator.CompareOperatorConstants _operator = CompareOperator.CompareOperatorConstants.Equal;
        readonly string _errorMessage;

        /// <summary>
        /// Initialize new instance of <see cref="RequiredIfComparisonAttribute"/> class.
        /// </summary>
        /// <param name="otherPropertyName">The name of property which the property is used to obtain its value and compare with <see cref="matchedValue"/>.</param>
        /// <param name="matchedValue">The value which must be matched with the value of <see cref="otherPropertyName"/> to perform the validation.</param>
        public RequiredIfComparisonAttribute(string otherPropertyName, object matchedValue) : base()
        {
            _otherPropertyName = otherPropertyName;
            _matchedValue = matchedValue;
        }

        /// <summary>
        /// Initialize new instance of <see cref="RequiredIfComparisonAttribute"/> class.
        /// </summary>
        /// <param name="otherPropertyName">The name of property which the property is used to obtain its value and compare with <see cref="matchedValue"/>.</param>
        /// <param name="matchedValue">The value which must be matched with the value of <see cref="otherPropertyName"/> to perform the validation.</param>
        /// <param name="errorMessage">The displayed error message when the validation fails.</param>
        public RequiredIfComparisonAttribute(string otherPropertyName, object matchedValue, string errorMessage) : base(errorMessage)
        {
            _otherPropertyName = otherPropertyName;
            _matchedValue = matchedValue;
            _errorMessage = errorMessage;
        }

        /// <summary>
        /// Initialize new instance of <see cref="RequiredIfComparisonAttribute"/> class.
        /// </summary>
        /// <param name="otherPropertyName">The name of property which the property is used to obtain its value and compare with <see cref="matchedValue"/>.</param>
        /// <param name="matchedValue">The value which must be matched with the value of <see cref="otherPropertyName"/> to perform the validation.</param>
        /// <param name="operator">The <see cref="CompareOperator.CompareOperatorConstants"/> operator used to compare the values.</param>
        public RequiredIfComparisonAttribute(
            string otherPropertyName,
            object matchedValue,
            CompareOperator.CompareOperatorConstants @operator) : base()
        {
            _otherPropertyName = otherPropertyName;
            _matchedValue = matchedValue;
            _operator = @operator;
        }

        /// <summary>
        /// Initialize new instance of <see cref="RequiredIfComparisonAttribute"/> class.
        /// </summary>
        /// <param name="otherPropertyName">The name of property which the property is used to obtain its value and compare with <see cref="matchedValue"/>.</param>
        /// <param name="matchedValue">The value which must be matched with the value of <see cref="otherPropertyName"/> to perform the validation.</param>
        /// <param name="operator">The <see cref="CompareOperator.CompareOperatorConstants"/> operator used to compare the values.</param>
        /// <param name="errorMessage">The displayed error message when the validation fails.</param>
        public RequiredIfComparisonAttribute(
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

        /// <summary>
        /// Determines whether the value from the current property is valid (neither null, empty nor default value)
        /// if the value from the <c>otherPropertyInfo</c> is fulfilled (the formula), represents the current property is mandatory.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <param name="validationContext">The <see cref="ValidationContext"/> instance.</param>
        /// <returns>The <see cref="ValidationResult"/> instance containing the result after validation.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;

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
