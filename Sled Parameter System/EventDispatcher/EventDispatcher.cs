using System;
using System.Collections.Generic;
using UnityEngine;

namespace SledTunerProject.SledParameterSystem
{
    /// <summary>
    /// A simple pub/sub system for events like "OnParameterChanged".
    /// Avoids tight coupling between SledParameterManager and the GUI or other systems.
    /// </summary>
    public class EventDispatcher
    {
        // eventName -> list of listeners
        private Dictionary<string, List<Action<string, float>>> _floatEventMap;

        public EventDispatcher()
        {
            _floatEventMap = new Dictionary<string, List<Action<string, float>>>();
        }

        /// <summary>
        /// Subscribe to an event that passes (paramID, newValue).
        /// </summary>
        public void Subscribe(string eventName, Action<string, float> callback)
        {
            if (!_floatEventMap.ContainsKey(eventName))
            {
                _floatEventMap[eventName] = new List<Action<string, float>>();
            }
            _floatEventMap[eventName].Add(callback);
        }

        /// <summary>
        /// Dispatch an event with paramID and float value.
        /// </summary>
        public void Dispatch(string eventName, string paramID, float newValue)
        {
            if (!_floatEventMap.ContainsKey(eventName)) return;

            foreach (var callback in _floatEventMap[eventName])
            {
                try
                {
                    callback?.Invoke(paramID, newValue);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventDispatcher] Error in '{eventName}' callback: {ex.Message}");
                }
            }
        }

        public void Unsubscribe(string eventName, Action<string, float> callback)
        {
            if (!_floatEventMap.ContainsKey(eventName)) return;
            _floatEventMap[eventName].Remove(callback);
        }
    }
}
