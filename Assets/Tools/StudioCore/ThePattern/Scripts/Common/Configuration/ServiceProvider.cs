using ThePattern.Common.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

using ThePattern.Extensions;

namespace ThePattern.Common.Injection
{
    public class ServiceProvider
    {
        public const string UNITY_ASSEMBLY = "Assembly-CSharp";

        private static Dictionary<Type, Type> _serviceProvider = new Dictionary<Type, Type>();
        private static Dictionary<Type, object> _serviceSingleton = new Dictionary<Type, object>();
        private static Dictionary<Type, string> _serviceInAssembly = new Dictionary<Type, string>();

        private static object CreateInstance(Type implementType)
        {
            // TODO: Need to fill Create object with Contructor have parameter
            // // Get Parameter[] of All contructor, Order By Descending Length
            // ParameterInfo[][] array = implementType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            //     .Select(o => o.GetParameters())
            //     .OrderByDescending(o => o.Length)
            //     .ToArray();
            // // Check if array is null
            // if (!array.Any())
            //     return null;
            // object[] objArray = ServiceProvider.ResolveParameters(array);
            // return objArray == null ? Activator.CreateInstance(implementType) : Activator.CreateInstance(implementType, objArray);
            return Activator.CreateInstance(implementType);
        }

        // private static object[] ResolveParameters(ParameterInfo[][] constructors) => (object[])null;

        public static void RegisterInjection<TService, TImplement>(string assemblyName = "") where TImplement : IInjection
        {
            if (!string.IsNullOrWhiteSpace(assemblyName))
                _serviceInAssembly[typeof(TService)] = assemblyName;
            _serviceProvider[typeof(TService)] = typeof(TImplement);
        }
        public static void RegisterSingleton<TService>(object implement) where TService : IInjection => _serviceSingleton[typeof(TService)] = implement;

        public static T CreateInjection<T>(string assemblyName = "") where T : IInjection
        {
            Type key = typeof(T); // Get Type
            if (string.IsNullOrWhiteSpace(assemblyName) && _serviceInAssembly.ContainsKey(key)) // Check for set assemblyName
                assemblyName = _serviceInAssembly[key];
            if (_serviceSingleton.ContainsKey(key)) // Return instance if have before
                return (T)_serviceSingleton[key];
            if (_serviceProvider.ContainsKey(key)) // Create Instance and return if dont have before but register Type
            {
                T instance = (T)CreateInstance(ServiceProvider._serviceProvider[key]);
                RegisterSingleton<T>(instance);
                return instance;
            }
            if (!string.IsNullOrWhiteSpace(assemblyName)) 
            {
                DateTime now = DateTime.Now;
                Assembly assembly = Assembly.Load(assemblyName); // Get Assembly to Create Module
                if (assembly != null) // If can get assembly => Create Module
                {
                    T module = assembly.GetModule<T>(); // Get Module
                    if (module != null) // If can Get Module => Register Singleton and Log time load injection
                    {
                        if (PatternInjection.Configuration.EnableVerboseLog)
                            Debug.LogWarning("Spent: " + DateTime.Now.Subtract(now).TotalSeconds.ToString() + "s - For load dependency of injection: " + key.Name);
                        ServiceProvider.RegisterSingleton<T>((object)module);
                    }
                    else if (PatternInjection.Configuration.EnableVerboseLog) // If can't Get Module => Log err
                        Debug.LogWarning("Not found class inheriting from: " + key.Name + ".\nIf you want custom some Service of Ensign, please create a class extends from " + key.Name);
                    return module; // return result
                }
                if (PatternInjection.Configuration.EnableVerboseLog)
                    Debug.LogWarning("Assembly " + assemblyName + " not found!"); // If can't get assembly => Log Err
            }
            return default(T); // assemblyName = null or empty => return null
        }
    }
}