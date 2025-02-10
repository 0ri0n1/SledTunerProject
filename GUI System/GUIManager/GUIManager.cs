using UnityEngine;
using System;
using SledTunerMod.GUI;

namespace SledTunerProject.GUISystem
{
    /// <summary>
    /// Main orchestrator for the in-game GUI.
    /// Handles high-level events and coordinates sub-panels.
    /// </summary>
    public class GUIManager : MonoBehaviour
    {
        // References to sub-managers in the GUI System
        private MenuBarManager _menuBarManager;
        private TabPanelManager _tabPanelManager;
        private JSONEditorPanel _jsonEditorPanel;
        private FileSelectionPanel _fileSelectionPanel;
        private ParameterEditor _parameterEditor;
        private UtilityControls _utilityControls;
        private SearchManager _searchManager;
        private ValidationManager _validationManager;
        private GUIPresetManager _guiPresetManager;

        // Example: A simple UI toggle for showing/hiding the entire GUI.
        private bool _isGUIVisible = false;

        private void Awake()
        {
            // Optional: Initialize or find references to sub-managers if they are separate GameObjects.
            // Or create them programmatically:
            // _menuBarManager = gameObject.AddComponent<MenuBarManager>();
            // ... etc.
        }

        public void InitializeGUI()
        {
            // Example: Create or setup sub-panels. In practice, you might load from prefabs or unify under one canvas.
            _menuBarManager = new MenuBarManager();
            _tabPanelManager = new TabPanelManager();
            _jsonEditorPanel = new JSONEditorPanel();
            _fileSelectionPanel = new FileSelectionPanel();
            _parameterEditor = new ParameterEditor();
            _utilityControls = new UtilityControls();
            _searchManager = new SearchManager();
            _validationManager = new ValidationManager();
            _guiPresetManager = new GUIPresetManager();

            // Example usage: sub-managers might need references to each other or external managers:
            // _menuBarManager.SetGUIManager(this);
            // _menuBarManager.SetConfigManager(...);
            // ...
        }

        /// <summary>
        /// Called by SledParameterManager (or an event) when a parameter changes.
        /// </summary>
        /// <param name="parameterID">Unique ID of the parameter.</param>
        /// <param name="newValue">New value after change.</param>
        public void OnParameterChanged(string parameterID, object newValue)
        {
            // Refresh relevant UI fields in the ParameterEditor or other sub-panels.
            Debug.Log($"[GUIManager] Parameter '{parameterID}' changed to: {newValue}");
            // e.g., _parameterEditor.UpdateParameterField(parameterID, newValue);
        }

        /// <summary>
        /// Displays a message to the user (could be a popup, status bar message, etc.).
        /// </summary>
        public void ShowMessage(string message, MessageType messageType = MessageType.Info)
        {
            // Implementation depends on your UI approach (popups, toast, label, etc.)
            Debug.Log($"[GUIManager] ShowMessage: {messageType} - {message}");
        }

        /// <summary>
        /// Simple toggle to show/hide the entire GUI.
        /// </summary>
        public void ToggleGUIVisibility()
        {
            _isGUIVisible = !_isGUIVisible;
        }

        // If using IMGUI, OnGUI could be placed here (or in sub-panels).
        // If using Unity’s modern UI (uGUI), you’ll have separate Canvas/RectTransforms.
        private void OnGUI()
        {
            if (!_isGUIVisible)
                return;

            // Example: minimal IMGUI usage
            GUILayout.Label("Sled Tuner Main GUI");
            if (GUILayout.Button("Menu Bar")) { /* Show/Hide menu bar or forward to MenuBarManager */ }
            if (GUILayout.Button("Parameter Editor")) { /* Show Parameter Editor panel, etc. */ }
        }
    }

    /// <summary>
    /// Common message types for user feedback.
    /// </summary>
    public enum MessageType
    {
        Info,
        Warning,
        Error
    }
}
