using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ThePattern.Unity
{
    /// <summary>
    /// Observer Pattern
    /// </summary>
    public class EventHub : Singleton<EventHub>
    {

        private Dictionary<string, List<Action<object>>> _EventMap = new Dictionary<string, List<Action<object>>>();
        /// <summary>
        /// Call it when you need Register a function to Listen a 1 event
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="callback"></param>
        public void RegisterEvent(string eventName, Action<object> callback)
        {
            if (!_EventMap.ContainsKey(eventName))
            {
                _EventMap.Add(eventName, new List<Action<object>>());
            }
            List<Action<object>> listAction = _EventMap[eventName];
            listAction.Add(callback);
        }

        /// <summary>
        /// Call it when you need Remove your listener 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="callback"></param>
        public void RemoveEvent(string eventName, Action<object> callback)
        {
            if (_EventMap.ContainsKey(eventName))
            {
                List<Action<object>> listAction = _EventMap[eventName];
                listAction.Remove(callback);
            }
        }

        /// <summary>
        /// Call it when you need a listener listen your event
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        public void UpdateEvent(string eventName, object data)
        {
            if (_EventMap.ContainsKey(eventName))
            {
                List<Action<object>> listAction = _EventMap[eventName];
                foreach (Action<object> callback in listAction)
                {
                    callback(data);
                }
            }
        }
    }
}