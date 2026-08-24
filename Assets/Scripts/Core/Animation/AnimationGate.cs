using System;

namespace CaseStudy.WheelSpin
{
    /// <summary>
    /// Joins several animations that start and finish at different times into one completion callback.
    /// Every part is reserved with <see cref="Track"/> before it starts; <see cref="Seal"/> declares
    /// that no further parts will be added. While the gate is open it never completes early, so a part
    /// that finishes synchronously cannot fire the callback before the rest are registered.
    /// </summary>
    public sealed class AnimationGate
    {
        private readonly Action _partComplete;

        private int _pending;
        private bool _isOpen;
        private Action _onComplete;

        public AnimationGate() => _partComplete = HandlePartComplete;

        public bool IsRunning => _isOpen || _pending > 0;

        public int Pending => _pending;

        public void Begin(Action onComplete)
        {
            _onComplete = onComplete;
            _pending = 0;
            _isOpen = true;
        }

        public Action Track()
        {
            _pending++;
            return _partComplete;
        }

        public void Seal()
        {
            if (!_isOpen)
                return;

            _isOpen = false;

            if (_pending == 0)
                Complete();
        }

        public void Cancel()
        {
            _isOpen = false;
            _pending = 0;
            _onComplete = null;
        }

        private void HandlePartComplete()
        {
            if (_pending > 0)
                _pending--;

            if (_isOpen || _pending > 0)
                return;

            Complete();
        }

        private void Complete()
        {
            Action callback = _onComplete;
            _onComplete = null;
            callback?.Invoke();
        }
    }
}
