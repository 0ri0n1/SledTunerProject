using UnityEngine;
using System.Collections.Generic;

namespace SledTunerProject.GUISystem
{
    /// <summary>
    /// Responsible for displaying and interacting with individual parameters (sliders, toggles, text fields).
    /// Immediately communicates changes back to the SledParameterManager.
    /// </summary>
    public class ParameterEditor
    {
        // For instance, we could store a dictionary of parameter IDs -> current values
        private Dictionary<string, object> _parameterValues = new Dictionary<string, object>();

        public ParameterEditor()
        {
            // Potentially load from a ParameterStore or SledParameterManager
        }

        public void RenderParameterFields(string category)
        {
            // In a real system, you might query SledParameterManager for parameters under this category
            // Then generate sliders, toggles, etc., for each one
            GUILayout.Label($"Rendering parameters for category: {category}");
        }

        /// <summary>
        /// Called by the GUI manager or event system when a parameter value changes.
        /// </summary>
        public void OnParameterValueChanged(string paramID, object newValue)
        {
            if (_parameterValues.ContainsKey(paramID))
                _parameterValues[paramID] = newValue;
            else
                _parameterValues.Add(paramID, newValue);

            // Then push to the SledParameterManager if needed
            Debug.Log($"[ParameterEditor] Param '{paramID}' changed to {newValue}");
        }
    }
}
