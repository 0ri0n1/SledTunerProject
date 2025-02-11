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
    /// The "advanced" approach: reflection-based fields, color channels for Light, etc.
    /// Derived from your old code, but in a smaller class.
    /// </summary>
    public class AdvancedTunerPanel
    {
        private SledParameterManager _sledParam;
        private ConfigManager _configManager;

        // Reflection dictionaries
        private Dictionary<string, Dictionary<string, string>> _fieldInputs;
        private Dictionary<string, Dictionary<string, double>> _fieldPreview;

        // For minus/plus detection
        private HashSet<string> _minusHeldNow;
        private HashSet<string> _minusHeldPrev;
        private HashSet<string> _plusHeldNow;
        private HashSet<string> _plusHeldPrev;

        private bool _pendingCommit;
        private float _lastCommitTime;
        private const float _commitDelay = 0.2f;

        private Dictionary<string, bool> _foldoutStates;
        private Vector2 _scrollPos = Vector2.zero;

        private bool _treeViewEnabled = true; // example toggle
        private bool _manualApply = true;

        private Texture2D _colorPreviewTexture;

        public AdvancedTunerPanel(SledParameterManager spm, ConfigManager cfg)
        {
            _sledParam = spm;
            _configManager = cfg;

            // Just init them here to avoid null issues
            _fieldInputs = new Dictionary<string, Dictionary<string, string>>();
            _fieldPreview = new Dictionary<string, Dictionary<string, double>>();
            _minusHeldNow = new HashSet<string>();
            _minusHeldPrev = new HashSet<string>();
            _plusHeldNow = new HashSet<string>();
            _plusHeldPrev = new HashSet<string>();
            _foldoutStates = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Refresh reflection data from SledParameterManager.
        /// Called whenever you open the menu or reload config.
        /// </summary>
        public void RePopulateFields()
        {
            _fieldInputs.Clear();
            _fieldPreview.Clear();

            var currentParams = _sledParam.GetCurrentParameters();
            foreach (var compEntry in _sledParam.ComponentsToInspect)
            {
                string compName = compEntry.Key;
                _fieldInputs[compName] = new Dictionary<string, string>();
                _fieldPreview[compName] = new Dictionary<string, double>();

                foreach (string field in compEntry.Value)
                {
                    object val = _sledParam.GetFieldValue(compName, field);
                    string valStr = (val != null) ? val.ToString() : "(No data)";
                    _fieldInputs[compName][field] = valStr;

                    double numericVal = 0.0;
                    if (val is double dd) numericVal = dd;
                    else if (val is float ff) numericVal = ff;
                    else if (val is int ii) numericVal = ii;
                    else
                    {
                        if (double.TryParse(valStr, out double tryD)) numericVal = tryD;
                    }
                    _fieldPreview[compName][field] = numericVal;
                }
            }

            // ensure foldout states
            foreach (var comp in _fieldInputs.Keys)
            {
                if (!_foldoutStates.ContainsKey(comp))
                    _foldoutStates[comp] = true;
            }
            MelonLogger.Msg("[AdvancedTunerPanel] Fields repopulated.");
        }

        public void DrawAdvancedPanel()
        {
            // Handle minus/plus sets
            _minusHeldPrev = new HashSet<string>(_minusHeldNow);
            _plusHeldPrev = new HashSet<string>(_plusHeldNow);
            _minusHeldNow.Clear();
            _plusHeldNow.Clear();

            GUILayout.BeginHorizontal();
            _manualApply = GUILayout.Toggle(_manualApply, "Manual Apply", GUILayout.Width(100));
            _treeViewEnabled = GUILayout.Toggle(_treeViewEnabled, "Tree View", GUILayout.Width(80));
            if (GUILayout.Button("ApplyAll", GUILayout.Width(70)))
            {
                CommitAllReflectionChanges();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            if (_treeViewEnabled)
                DrawTreeViewParameters();
            else
                DrawFlatParameters();
            GUILayout.EndScrollView();

            // detect button releases for minus/plus
            if (Event.current.type == EventType.Repaint)
            {
                DetectButtonReleases();
            }

            // if commit is pending
            if (_pendingCommit && (Time.realtimeSinceStartup - _lastCommitTime >= _commitDelay))
            {
                CommitAllReflectionChanges();
                _pendingCommit = false;
            }
        }

        private void DrawFlatParameters()
        {
            foreach (var comp in _fieldInputs)
            {
                GUILayout.Label($"Component: {comp.Key}");
                if (comp.Key == "Light")
                {
                    DrawLightColorChannels(true);
                }
                else
                {
                    DrawReflectionParameters(comp.Key, comp.Value);
                }
                GUILayout.Space(5);
            }
        }

        private void DrawTreeViewParameters()
        {
            foreach (var comp in _fieldInputs)
            {
                if (!_foldoutStates.ContainsKey(comp.Key))
                    _foldoutStates[comp.Key] = true;

                _foldoutStates[comp.Key] = GUILayout.Toggle(_foldoutStates[comp.Key], $"<b>{comp.Key}</b>", GUILayout.ExpandWidth(false));
                if (_foldoutStates[comp.Key])
                {
                    GUILayout.BeginVertical("box");
                    if (comp.Key == "Light")
                    {
                        DrawLightColorChannels(true);
                    }
                    else
                    {
                        DrawReflectionParameters(comp.Key, comp.Value);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.Space(5);
            }
        }

        private void DrawLightColorChannels(bool isAdvanced)
        {
            if (!_fieldInputs.ContainsKey("Light"))
                return;

            string[] channels = { "r", "g", "b", "a" };
            foreach (var ch in channels)
            {
                if (!_fieldInputs["Light"].ContainsKey(ch)) continue;

                float.TryParse(_fieldInputs["Light"][ch], out float curVal);
                float newVal = curVal;

                GUILayout.BeginHorizontal();
                GUILayout.Label($"{ch.ToUpper()}: ", GUILayout.Width(40));
                newVal = GUILayout.HorizontalSlider(newVal, 0f, 1f, GUILayout.Width(150));
                string txtVal = GUILayout.TextField(newVal.ToString("F2"), GUILayout.Width(50));
                if (float.TryParse(txtVal, out float parsed)) newVal = parsed;

                string minusKey = "Light." + ch + ".minus";
                bool minusHeld = GUILayout.RepeatButton("-", GUILayout.Width(25));
                if (minusHeld) _minusHeldNow.Add(minusKey);

                string plusKey = "Light." + ch + ".plus";
                bool plusHeld = GUILayout.RepeatButton("+", GUILayout.Width(25));
                if (plusHeld) _plusHeldNow.Add(plusKey);

                GUILayout.EndHorizontal();

                if (Event.current.type == EventType.Repaint)
                {
                    if (_minusHeldNow.Contains(minusKey))
                        newVal = Mathf.Max(0f, newVal - 0.01f);
                    if (_plusHeldNow.Contains(plusKey))
                        newVal = Mathf.Min(1f, newVal + 0.01f);
                }

                if (Mathf.Abs(newVal - curVal) > 0.0001f)
                {
                    _fieldInputs["Light"][ch] = newVal.ToString("F2");
                    if (isAdvanced && _fieldPreview.ContainsKey("Light") && _fieldPreview["Light"].ContainsKey(ch))
                        _fieldPreview["Light"][ch] = newVal;
                }
            }
            // optionally show color preview
        }

        private void DrawReflectionParameters(string compName, Dictionary<string, string> fields)
        {
            foreach (var kvp in fields)
            {
                string fieldName = kvp.Key;
                // skip color channels if Light => handled above
                if (compName == "Light" && (fieldName == "r" || fieldName == "g" || fieldName == "b" || fieldName == "a"))
                    continue;

                GUILayout.BeginHorizontal();
                //
                // MARKER TO KNOW WHERE TO UNCOMMENT LABEL IF NEEDED
                GUILayout.Label($"{fieldName}:", GUILayout.Width(160));

                double currentVal = _fieldPreview[compName][fieldName];
                Type fieldType = _sledParam.GetFieldType(compName, fieldName);

                if (fieldType == typeof(float) || fieldType == typeof(double) || fieldType == typeof(int))
                {
                    GUILayout.EndHorizontal(); // close row
                    DrawNumericField(compName, fieldName, currentVal);
                }
                else if (fieldType == typeof(bool))
                {
                    bool.TryParse(_fieldInputs[compName][fieldName], out bool curVal);
                    bool newVal = GUILayout.Toggle(curVal, curVal ? "On" : "Off", GUILayout.Width(80));
                    if (newVal != curVal)
                    {
                        _fieldInputs[compName][fieldName] = newVal.ToString();
                        if (!_manualApply)
                        {
                            _sledParam.SetFieldValue(compName, fieldName, newVal);
                            _sledParam.ApplyParameters();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    string newStr = GUILayout.TextField(_fieldInputs[compName][fieldName], GUILayout.Width(100));
                    if (newStr != _fieldInputs[compName][fieldName])
                    {
                        _fieldInputs[compName][fieldName] = newStr;
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }

        private void DrawNumericField(string compName, string fieldName, double currentVal)
        {
            float valF = (float)currentVal;
            GUILayout.BeginHorizontal();
            //
            // MARKER TO KNOW WHERE TO UNCOMMENT LABEL IF NEEDED
            //GUILayout.Label($"{fieldName}:", GUILayout.Width(125));
            valF = GUILayout.HorizontalSlider(valF, -100000f, 100000f, GUILayout.Width(150)); // or get actual min/max from store
            string textVal = GUILayout.TextField(valF.ToString("F2"), GUILayout.Width(60));
            if (float.TryParse(textVal, out float parsed))
                valF = parsed;

            // minus/plus
            string minusKey = compName + "." + fieldName + ".minus";
            bool minusHeld = GUILayout.RepeatButton("-", GUILayout.Width(25));
            if (minusHeld) _minusHeldNow.Add(minusKey);

            string plusKey = compName + "." + fieldName + ".plus";
            bool plusHeld = GUILayout.RepeatButton("+", GUILayout.Width(25));
            if (plusHeld) _plusHeldNow.Add(plusKey);

            GUILayout.EndHorizontal();

            if (Event.current.type == EventType.Repaint)
            {
                if (_minusHeldNow.Contains(minusKey))
                    valF -= 0.01f;
                if (_plusHeldNow.Contains(plusKey))
                    valF += 0.01f;
            }

            if (Math.Abs(valF - currentVal) > 0.0001)
            {
                _fieldPreview[compName][fieldName] = valF;
                _fieldInputs[compName][fieldName] = valF.ToString("F2");
            }
        }

        private void DetectButtonReleases()
        {
            bool commitNeeded = false;
            foreach (var key in _minusHeldPrev)
            {
                if (!_minusHeldNow.Contains(key)) { commitNeeded = true; break; }
            }
            if (!commitNeeded)
            {
                foreach (var key in _plusHeldPrev)
                {
                    if (!_plusHeldNow.Contains(key)) { commitNeeded = true; break; }
                }
            }
            if (commitNeeded)
            {
                _pendingCommit = true;
                _lastCommitTime = Time.realtimeSinceStartup;
                MelonLogger.Msg("[AdvancedTunerPanel] commit pending (debounce).");
            }
        }

        private void CommitAllReflectionChanges()
        {
            MelonLogger.Msg("[AdvancedTunerPanel] Committing reflection changes now.");
            foreach (var compKvp in _fieldPreview)
            {
                string compName = compKvp.Key;
                foreach (var fieldKvp in compKvp.Value)
                {
                    string fieldName = fieldKvp.Key;
                    double val = fieldKvp.Value;
                    Type ft = _sledParam.GetFieldType(compName, fieldName);

                    if (ft == typeof(float) || ft == typeof(double) || ft == typeof(int))
                    {
                        _sledParam.SetFieldValue(compName, fieldName, val);
                    }
                    else if (ft == typeof(bool))
                    {
                        bool.TryParse(_fieldInputs[compName][fieldName], out bool b);
                        _sledParam.SetFieldValue(compName, fieldName, b);
                    }
                    else if (compName == "Light" && (fieldName == "r" || fieldName == "g" || fieldName == "b" || fieldName == "a"))
                    {
                        if (_fieldInputs[compName].TryGetValue(fieldName, out string cStr))
                        {
                            if (float.TryParse(cStr, out float cVal))
                                _sledParam.SetFieldValue(compName, fieldName, cVal);
                        }
                    }
                }
            }
            _sledParam.ApplyParameters();
        }
    }
}
