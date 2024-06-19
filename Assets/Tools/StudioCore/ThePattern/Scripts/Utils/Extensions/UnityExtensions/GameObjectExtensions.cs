using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePattern.Extensions
{
    /// <summary>
    /// Extension Method For GameObject
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Set Parent for GameObject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="parent">parent</param>
        public static void SetParent(this GameObject gameObject, Transform parent)
        {
            gameObject.SetParent(parent, Vector3.zero, Vector3.one);
        }
        /// <summary>
        /// Set Parent for GameObject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="parent">parent</param>
        /// <param name="position">local position</param>
        public static void SetParent(this GameObject gameObject, Transform parent, Vector3 position)
        {
            gameObject.SetParent(parent, position, Vector3.one);
        }
        /// <summary>
        /// Set Parent for GameObject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="parent">parent</param>
        /// <param name="position">local position</param>
        /// <param name="scale">local scale</param>
        public static void SetParent(this GameObject gameObject, Transform parent, Vector3 position, Vector3 scale)
        {
            gameObject.transform.SetParent(parent);
            gameObject.transform.localPosition = position;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = scale;
        }
        /// <summary>
        /// Try to Get Component
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns>Component</returns>
        public static T AddOrGetComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
        public static void DestroyAllChilren(this Transform parent)
        {
            while (parent.childCount > 0)
                Object.DestroyImmediate(parent.GetChild(0).gameObject);
        }
        public static void DestroyAllChilren(this GameObject parent)
        {
            DestroyAllChilren(parent.transform);
        }
    }
}