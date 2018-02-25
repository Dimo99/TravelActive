using System;

namespace Api.ION
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SecretAttribute : Attribute
    {

    }
}