using System;

namespace Core
{
    public partial class GameLogic
    {
        public int TimeLimitSeconds { get; private set; }
        public int ElapsedSeconds { get; private set; }
        public int RemainingSeconds => Math.Max(0, TimeLimitSeconds - ElapsedSeconds);

        private double _timerStartedAt;
        private bool _timerConfigured;

        public void ConfigureTimer(int timeLimitSeconds, int elapsedSeconds, double currentTime)
        {
            TimeLimitSeconds = Math.Max(1, timeLimitSeconds);
            ElapsedSeconds = Math.Clamp(elapsedSeconds, 0, TimeLimitSeconds);
            _timerStartedAt = currentTime - ElapsedSeconds;
            _timerConfigured = true;
        }

        public void UpdateTimer(double currentTime)
        {
            if (!_timerConfigured || IsGameOver)
                return;

            ElapsedSeconds = Math.Max(0, (int)Math.Floor(currentTime - _timerStartedAt));
            if (ElapsedSeconds < TimeLimitSeconds)
                return;

            ElapsedSeconds = TimeLimitSeconds;
            IsGameOver = true;
            IsWin = false;
            OnGameLose?.Invoke();
        }
    }
}
