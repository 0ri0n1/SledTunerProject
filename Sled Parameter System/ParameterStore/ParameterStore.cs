using System.Collections.Generic;
using UnityEngine;

namespace SledTunerProject.SledParameterSystem
{
    /// <summary>
    /// Holds all parameters, including current values, defaults, min/max ranges, etc.
    /// </summary>
    public class ParameterStore
    {
        private Dictionary<string, Parameter> _parameters;

        public ParameterStore()
        {
            _parameters = new Dictionary<string, Parameter>();
        }

        /// <summary>
        /// Retrieves the float value of a parameter by ID.
        /// Returns 0 if not found.
        /// </summary>
        public float GetParameterValue(string paramID)
        {
            if (_parameters.ContainsKey(paramID))
                return _parameters[paramID].CurrentValue;

            return 0f; // or handle not found differently
        }

        /// <summary>
        /// Sets or updates a parameter with a new value. If it doesn't exist, create it.
        /// </summary>
        public void SetParameterValue(string paramID, float newValue)
        {
            if (!_parameters.ContainsKey(paramID))
            {
                // Create a default Parameter object here, or fetch metadata from somewhere.
                _parameters[paramID] = new Parameter
                {
                    ID = paramID,
                    DefaultValue = 0f,
                    MinValue = -999999f,
                    MaxValue = 999999f,
                    CurrentValue = newValue,
                    Type = ParameterType.Float
                };
            }
            else
            {
                _parameters[paramID].CurrentValue = newValue;
            }
        }

        /// <summary>
        /// Returns a collection of all parameters for iteration.
        /// </summary>
        public IEnumerable<Parameter> GetAllParameters()
        {
            return _parameters.Values;
        }

        /// <summary>
        /// Returns a dictionary (paramID -> float) representing the current values.
        /// Useful for saving or preset exporting.
        /// </summary>
        public Dictionary<string, float> GetAllParameterData()
        {
            var result = new Dictionary<string, float>();
            foreach (var kvp in _parameters)
            {
                result[kvp.Key] = kvp.Value.CurrentValue;
            }
            return result;
        }

        /// <summary>
        /// Resets all parameters to default values.
        /// </summary>
        public void ResetAllParametersToDefault()
        {
            foreach (var param in _parameters.Values)
            {
                param.CurrentValue = param.DefaultValue;
            }
            Debug.Log("[ParameterStore] All parameters reset to default.");
        }
    }

    /// <summary>
    /// Example parameter structure.
    /// </summary>
    public class Parameter
    {
        public string ID;
        public float CurrentValue;
        public float DefaultValue;
        public float MinValue;
        public float MaxValue;
        public ParameterType Type;
    }

    public enum ParameterType
    {
        Float,
        Int,
        Bool,
        Enum
        // etc.
    }
}
