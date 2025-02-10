using SledTunerMod.Parameters;
using System;
using UnityEngine;

namespace SledTunerProject.SledParameterSystem
{
    /// <summary>
    /// The orchestrator for all parameter operations.
    /// Communicates with the ParameterStore, ParameterLoader, Validator, Updater, etc.
    /// </summary>
    public class SledParameterManager
    {
        private ParameterStore _parameterStore;
        private ParameterLoader _parameterLoader;
        private ParameterValidator _validator;
        private ParameterUpdater _updater;
        private UndoRedoManager _undoRedoManager;
        private ParameterPresetManager _presetManager;
        private EventDispatcher _eventDispatcher;

        /// <summary>
        /// Indicates whether parameters are initialized and ready.
        /// </summary>
        public bool IsInitialized { get; private set; }

        // Example: toggle to enable/disable live updates to the game.
        private bool _liveUpdatesEnabled = true;

        public SledParameterManager()
        {
            _parameterStore = new ParameterStore();
            _parameterLoader = new ParameterLoader();
            _validator = new ParameterValidator();
            _updater = new ParameterUpdater();
            _undoRedoManager = new UndoRedoManager();
            _presetManager = new ParameterPresetManager();
            _eventDispatcher = new EventDispatcher();
        }

        /// <summary>
        /// Called at mod startup or when ready to initialize default/last-used config.
        /// </summary>
        public void InitializeParameters()
        {
            // You could load a default JSON config here, or just set some default parameters:
            // e.g., LoadConfig("DefaultConfig.json");
            // or set up a blank state, then mark IsInitialized = true.
            IsInitialized = true;
            Debug.Log("[SledParameterManager] Parameters initialized.");
        }

        /// <summary>
        /// Set a parameter value, optionally validated/clamped before committing.
        /// Records undo/redo, triggers event dispatch, updates the game if live updates are on.
        /// </summary>
        public void SetParameterValue(string paramID, float newValue)
        {
            // 1. Get old value
            float oldValue = _parameterStore.GetParameterValue(paramID);

            // 2. Validate/clamp
            float validValue = _validator.Clamp(paramID, newValue);

            // 3. If changed, record undo and store new value
            if (Math.Abs(validValue - oldValue) > 1e-6)
            {
                _undoRedoManager.RecordChange(paramID, oldValue, validValue);
                _parameterStore.SetParameterValue(paramID, validValue);

                // 4. Dispatch an event for GUI or others
                _eventDispatcher.Dispatch("OnParameterChanged", paramID, validValue);

                // 5. If live updates are on, push to the game
                if (_liveUpdatesEnabled)
                {
                    _updater.UpdateGameObject(paramID, validValue);
                }
            }
        }

        /// <summary>
        /// Retrieves a parameter value from the store.
        /// </summary>
        public float GetParameterValue(string paramID)
        {
            return _parameterStore.GetParameterValue(paramID);
        }

        /// <summary>
        /// Loads a config from a file path, merges or replaces store data, and applies.
        /// </summary>
        public void LoadConfig(string filePath)
        {
            var data = _parameterLoader.LoadFromJSON(filePath);
            // Apply the data to the store, or merge:
            _parameterLoader.ApplyToParameterStore(_parameterStore, data);

            Debug.Log($"[SledParameterManager] Loaded config from: {filePath}");
            ApplyAllParameters();
        }

        /// <summary>
        /// Applies all parameters in the store to the game in one go (useful for batch updates).
        /// </summary>
        public void ApplyAllParameters()
        {
            // If live updates are disabled, do a full sync
            foreach (var param in _parameterStore.GetAllParameters())
            {
                _updater.UpdateGameObject(param.ID, param.CurrentValue);
            }
        }

        /// <summary>
        /// Toggles live updating of parameters as they change.
        /// </summary>
        public void ToggleLiveUpdates(bool enable)
        {
            _liveUpdatesEnabled = enable;
        }

        /// <summary>
        /// Exposes the undo operation.
        /// </summary>
        public void UndoLastChange()
        {
            var change = _undoRedoManager.UndoLastChange();
            if (change != null)
            {
                _parameterStore.SetParameterValue(change.ParamID, change.OldValue);
                if (_liveUpdatesEnabled)
                {
                    _updater.UpdateGameObject(change.ParamID, change.OldValue);
                }
                _eventDispatcher.Dispatch("OnParameterChanged", change.ParamID, change.OldValue);
            }
        }

        /// <summary>
        /// Exposes the redo operation.
        /// </summary>
        public void RedoLastUndoneChange()
        {
            var change = _undoRedoManager.RedoLastUndoneChange();
            if (change != null)
            {
                _parameterStore.SetParameterValue(change.ParamID, change.NewValue);
                if (_liveUpdatesEnabled)
                {
                    _updater.UpdateGameObject(change.ParamID, change.NewValue);
                }
                _eventDispatcher.Dispatch("OnParameterChanged", change.ParamID, change.NewValue);
            }
        }

        /// <summary>
        /// Offers preset loading and saving, deferring to ParameterPresetManager.
        /// </summary>
        public void LoadPreset(string presetName)
        {
            var data = _presetManager.LoadPreset(presetName);
            _parameterLoader.ApplyToParameterStore(_parameterStore, data);
            ApplyAllParameters();
            Debug.Log($"[SledParameterManager] Preset '{presetName}' loaded.");
        }

        public void SavePreset(string presetName)
        {
            var currentData = _parameterStore.GetAllParameterData();
            _presetManager.SavePreset(presetName, currentData);
            Debug.Log($"[SledParameterManager] Preset '{presetName}' saved.");
        }

        public EventDispatcher GetEventDispatcher()
        {
            return _eventDispatcher;
        }
    }
}
