using System;

namespace Content.Communication.TickTasking
{
    public abstract class TickTask
    {
        protected float _interval;
        protected float _maxCount;
        protected bool _hasTimeout = false;
        private float _currentElapsed = 0;
        private float _currentCount = 0;

        public EventHandler TickEvent; 
        public EventHandler TimeoutEvent;


        public void CountTick(float interval)
        {
            _currentElapsed += interval;

            if (_currentElapsed >= _interval)
            {
                _currentElapsed = 0;
                TickEvent?.Invoke(this,null);
                _currentCount++;
            }

            if (_hasTimeout && (_currentCount >= _maxCount))
            {
                _currentCount = 0;
                TimeoutEvent?.Invoke(this, null);
            }
        }

        public void ResetTimer()
        {
            _currentElapsed = 0;
            _currentCount = 0;
        }
    }
}