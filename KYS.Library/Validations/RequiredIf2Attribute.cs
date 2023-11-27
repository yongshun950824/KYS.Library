using KYS.Library.Extensions;
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

                bool isValid = false;
                #region Approach 1
                /*
                MethodInfo operatorFunc = typeof(CompareOperator)
                    .GetMethods()
                    .Where(x => x.IsGenericMethod
                        && x.Name == nameof(CompareOperator.Compare))
                    .First()
                    .MakeGenericMethod(otherPropertyType);

                isValid = (bool)operatorFunc.Invoke(this, new[] { _operator, referencePropertyValue, _matchedValue });
                */
                #endregion

                #region Approach 2
                Func<IComparable, IComparable, bool> operatorFunc = CompareOperator.GetCompareOperatorFunc(_operator);
                isValid = operatorFunc.Invoke((IComparable)referencePropertyValue, (IComparable)_matchedValue);
                #endregion

                if (isValid)
                {
                    if (value == null
                        || value.ToString() == Activator.CreateInstance(value.GetType()).ToString())
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

    public static class CompareOperator
    {
        public enum CompareOperatorConstants
        {
            [Description("Equal to")]
            Equal,
            [Description("Not equal to")]
            NotEqual,
            [Description("Less than")]
            LessThan,
            [Description("Less than or equal to")]
            LessThanOrEqual,
            [Description("Greater than")]
            GreaterThan,
            [Description("Greater than or equal to")]
            GreaterThanOrEqual
        }

        public static bool Compare(CompareOperatorConstants @operator, IComparable a, IComparable b)
        {
            switch (@operator)
            {
                case CompareOperatorConstants.Equal:
                    return a.Equals(b);

                case CompareOperatorConstants.NotEqual:
                    return !a.Equals(b);

                case CompareOperatorConstants.LessThan:
                    return a.CompareTo(b) == -1;

                case CompareOperatorConstants.LessThanOrEqual:
                    return a.CompareTo(b) == -1
                        || a.CompareTo(b) == 0;

                case CompareOperatorConstants.GreaterThan:
                    return a.CompareTo(b) == 1;

                case CompareOperatorConstants.GreaterThanOrEqual:
                    return a.CompareTo(b) == 1
                        || a.CompareTo(b) == 0;

                default:
                    throw new ArgumentException("Invalid operator");
            }
        }

        public static bool Compare<T>(CompareOperatorConstants @operator, T a, T b) where T : IComparable, IComparable<T>
        {
            switch (@operator)
            {
                case CompareOperatorConstants.Equal:
                    return a.Equals(b);

                case CompareOperatorConstants.NotEqual:
                    return !a.Equals(b);

                case CompareOperatorConstants.LessThan:
                    return a.CompareTo(b) == -1;

                case CompareOperatorConstants.LessThanOrEqual:
                    return a.CompareTo(b) == -1
                        || a.CompareTo(b) == 0;

                case CompareOperatorConstants.GreaterThan:
                    return a.CompareTo(b) == 1;

                case CompareOperatorConstants.GreaterThanOrEqual:
                    return a.CompareTo(b) == 1
                        || a.CompareTo(b) == 0;

                default:
                    throw new ArgumentException("Invalid operator");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="operator"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Func<IComparable, IComparable, bool> GetCompareOperatorFunc(CompareOperatorConstants @operator)
        {
            switch (@operator)
            {
                case CompareOperatorConstants.Equal:
                    return (a, b) => a.Equals(b);

                case CompareOperatorConstants.NotEqual:
                    return (a, b) => !a.Equals(b);

                case CompareOperatorConstants.LessThan:
                    return (a, b) => a.CompareTo(b) == -1;

                case CompareOperatorConstants.LessThanOrEqual:
                    return (a, b) => a.CompareTo(b) == -1
                        || a.CompareTo(b) == 0;

                case CompareOperatorConstants.GreaterThan:
                    return (a, b) => a.CompareTo(b) == 1;

                case CompareOperatorConstants.GreaterThanOrEqual:
                    return (a, b) => a.CompareTo(b) == 1
                        || a.CompareTo(b) == 0;

                default:
                    throw new Exception("Invalid operator");
            }
        }
    }
}
