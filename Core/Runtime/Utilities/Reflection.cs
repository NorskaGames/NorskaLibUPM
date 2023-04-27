using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NorskaLib.Utilities
{
    public struct ReflectionUtils
    {
        public static IEnumerable<Type> GetSubclasses<T>(bool includeAbstract, Assembly assembly = null) where T : class
        {
            var type = typeof(T);
            if (assembly is null)
                assembly = Assembly.GetAssembly(type);

            return assembly.GetTypes()
                .Where(t => t.IsClass && (includeAbstract || !t.IsAbstract) && t.IsSubclassOf(type));
        }

        public static IEnumerable<Type> GetAssignables<T>(Assembly assembly = null)
        {
            var type = typeof(T);
            return (assembly ?? Assembly.GetAssembly(type)).GetTypes()
                .Where(t => t != type && type.IsAssignableFrom(t));
        }
    }
}