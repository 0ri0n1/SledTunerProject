using MelonLoader;
using UnityEngine;
using SledTunerProject.SledParameterSystem;
using SledTunerProject.ConfigSystem;
using SledTunerProject.GUISystem;

[assembly: MelonInfo(typeof(SledTunerProject.Main), "SledTuner", "1.0.0", "YourName")]
[assembly: MelonGame("Hanki Games", "Sledders")]

namespace SledTunerProject
{
    public class Main : MelonMod
    {
        private GUIManager _guiManager;

        private SledParameterManager _sledParameterManager;
        private ConfigManager _configManager;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("[Main] SledTuner initializing...");

            _sledParameterManager = new SledParameterManager();
            _sledParameterManager.InitializeComponents();


            _configManager = new ConfigManager(_sledParameterManager);

            _guiManager = new GUIManager(_sledParameterManager, _configManager);
        }

        public override void OnUpdate()
        {
            // Press F2 to toggle
            if (Input.GetKeyDown(KeyCode.F2))
            {
                _guiManager.ToggleMenu();
            }
        }

        public override void OnGUI()
        {
            // Draw the mod's IMGUI
            _guiManager.DrawMenu();
        }
    }
}
