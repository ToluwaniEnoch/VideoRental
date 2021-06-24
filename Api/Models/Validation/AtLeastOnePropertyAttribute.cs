using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Api.Models.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AtLeastOnePropertyAttribute : ValidationAttribute
    {
        private string[] PropertyList { get; set; }

        public AtLeastOnePropertyAttribute(params string[] propertyList)
        {
            PropertyList = propertyList;
        }

        public override object TypeId => this;

        public override bool IsValid(object? value)
        {
            PropertyInfo? propertyInfo;
            foreach (string propertyName in PropertyList)
            {
                propertyInfo = value?.GetType()?.GetProperty(propertyName);

                if (propertyInfo != null && propertyInfo.GetValue(value, null) != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}