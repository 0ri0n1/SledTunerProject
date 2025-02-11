using UnityEngine;
using MelonLoader;
using SledTunerProject.SledParameterSystem;
using SledTunerProject.ConfigSystem;

namespace SledTunerProject.GUISystem
{
    /// <summary>
    /// Top-level orchestrator that opens/closes the tuner window,
    /// draws sub-panels (MenuBarPanel, SimpleTunerPanel, AdvancedTunerPanel)
    /// in a single IMGUI window, and handles window resizing.
    /// </summary>
    public class GUIManager
    {
        // External references
        private SledParameterManager _sledParamManager;
        private ConfigManager _configManager;

        // Sub-panels
        private MenuBarPanel _menuBar;
        private SimpleTunerPanel _simplePanel;
        private AdvancedTunerPanel _advancedPanel;

        // Window / IMGUI state
        private bool _menuOpen = false;
        private Rect _windowRect;
        private bool _isResizing = false;
        private bool _isMinimized = false;
        private Rect _prevWindowRect;
        private Vector2 _resizeStartMousePos;
        private Rect _resizeStartWindowRect;

        // Toggle advanced vs. simple
        private bool _showAdvanced = false;

        // Optional alpha for the window's GUI
        private float _opacity = 1f;

        // Styles for top-bar buttons and title
        private GUIStyle _titleBarButtonStyle;
        private GUIStyle _headerStyle;

        // NEW: style for removing default IMGUI window padding at the top
        private GUIStyle _customWindowStyle;

        public GUIManager(SledParameterManager spm, ConfigManager cfg)
        {
            _sledParamManager = spm;
            _configManager = cfg;

            // Create sub-panels
            _menuBar = new MenuBarPanel(_configManager, _sledParamManager);
            _simplePanel = new SimpleTunerPanel(_sledParamManager);
            _advancedPanel = new AdvancedTunerPanel(_sledParamManager, _configManager);

            // Default window rect
            _windowRect = new Rect(
                Screen.width * 0.2f,
                Screen.height * 0.2f,
                Screen.width * 0.9f,
                Screen.height * 0.9f
            );

            MelonLogger.Msg("[GUIManager] Created with 3 sub-panels (MenuBar, Simple, Advanced).");
        }

        public void ToggleMenu()
        {
            _menuOpen = !_menuOpen;
            if (_menuOpen)
            {
                _advancedPanel.RePopulateFields();
                MelonLogger.Msg("[GUIManager] Menu opened (advanced fields repopulated).");
            }
            else
            {
                MelonLogger.Msg("[GUIManager] Menu closed.");
            }
        }

        /// <summary>
        /// Draws a horizontal slider for adjusting the overall GUI opacity,
        /// plus a percentage label for feedback.
        /// </summary>
        private void DrawOpacitySlider()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Opacity:", GUILayout.Width(60));

            float newVal = GUILayout.HorizontalSlider(_opacity, 0.1f, 1.0f, GUILayout.Width(150));
            _opacity = newVal;

            GUILayout.Label($"{(_opacity * 100f):F0}%", GUILayout.Width(40));
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Called by your mod's OnGUI or a MonoBehaviour's OnGUI.
        /// Renders the main IMGUI window if _menuOpen is true.
        /// </summary>
        public void DrawMenu()
        {
            // If the menu is toggled off, do nothing
            if (!_menuOpen) return;

            // Set overall alpha for the entire window
            var prevColor = GUI.color;
            GUI.color = new Color(prevColor.r, prevColor.g, prevColor.b, _opacity);

            // Use the built-in window style again
            _windowRect = GUILayout.Window(
                1001,                 // unique window ID
                _windowRect,          // current rect
                WindowFunction,       // callback
                "",                   // empty title (we draw a custom title bar ourselves)
                GUI.skin.window       // <-- the default built-in style
            );

            // Restore GUI color
            GUI.color = prevColor;

            // Allow resizing from corners
            HandleResize();
        }


