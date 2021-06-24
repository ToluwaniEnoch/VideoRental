using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string property;
        private readonly object triggerValue;

        public RequiredIfAttribute(string property, object triggerValue)
        {
            this.property = property;
            this.triggerValue = triggerValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(property);

            if (propertyInfo is null)
                return new ValidationResult($"Unknown Property {property}");

            var dependentProperty = validationContext.MemberName;

            if (triggerValue.GetType() != propertyInfo.PropertyType)
                return new ValidationResult($"Wrong Property Type Set For {dependentProperty} dependent on {property}");

            var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance);

            if (propertyValue is null) return ValidationResult.Success;

            var type = triggerValue.GetType();

            if (type.IsValueType)
            {
                if (!propertyValue.Equals(triggerValue) || (propertyValue.Equals(triggerValue) && value != null))
                    return ValidationResult.Success;
            }
            else
            {
                if ((propertyValue != triggerValue) || (propertyValue == triggerValue && value != null))
                    return ValidationResult.Success;
            }

            return new ValidationResult($"{dependentProperty} is required when {property} is {triggerValue}");
        }
    }
}
