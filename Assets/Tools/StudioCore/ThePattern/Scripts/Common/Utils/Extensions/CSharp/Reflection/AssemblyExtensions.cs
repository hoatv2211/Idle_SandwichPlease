using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

using ThePattern.Common.Injection;

namespace ThePattern.Extensions
{
    public static class AssemblyExtensions
    {
        public static T GetModule<T>(this Assembly assembly) where T : IInjection 
            => (T)assembly.GetTypes() // Get all Type in Assembly
                .Where(t => t.GetInterfaces().Contains(typeof(T)) && t.GetConstructor(Type.EmptyTypes) != null) // if -> Type inherit IInjection & have constructor no param
                .Select(t => Activator.CreateInstance(t)) // Create a instance of all
                .FirstOrDefault(); // Get first

        public static List<T> GetModules<T>(this Assembly assembly) where T : IInjection 
            => assembly.GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(T)) && t.GetConstructor(Type.EmptyTypes) != null)
                .Select(t => (T)Activator.CreateInstance(t))
                .ToList<T>();
    }
    
}
