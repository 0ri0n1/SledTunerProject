using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using MelonLoader;
using SledTunerProject.SledParameterSystem;

namespace SledTunerProject.GUISystem
{
    /// <summary>
    /// The "simple" approach: local fields like speed, gravity, power, etc.
    /// Not reflection-based. Quick easy editing.
    /// </summary>
    public class SimpleTunerPanel
    {
        private SledParameterManager _sledParam;

        // Local fields
        private float _speed = 10f;
        private float _gravity = -9.81f;

        public SimpleTunerPanel(SledParameterManager spm)
        {
            _sledParam = spm;
        }

        public void DrawSimplePanel()
        {
            GUILayout.Label("=== Simple Tuner ===");
            GUILayout.Space(5);

            // speed
            GUILayout.Label($"Speed: {_speed:F2}");
            _speed = GUILayout.HorizontalSlider(_speed, 0f, 200f, GUILayout.Width(150));
            _speed = FloatField(_speed);

            // gravity
            GUILayout.Label($"Gravity: {_gravity:F2}");
            _gravity = GUILayout.HorizontalSlider(_gravity, -20f, 0f, GUILayout.Width(150));
            _gravity = FloatField(_gravity);

            // Apply button if you want to push to SledParameterManager
            if (GUILayout.Button("Apply", GUILayout.Width(60)))
            {
                // _sledParam.SetParameterValue("speed", _speed);
                // _sledParam.SetParameterValue("gravity", _gravity);
                // _sledParam.ApplyParameters();
                MelonLogger.Msg("[SimpleTunerPanel] Apply clicked (placeholder).");
            }
        }

        private float FloatField(float value)
        {
            string txt = GUILayout.TextField(value.ToString("F2"), GUILayout.Width(60));
            if (float.TryParse(txt, out float parsed))
            {
                value = parsed;
            }
            return value;
        }
    }
}
