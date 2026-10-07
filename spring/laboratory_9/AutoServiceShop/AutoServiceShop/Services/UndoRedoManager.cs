using System.Collections.Generic;

namespace AutoServiceShop.Services
{
    public class UndoRedoManager<T>
    {
        private readonly Stack<T> _undoStack = new Stack<T>();
        private readonly Stack<T> _redoStack = new Stack<T>();
        private T _currentState;

        public void PushState(T state)
        {
            if (_currentState != null)
                _undoStack.Push(_currentState);
            _currentState = state;
            _redoStack.Clear();
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public T Undo()
        {
            if (!CanUndo) return _currentState;
            _redoStack.Push(_currentState);
            _currentState = _undoStack.Pop();
            return _currentState;
        }

        public T Redo()
        {
            if (!CanRedo) return _currentState;
            _undoStack.Push(_currentState);
            _currentState = _redoStack.Pop();
            return _currentState;
        }
    }
}