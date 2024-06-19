using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ThePattern.Unity;
using ThePattern.Unity.Injection;
using ThePattern.Extensions;
using ThePattern.Attributes;
using ThePattern.Common.Injection;

namespace ThePattern.Unity
{
    [Resource(isDontDestroyOnLoad: true)]
    public class ThePatternAppication : Singleton<ThePatternAppication>
    {
        protected override void Init()
        {
            this.gameObject.isStatic = true; // Set StaticEditorFlags => include in their precomputations in Unity Editor
            this.gameObject.hideFlags = HideFlags.HideAndDontSave; // Object not shown in Hierarchy, not saved to Scenes, and can be destroy by owner

            LoadServiceInjection();
        }

        private void LoadServiceInjection()
        {
            Assembly assembly = Assembly.Load("Assembly-CSharp");
            if (assembly == null)
                return;
            List<IServiceInjection> modules = assembly.GetModules<IServiceInjection>();
            List<IServiceInjection> orderedInjections = new List<IServiceInjection>();
            List<IServiceInjection> normalInjection = new List<IServiceInjection>();
            modules.ForEach(serviceInjection =>
            {
                if (serviceInjection.GetType().GetCustomAttribute<InjectionOrderAttribute>() != null)
                {
                    orderedInjections.Add(serviceInjection);
                }
                else
                {
                    normalInjection.Add(serviceInjection);
                }
            });
            List<IServiceInjection> list = orderedInjections
                .OrderBy(p => p.GetType().GetCustomAttribute<InjectionOrderAttribute>().Order)
                .ToList();
            list.AddRange(normalInjection);
            list.ForEach(serviceInjection =>
            {
                try
                {
                    serviceInjection.OnServiceInit();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Bootstrap()
        {
            Startup();
            Instance.Load();
        }

        public static void Startup()
        {
            ServiceProvider.RegisterInjection<IConfigInjection, UnityConfigInjection>(ServiceProvider.UNITY_ASSEMBLY);
        }
    }
}