<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <title>Sled Tuner Mod - README</title>
  <style>
    body {
      font-family: Arial, sans-serif;
      background: linear-gradient(to bottom, #ffffff 0%, #cceaff 100%);
      color: #333;
      margin: 0;
      padding: 0;
    }
    header {
      background: #00557f;
      color: #fff;
      padding: 2em;
      text-align: center;
      background-image: url('https://cdn.pixabay.com/photo/2016/01/19/17/56/mountain-1149977_960_720.jpg'); 
      background-size: cover;
      background-position: center;
    }
    header h1 {
      margin: 0;
      font-size: 3em;
      text-shadow: 2px 2px #000;
    }
    header p {
      margin: 1em 0 0;
      text-shadow: 1px 1px #000;
    }
    main {
      max-width: 800px;
      margin: 2em auto;
      padding: 1em;
      background-color: rgba(255, 255, 255, 0.85);
      border-radius: 8px;
      box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    }
    h2 {
      margin-top: 1.5em;
      color: #00557f;
      border-bottom: 2px solid #eee;
      padding-bottom: 0.4em;
    }
    ul {
      list-style-type: none;
      padding-left: 0;
    }
    li {
      margin: 0.5em 0;
    }
    .emoji {
      font-size: 1.2em;
      margin-right: 0.4em;
    }
    code {
      background-color: #f0f0f0;
      padding: 0.2em 0.4em;
      border-radius: 4px;
    }
    pre {
      background-color: #f7f7f7;
      padding: 1em;
      border-radius: 4px;
      overflow-x: auto;
    }
    .footer {
      margin-top: 2em;
      text-align: center;
      font-size: 0.9em;
      color: #666;
    }
    .highlight {
      background-color: #eaffea;
      border-left: 4px solid #00bf00;
      padding: 1em;
      margin: 1em 0;
      border-radius: 4px;
    }
  </style>
</head>
<body>

<header>
  <h1>❄️ Sled Tuner Mod</h1>
  <p>🛷 Fine-tune your sled in <strong>Sledders</strong> with real-time editing!</p>
</header>

<main>
  <section>
    <h2>Overview</h2>
    <p>
      A <strong>Unity</strong> mod for <em>Sledders</em> that allows <strong>players</strong> and <strong>modders</strong> 
      to tweak sled parameters (engine, suspension, physics, etc.) <em>on the fly</em>!  
      Built for <code>.NET 4.7.2</code> and <code>C# 7.3</code> with a 
      <strong>clean, modular</strong> architecture.
    </p>
  </section>

  <section>
    <h2>Features</h2>
    <ul>
      <li><span class="emoji">⏱️</span><strong>Real-Time Editing</strong>: Adjust engine power, suspension stiffness, etc.</li>
      <li><span class="emoji">👀</span><strong>Custom GUI</strong>: Sliders, tabs, a JSON editor, advanced &amp; simple views.</li>
      <li><span class="emoji">↩️</span><strong>Undo/Redo</strong>: Roll back or re-apply parameter changes for safe experimentation.</li>
      <li><span class="emoji">🗂️</span><strong>Preset Management</strong>: Create, load, and save multiple parameter sets.</li>
      <li><span class="emoji">📁</span><strong>Config Files</strong>: Robust JSON I/O with optional auto-save &amp; backups.</li>
      <li><span class="emoji">🏔️</span><strong>Game Integration</strong>: Direct updates into Sledders via <code>GameObjectManager</code>.</li>
    </ul>
  </section>

  <section>
    <h2>Architecture Overview</h2>
    <p>
      The project is divided into <strong>five main systems</strong>, each in its own folder:
    </p>
    <ul>
      <li><strong>GUI System</strong>: Panels, tab manager, parameter editor, etc.</li>
      <li><strong>Sled Parameter System</strong>: Core logic (<code>SledParameterManager</code>, store, validator, undo/redo).</li>
      <li><strong>Config System</strong>: File I/O, backup, auto-save (<code>ConfigManager</code>).</li>
      <li><strong>Game Integration System</strong>: Actual in-game references (<code>GameObjectManager</code>).</li>
      <li><strong>Utility System</strong>: Helper methods, logging, error handling.</li>
    </ul>
  </section>

  <section>
    <h2>Setup &amp; Installation</h2>
    <ol>
      <li><strong>Requirements</strong>: Unity 2019+, .NET 4.7.2, C# 7.3, MelonLoader (optional).</li>
      <li><strong>Clone/Download</strong>: 
        <pre>git clone https://github.com/0ri0n1/SledTunerProject.git</pre>
      </li>
      <li><strong>Open the Project</strong>: 
        If it's a Unity project, open via Unity Hub. Otherwise, open <code>SledTuner.csproj</code> in an IDE. 
      </li>
      <li><strong>Compile</strong>: Ensure <code>.NET 4.7.2</code> &amp; <code>C# 7.3</code>, then build.</li>
      <li><strong>Install the Mod</strong>:
        <ul>
          <li>Place the resulting <code>SledTuner.dll</code> into <em>Sledders/Mods</em> (or your MelonLoader folder).</li>
          <li>Launch Sledders with MelonLoader.</li>
        </ul>
      </li>
    </ol>
  </section>

  <section>
    <h2>Usage</h2>
    <p>
      <span class="emoji">🎮</span> <strong>Open the Menu:</strong> Press <code>F2</code> by default to toggle the GUI. 
      Navigate the tabs or advanced editor for parameter adjustments.
    </p>
    <p>
      <span class="emoji">🐾</span> <strong>Advanced Editing:</strong> Switch to the <em>Advanced View</em> for reflection-based parameters or raw JSON editing.
    </p>
    <p>
      <span class="emoji">↩️</span> <strong>Undo/Redo:</strong> Use hotkeys (<code>Ctrl+Z</code>, <code>Ctrl+Y</code>) or the Menu to revert changes.
    </p>
    <p>
      <span class="emoji">💾</span> <strong>Load/Save Configs:</strong> 
      In the File menu, open or save JSON configs. By default, they reside in <code>SledTuner/Configs</code>.
    </p>
    <p>
      <span class="emoji">🗂️</span> <strong>Presets:</strong> 
      Quickly load or save named sets in the <code>SledTuner/Presets</code> folder.
    </p>
  </section>

  <section>
    <h2>Development Phases</h2>
    <ol>
      <li><strong>Core Classes</strong>: <code>ParameterStore</code>, <code>ParameterValidator</code>, <code>ParameterUpdater</code></li>
      <li><strong>Basic GUI</strong>: <code>GUIManager</code> with minimal UI to log parameter changes</li>
      <li><strong>Undo/Redo</strong>: <code>UndoRedoManager</code> + GUI integration</li>
      <li><strong>File &amp; Preset</strong>: <code>ConfigManager</code>, <code>ParameterPresetManager</code></li>
      <li><strong>Real-Time Updates</strong>: Hook <code>ParameterUpdater</code> → <code>GameObjectManager</code></li>
      <li><strong>Validation</strong>: Show range errors or warnings in the GUI</li>
      <li><strong>Testing &amp; Refinement</strong>: Edge cases, performance checks</li>
      <li><strong>Advanced</strong>: Multi-config comparison, cloud sync, data visualization</li>
    </ol>
  </section>

  <section>
    <h2>Contributing</h2>
    <div class="highlight">
      <p><strong>Pull Requests</strong>: Always welcome! Keep commits atomic and reference open issues.</p>
      <p><strong>Issues / Bugs</strong>: Submit a GitHub issue with logs or screenshots.</p>
      <p><strong>Coding Standards</strong>: 
        Use <code>C# 7.3</code> where appropriate, maintain consistent naming (PascalCase for methods), 
        and separate concerns into their respective folders.
      </p>
    </div>
  </section>

  <section>
    <h2>License</h2>
    <p>
      This project is licensed under the <strong>MIT License</strong>—see the 
      <a href="LICENSE">LICENSE</a> file for details.
    </p>
  </section>

  <section>
    <h2>Contact &amp; Support</h2>
    <ul>
      <li><strong>Discord</strong>: Join our Sledders modding community (link here)</li>
      <li><strong>GitHub</strong>: Submit an <a href="https://github.com/YourName/SledTunerProject/issues">issue</a></li>
      <li><strong>Email</strong>: <em>youremail@example.com</em> (optional)</li>
    </ul>
    <p><strong>Happy Sledding!</strong> 🏔️🛷❄️</p>
  </section>
</main>

<div class="footer">
  <p>Copyright © 2025 Sled Tuner Mod.
     Built with <span style="color:red;">♥</span> for snowy adventures.</p>
</div>

</body>
</html>
