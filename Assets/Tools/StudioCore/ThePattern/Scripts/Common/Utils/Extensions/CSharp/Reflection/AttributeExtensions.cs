using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace ThePattern
{
    public static class AttributeExtensions
    {
        public static List<T> GetCustomAttributes<T>(this FieldInfo field) where T : Attribute
        {
            T[] customAttributes = (T[])field.GetCustomAttributes(typeof(T), false); // Get All Attribute in Field but not in inherit
            return customAttributes != null && (uint)customAttributes.Length > 0U ? customAttributes.ToList<T>() : new List<T>();
        }
        public static T GetCustomAttribute<T>(this FieldInfo field) where T : Attribute
        {
            T[] customAttributes = (T[])field.GetCustomAttributes(typeof(T), false); // Get All Attribute in Field but not in inherit
            return customAttributes != null && (uint)customAttributes.Length > 0U ? ((IEnumerable<T>)customAttributes).FirstOrDefault<T>() : default(T);
        }
        public static T GetCustomAttribute<T>(this PropertyInfo proper) where T : Attribute
        {
            T[] customAttributes = (T[])proper.GetCustomAttributes(typeof(T), false); // Get All Attribute in Property but not in inherit
            return customAttributes != null && (uint)customAttributes.Length > 0U ? ((IEnumerable<T>)customAttributes).FirstOrDefault<T>() : default(T);
        }
        public static List<T> GetCustomAttributes<T>(this Type type) where T : Attribute
        {
            List<T> objList = new List<T>();
            T[] customAttributes = (T[])type.GetCustomAttributes(typeof(T), false); // Get All Attribute in type but not in inherit
            return customAttributes != null && (uint)customAttributes.Length > 0U ? ((IEnumerable<T>)customAttributes).ToList<T>() : objList;
        }
        public static T GetCustomAttribute<T>(this Type type) where T : Attribute
        {
            T[] customAttributes = (T[])type.GetCustomAttributes(typeof(T), false); // Get All Attribute in type but not in inherit
            return customAttributes != null && (uint)customAttributes.Length > 0U ? ((IEnumerable<T>)customAttributes).FirstOrDefault<T>() : default(T);
        }
        public static T GetCustomAttribute<T>(Enum e) where T : Attribute => e.GetType().GetField(e.ToString()).GetCustomAttribute<T>();
    }
}