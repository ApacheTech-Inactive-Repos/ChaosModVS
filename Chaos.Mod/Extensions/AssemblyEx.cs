using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chaos.Mod.Extensions
{
    public static class AssemblyEx
    {
        public static IEnumerable<T> GetEnumerableOfType<T>(this Assembly assembly, params object[] constructorArgs)
            where T : class, IComparable<T>
        {
            var objects = assembly
                .GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)))
                .Select(type => (T) Activator.CreateInstance(type, constructorArgs))
                .ToList();
            objects.Sort();

            return objects;
        }

        public static IEnumerable<Type> GetDerivedTypes<T>(this T attribute, Assembly assembly) where T : Attribute
        {
            return assembly.GetTypes()
                .Where(type => type.GetCustomAttributes(typeof(T), true).Length > 0);
        }

        public static IEnumerable<Type> GetTypesWithAttribute<T>(this Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(type => type.GetCustomAttributes(typeof(T), true).Length > 0);
        }
    }
}