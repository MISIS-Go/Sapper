using System;
using System.Timers;

namespace Model.Core
{
    public partial class GameLogic
    {
        private Timer _gameTimer;
        private int _remainingSeconds;
        public int RemainingSeconds => _remainingSeconds;
        public event Action<int> OnTimerTick;
        public event Action OnTimerExpired;

        public void StartTimer(int initialSeconds)
        {
            _remainingSeconds = initialSeconds;
            _gameTimer = new Timer(1000);
            _gameTimer.Elapsed += TimerElapsed;
            _gameTimer.AutoReset = true;
            _gameTimer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (IsGameOver)
            {
                StopTimer();
                return;
            }

            if (_remainingSeconds > 0)
            {
                _remainingSeconds--;
                OnTimerTick?.Invoke(_remainingSeconds);
            }

            if (_remainingSeconds == 0)
            {
                StopTimer();
                IsGameOver = true;
                IsWin = false;
                OnTimerExpired?.Invoke();
                OnGameLose?.Invoke();
            }
        }

        public void StopTimer()
        {
            if (_gameTimer != null)
            {
                _gameTimer.Stop();
                _gameTimer.Dispose();
                _gameTimer = null;
            }
        }
    }
}