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

        // Styles for top-bar buttons
        private GUIStyle _titleBarButtonStyle;
        private GUIStyle _headerStyle; // if you want a style for the window title

        public GUIManager(SledParameterManager spm, ConfigManager cfg)
        {
            _sledParamManager = spm;
            _configManager = cfg;

            // Create sub-panels
            _menuBar = new MenuBarPanel(_configManager, _sledParamManager);
            _simplePanel = new SimpleTunerPanel(_sledParamManager);
            _advancedPanel = new AdvancedTunerPanel(_sledParamManager, _configManager);

            // Default window rect (large example)
            _windowRect = new Rect(
                Screen.width * 0.2f,
                Screen.height * 0.2f,
                Screen.width * 0.9f,
                Screen.height * 0.9f
            );

            MelonLogger.Msg("[GUIManager] Created with 3 sub-panels (MenuBar, Simple, Advanced).");
        }

        /// <summary>
        /// Toggles the entire menu on/off. Hook to F2 in your mod.
        /// </summary>
        public void ToggleMenu()
        {
            _menuOpen = !_menuOpen;
            if (_menuOpen)
            {
                // Refresh advanced fields so reflection data is up to date
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

            // The slider goes from 0.1 (mostly transparent) to 1.0 (fully opaque).
            float newVal = GUILayout.HorizontalSlider(_opacity, 0.1f, 1.0f, GUILayout.Width(150));
            _opacity = newVal;

            // Display current opacity % next to the slider.
            GUILayout.Label($"{(_opacity * 100f):F0}%", GUILayout.Width(40));
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Called by your mod's OnGUI or a MonoBehaviour's OnGUI.
        /// Renders the main IMGUI window if _menuOpen is true.
        /// </summary>
        public void DrawMenu()
        {
            if (!_menuOpen) return;

            // Setup alpha blending
            var prevColor = GUI.color;
            GUI.color = new Color(prevColor.r, prevColor.g, prevColor.b, _opacity);

            // Draw the main window
            _windowRect = GUILayout.Window(
                1001,
                _windowRect,
                WindowFunction,
                "",                // We'll manually draw a title bar
                GUI.skin.window
            );

            // Restore color
            GUI.color = prevColor;

            // Handle edge-based resizing
            HandleResize();
        }

        /// <summary>
        /// The main IMGUI window function. Lays out the "title bar" row
        /// with _ [ ] X, top bar panel (menuBar), toggles for advanced vs. simple,
        /// and then calls the appropriate panel draws.
        /// </summary>
        private void WindowFunction(int windowID)
        {
            // Initialize or update styles if needed
            InitStyles();

            // If no sled is found, show "retry" logic
            if (!_sledParamManager.IsInitialized)
            {
                GUILayout.Label("<color=red>No sled found. Wait for spawn or click 'Retry'.</color>");
                if (GUILayout.Button("Retry"))
                {
                    _sledParamManager.InitializeComponents();
                    _advancedPanel.RePopulateFields();
                }
                // Make window draggable
                GUI.DragWindow(new Rect(0, 0, 10000, 20));
                return;
            }

            // === 1) Draw the custom title bar (with minimize, maximize, close) ===
            DrawCustomTitleBar();

            GUILayout.Space(5);

            // === 2) Draw the top MenuBar (Load, Save, Reset, etc.) ===
            _menuBar.DrawMenuBar();

            GUILayout.Space(5);

            // === 3) Simple/Advanced Toggle row ===
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

            // === 4) Show the correct panel ===
            if (_showAdvanced)
            {
                _advancedPanel.DrawAdvancedPanel();
            }
            else
            {
                _simplePanel.DrawSimplePanel();
            }

            // === 5) Draggable area at the top
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        /// <summary>
        /// Draws a custom "title bar" row with Sled Tuner label on the left
        /// and the _ [ ] X buttons on the right.
        /// </summary>
        private void DrawCustomTitleBar()
        {
            GUILayout.BeginHorizontal();

            // Title text
            GUILayout.Label("Sled Tuner", _headerStyle, GUILayout.ExpandWidth(false));

            // Push the window buttons to the far right
            GUILayout.FlexibleSpace();

            // Minimize
            if (GUILayout.Button("_", _titleBarButtonStyle, GUILayout.Width(25)))
            {
                MinimizeWindow();
            }
            // Maximize
            if (GUILayout.Button("[ ]", _titleBarButtonStyle, GUILayout.Width(25)))
            {
                MaximizeWindow();
            }
            // Close
            if (GUILayout.Button("X", _titleBarButtonStyle, GUILayout.Width(25)))
            {
                ToggleMenu(); // or CloseWindow() if you have that method
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Initializes or updates the custom styles if not set. 
        /// Called at the start of WindowFunction.
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
                    // or any additional styling you want
                };
            }
        }

        #region Resizing/Minimize Logic

        /// <summary>
        /// Allows the user to resize the window from the bottom-right corner.
        /// </summary>
        private void HandleResize()
        {
            // We'll only do logic on relevant event types to avoid overhead
            if (Event.current.type != EventType.Repaint
                && Event.current.type != EventType.MouseDown
                && Event.current.type != EventType.MouseUp
                && Event.current.type != EventType.MouseDrag)
                return;

            // Basic approach: check if mouse is near the bottom-right corner
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            bool nearRight = Mathf.Abs(mousePos.x - _windowRect.xMax) < 10f;
            bool nearBottom = Mathf.Abs(mousePos.y - _windowRect.yMax) < 10f;

            // Start resizing
            if (Event.current.type == EventType.MouseDown && (nearRight || nearBottom))
            {
                _isResizing = true;
                _resizeStartMousePos = mousePos;
                _resizeStartWindowRect = _windowRect;
                Event.current.Use();
            }

            // Continue resizing
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

            // Stop resizing
            if (_isResizing && Event.current.type == EventType.MouseUp)
            {
                _isResizing = false;
                Event.current.Use();
            }
        }

        /// <summary>
        /// Minimizes the window by storing the old size and forcing a small rect.
        /// </summary>
        private void MinimizeWindow()
        {
            if (!_isMinimized)
            {
                _prevWindowRect = _windowRect;
                // Force a small bar in the corner
                _windowRect = new Rect(Screen.width - 200f, 0f, 200f, 30f);
                _isMinimized = true;
                MelonLogger.Msg("[GUIManager] Window minimized.");
            }
        }

        /// <summary>
        /// Maximize toggles between restored and full-screen if not minimized,
        /// or we restore from the minimized rect if currently minimized.
        /// </summary>
        private void MaximizeWindow()
        {
            // If currently minimized, restore old rect
            if (_isMinimized)
            {
                _windowRect = _prevWindowRect;
                _isMinimized = false;
                MelonLogger.Msg("[GUIManager] Window restored from minimized state.");
            }
            else
            {
                // Force fullscreen
                _prevWindowRect = _windowRect;
                _windowRect = new Rect(0f, 0f, Screen.width, Screen.height);
                MelonLogger.Msg("[GUIManager] Window maximized to full screen.");
            }
        }

        #endregion
    }
}
