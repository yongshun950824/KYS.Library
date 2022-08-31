using System;
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

        public RequiredIf2Attribute(string otherPropertyName, object matchedValue) : base()
        {
            this._otherPropertyName = otherPropertyName;
            this._matchedValue = matchedValue;
        }

        public RequiredIf2Attribute(string otherPropertyName, object matchedValue, string errorMessage) : base(errorMessage)
        {
            this._otherPropertyName = otherPropertyName;
            this._matchedValue = matchedValue;
        }

        public RequiredIf2Attribute(
            string otherPropertyName,
            object matchedValue,
            CompareOperator.CompareOperatorConstants @operator) : base()
        {
            this._otherPropertyName = otherPropertyName;
            this._matchedValue = matchedValue;
            this._operator = @operator;
        }

        public RequiredIf2Attribute(
            string otherPropertyName,
            object matchedValue,
            CompareOperator.CompareOperatorConstants @operator,
            string errorMessage) : base(errorMessage)
        {
            this._otherPropertyName = otherPropertyName;
            this._matchedValue = matchedValue;
            this._operator = @operator;
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

                /*
				MethodInfo operatorFunc = typeof(CompareOperator)
					.GetMethod(nameof(CompareOperator.GetOperatorFunc))
					.MakeGenericMethod(otherPropertyType);
				*/

                MethodInfo operatorFunc = typeof(CompareOperator)
                    .GetMethods()
                    .Where(x => x.IsGenericMethod
                        && x.Name == nameof(CompareOperator.Compare))
                    .First()
                    .MakeGenericMethod(otherPropertyType);

                // TO-DO: Work with GetCompareOperatorFunc (third method)

                if ((bool)operatorFunc.Invoke(this, new[] { (object)_operator, referencePropertyValue, _matchedValue }))
                {
                    if (value == null
                        || value == default)
                        validationResult = new ValidationResult(null, new string[] { validationContext.MemberName });
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return validationResult;
        }
    }

    public static class CompareOperator
    {
        public enum CompareOperatorConstants
        {
            Equal,
            NotEqual,
            LesserThan,
            LesserThanOrEqual,
            GreaterThan,
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

                case CompareOperatorConstants.LesserThan:
                    return a.CompareTo(b) == -1;

                case CompareOperatorConstants.LesserThanOrEqual:
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

                case CompareOperatorConstants.LesserThan:
                    return a.CompareTo(b) == -1;

                case CompareOperatorConstants.LesserThanOrEqual:
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

        #region Unsupported methods
        /// <summary>
        /// TO-DO: Migrate RequiredIf2Attribute to support run Func from reflection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operator"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        //public static Func<T, T, bool> GetCompareOperatorFunc<T>(CompareOperatorConstants @operator) where T : IComparable, IComparable<T>
        //{
        //	switch (@operator)
        //	{
        //		case CompareOperatorConstants.Equal:
        //			return (a, b) => a.Equals(b);

        //		case CompareOperatorConstants.NotEqual:
        //			return (a, b) => !a.Equals(b);

        //		case CompareOperatorConstants.LesserThan:
        //			return (a, b) => a.CompareTo(b) == -1;

        //		case CompareOperatorConstants.LesserThanOrEqual:
        //			return (a, b) => a.CompareTo(b) == -1
        //				|| a.CompareTo(b) == 0;

        //		case CompareOperatorConstants.GreaterThan:
        //			return (a, b) => a.CompareTo(b) == 1;

        //		case CompareOperatorConstants.GreaterThanOrEqual:
        //			return (a, b) => a.CompareTo(b) == 1
        //				|| a.CompareTo(b) == 0;

        //		default:
        //			throw new Exception("Invalid operator");
        //	}
        //}
        #endregion
    }
}
