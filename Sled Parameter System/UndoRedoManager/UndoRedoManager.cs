using System.Collections.Generic;
using UnityEngine;

namespace SledTunerProject.SledParameterSystem
{
    /// <summary>
    /// Maintains a history of parameter changes for undo/redo functionality.
    /// </summary>
    public class UndoRedoManager
    {
        // Two stacks: one for undo, one for redo
        private Stack<ParameterChange> _undoStack;
        private Stack<ParameterChange> _redoStack;

        public UndoRedoManager()
        {
            _undoStack = new Stack<ParameterChange>();
            _redoStack = new Stack<ParameterChange>();
        }

        public void RecordChange(string paramID, float oldValue, float newValue)
        {
            var change = new ParameterChange(paramID, oldValue, newValue);
            _undoStack.Push(change);

            // Once a new change is recorded, the redo stack is invalidated
            _redoStack.Clear();

            Debug.Log($"[UndoRedoManager] Recorded change: {paramID} from {oldValue} to {newValue}");
        }

        /// <summary>
        /// Undoes the last change, returns the change object if successful, null if none.
        /// </summary>
        public ParameterChange UndoLastChange()
        {
            if (_undoStack.Count == 0)
                return null;

            var change = _undoStack.Pop();
            // push to redo stack
            _redoStack.Push(change);

            Debug.Log($"[UndoRedoManager] Undo change: {change.ParamID} => revert to {change.OldValue}");
            return change;
        }

        /// <summary>
        /// Redoes the last undone change, returns the change object if successful, null if none.
        /// </summary>
        public ParameterChange RedoLastUndoneChange()
        {
            if (_redoStack.Count == 0)
                return null;

            var change = _redoStack.Pop();
            // push back to undo stack
            _undoStack.Push(change);

            Debug.Log($"[UndoRedoManager] Redo change: {change.ParamID} => apply {change.NewValue}");
            return change;
        }
    }

    public class ParameterChange
    {
        public string ParamID { get; private set; }
        public float OldValue { get; private set; }
        public float NewValue { get; private set; }

        public ParameterChange(string paramID, float oldValue, float newValue)
        {
            ParamID = paramID;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
