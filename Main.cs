using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using HarmonyLib;
using SledTunerProject.ConfigSystem;
using SledTunerProject.GUISystem;
using SledTunerProject.SledParameterSystem;
using SledTunerMod.Config;
using SledTunerMod.GUI;
using SledTunerMod.Parameters;

[assembly: MelonInfo(typeof(SledTunerProject.Main), "SledTunerMod", "1.0.0", "YourName")]
[assembly: MelonGame("Hanki Games", "Sledders")]

namespace SledTunerProject
{
    public class Main : MelonMod
    {
        // Core systems
        private SledParameterManager _sledParameterManager;
        private ConfigManager _configManager;
        private GUIManager _guiManager;

        // Use fully-qualified type to avoid confusion with Harmony.
        private HarmonyLib.Harmony _harmony;

        // Scenes that auto-initialize the sled
        private readonly HashSet<string> _validScenes = new HashSet<string>
        {
            "Woodland",
            "Side_Cliffs_03_27",
            "Idaho",
            "Rocky Mountains",
            "Valley"
        };

        private bool _initialized = false;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("[SledTuner] OnInitializeMelon => Setting up the mod...");

            // Initialize Harmony and patch
            _harmony = new HarmonyLib.Harmony("your.unique.modid.sledtuner");
            _harmony.PatchAll();
            MelonLogger.Msg("[SledTuner] Harmony patches applied.");

            // Create the new system objects
            _sledParameterManager = new SledParameterManager();
            _configManager = new ConfigManager(_sledParameterManager);
            _guiManager = new GUIManager(_sledParameterManager, _configManager);

            SceneManager.sceneLoaded += OnSceneWasLoaded;

            MelonLogger.Msg("[SledTuner] Mod setup complete. Use F2 to toggle GUI, F3 to force re-init.");
        }

        private void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
        {
            MelonLogger.Msg($"[SledTuner] OnSceneWasLoaded: {scene.name}");

            if (_validScenes.Contains(scene.name))
            {
                MelonLogger.Msg($"[SledTuner] Scene '{scene.name}' is valid. Auto-initializing sled...");
                TryInitializeSled();
            }
            else
            {
                MelonLogger.Msg($"[SledTuner] Scene '{scene.name}' not recognized as valid. Marking uninitialized.");
                _initialized = false;
            }
        }

        private void TryInitializeSled()
        {
            if (_initialized)
            {
                MelonLogger.Msg("[SledTuner] Sled is already initialized, skipping re-init.");
                return;
            }

            // Attempt to initialize
            _sledParameterManager.InitializeComponents();
            if (_sledParameterManager.IsInitialized)
            {
                _guiManager.RebuildUI();  // e.g., refresh GUI fields
                _initialized = true;
                MelonLogger.Msg("[SledTuner] SledParameterManager reports IsInitialized = TRUE.");
            }
            else
            {
                MelonLogger.Warning("[SledTuner] SledParameterManager.IsInitialized = FALSE, something failed.");
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                MelonLogger.Msg("[SledTuner] F2 pressed => Toggle GUI");
                _guiManager.ToggleGUI();

                if (!_initialized)
                {
                    // If sled wasn't auto-initialized yet but user wants the GUI, try again
                    TryInitializeSled();
                }
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                MelonLogger.Msg("[SledTuner] F3 pressed => Forcing re-initialization.");
                _initialized = false;
                TryInitializeSled();
            }
        }

        // Forward OnGUI calls to our GUI system
        public override void OnGUI()
        {
            _guiManager?.OnGUI();
        }
    }
}
