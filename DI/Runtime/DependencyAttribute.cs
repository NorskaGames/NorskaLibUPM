using System;

namespace NorskaLib.DI
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DependencyAttribute : Attribute { }
}