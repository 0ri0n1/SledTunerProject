using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using MelonLoader;
using SledTunerProject.SledParameterSystem;
using SledTunerProject.ConfigSystem;

namespace SledTunerProject.GUISystem
{
    /// <summary>
    /// A small top bar for Load/Save/Reset. Called from GUIManager.
    /// </summary>
    public class MenuBarPanel
    {
        private ConfigManager _configManager;
        private SledParameterManager _sledParamManager;

        public MenuBarPanel(ConfigManager cfg, SledParameterManager spm)
        {
            _configManager = cfg;
            _sledParamManager = spm;
        }

        public void DrawMenuBar()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load", GUILayout.Width(50)))
            {
                _configManager.LoadConfiguration();
                MelonLogger.Msg("[MenuBarPanel] Load clicked.");
            }
            if (GUILayout.Button("Save", GUILayout.Width(50)))
            {
                _configManager.SaveConfiguration();
                MelonLogger.Msg("[MenuBarPanel] Save clicked.");
            }
            if (GUILayout.Button("Reset", GUILayout.Width(50)))
            {
                _configManager.ResetParameters();
                _sledParamManager.RevertParameters();
                MelonLogger.Msg("[MenuBarPanel] Reset clicked.");
            }
            GUILayout.EndHorizontal();
        }
    }
}