        /// <summary>
        /// The main IMGUI window function. Lays out the "title bar" row
        /// with _ [ ] X, top bar panel (menuBar), toggles for advanced vs. simple,
        /// and then calls the appropriate panel draws.
        /// </summary>
        private void WindowFunction(int windowID)
        {
            // If not initialized, show "retry" logic (unchanged)
            if (!_sledParamManager.IsInitialized)
            {
                GUILayout.Label("<color=red>No sled found. Wait for spawn or click 'Retry'.</color>");
                if (GUILayout.Button("Retry"))
                {
                    _sledParamManager.InitializeComponents();
                    _advancedPanel.RePopulateFields();
                }
                GUI.DragWindow(new Rect(0, 0, 10000, 20));
                return;
            }

            // 1) Custom title bar, etc.
            DrawCustomTitleBar();
            GUILayout.Space(5);

            // 2) Top MenuBar
            _menuBar.DrawMenuBar();
            GUILayout.Space(5);

            // 3) Simple/Advanced toggle
            GUILayout.BeginHorizontal();
            GUILayout.Label("View Mode:", GUILayout.Width(80));
            bool newShowAdv = GUILayout.Toggle(_showAdvanced, "Advanced");
            if (newShowAdv != _showAdvanced)
            {
                _showAdvanced = newShowAdv;
                MelonLogger.Msg($"[GUIManager] Switched to {(_showAdvanced ? "Advanced" : "Simple")} view.");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // 4) Show the chosen panel
            if (_showAdvanced)
            {
                _advancedPanel.DrawAdvancedPanel();
            }
            else
            {
                _simplePanel.DrawSimplePanel();
            }

            // 5) Now push everything else up, so the slider is "at the bottom"
            GUILayout.FlexibleSpace();  // This reserves flexible vertical space above the slider

            // 6) Center the slider horizontally
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();     // push from left
            DrawOpacitySlider();           // your existing method
            GUILayout.FlexibleSpace();     // push from right
            GUILayout.EndHorizontal();

            // 7) A bit of extra spacing below, if you like
            GUILayout.Space(10);

            // 8) Make top area draggable
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        /// <summary>
        /// Draws a custom "title bar" row with "Sled Tuner" text on the left
        /// and the _ [ ] X buttons on the right.
        /// </summary>
        private void DrawCustomTitleBar()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Sled Tuner", _headerStyle, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("_", _titleBarButtonStyle, GUILayout.Width(25)))
            {
                MinimizeWindow();
            }
            if (GUILayout.Button("[ ]", _titleBarButtonStyle, GUILayout.Width(25)))
            {
                MaximizeWindow();
            }
            if (GUILayout.Button("X", _titleBarButtonStyle, GUILayout.Width(25)))
            {
                ToggleMenu();
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Initializes or updates the custom styles if not set. 
        /// </summary>
        private void InitStyles()
        {
            if (_titleBarButtonStyle == null)
            {
                _titleBarButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    padding = new RectOffset(2, 2, 1, 1),
                    margin = new RectOffset(2, 2, 0, 0)
                };
            }

            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.white },
                };
            }

            // NEW: define our custom window style with zero top padding
            if (_customWindowStyle == null)
            {
                _customWindowStyle = new GUIStyle(GUI.skin.window)
                {
                    padding = new RectOffset(0, 0, 0, 0),
                    border = new RectOffset(2, 2, 2, 2)
                };
            }
        }

        #region Resizing/Minimize Logic

        private void HandleResize()
        {
            if (Event.current.type != EventType.Repaint
                && Event.current.type != EventType.MouseDown
                && Event.current.type != EventType.MouseUp
                && Event.current.type != EventType.MouseDrag)
                return;

            Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            bool nearRight = Mathf.Abs(mousePos.x - _windowRect.xMax) < 10f;
            bool nearBottom = Mathf.Abs(mousePos.y - _windowRect.yMax) < 10f;

            if (Event.current.type == EventType.MouseDown && (nearRight || nearBottom))
            {
                _isResizing = true;
                _resizeStartMousePos = mousePos;
                _resizeStartWindowRect = _windowRect;
                Event.current.Use();
            }

            if (_isResizing && (Event.current.type == EventType.MouseDrag
                             || Event.current.type == EventType.MouseMove))
            {
                Vector2 delta = mousePos - _resizeStartMousePos;
                if (nearRight)
                {
                    _windowRect.width = Mathf.Clamp(
                        _resizeStartWindowRect.width + delta.x,
                        200,
                        Screen.width - 50
                    );
                }
                if (nearBottom)
                {
                    _windowRect.height = Mathf.Clamp(
                        _resizeStartWindowRect.height + delta.y,
                        100,
                        Screen.height - 50
                    );
                }
                Event.current.Use();
            }

            if (_isResizing && Event.current.type == EventType.MouseUp)
            {
                _isResizing = false;
                Event.current.Use();
            }
        }

        private void MinimizeWindow()
        {
            if (!_isMinimized)
            {
                _prevWindowRect = _windowRect;
                _windowRect = new Rect(Screen.width - 200f, 0f, 200f, 30f);
                _isMinimized = true;
                MelonLogger.Msg("[GUIManager] Window minimized.");
            }
        }

        private void MaximizeWindow()
        {
            if (_isMinimized)
            {
                _windowRect = _prevWindowRect;
                _isMinimized = false;
                MelonLogger.Msg("[GUIManager] Window restored from minimized state.");
            }
            else
            {
                _prevWindowRect = _windowRect;
                _windowRect = new Rect(0f, 0f, Screen.width, Screen.height);
                MelonLogger.Msg("[GUIManager] Window maximized to full screen.");
            }
        }

        #endregion
    }
}
