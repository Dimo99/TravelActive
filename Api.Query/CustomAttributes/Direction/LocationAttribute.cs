using System;

namespace Api.Query.CustomAttributes.Direction
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
    public class LocationAttribute : Attribute
    {
        public string Label { get; set; }
    }
}