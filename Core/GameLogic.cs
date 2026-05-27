using System;

namespace Core
{
    public partial class GameLogic
    {
        public GameField Field { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsWin { get; private set; }
        private bool _hasGeneratedField;

        public event Action? OnGameLose;
        public event Action? OnGameWin;

        public GameLogic(int width, int height, double minePercentage)
        {
            Field = new GameField(width, height, minePercentage);
            IsGameOver = false;
            IsWin = false;
            _hasGeneratedField = false;
        }

        public void RevealCell(int row, int col)
        {
            if (IsGameOver) return;

            if (!_hasGeneratedField)
            {
                if (!Field.TryGenerateField(row, col, 1))
                {
                    _hasGeneratedField = true;
                    OpenCellRecursively(row, col);
                    CheckWinCondition();
                    return;
                }

                _hasGeneratedField = true;
            }

            var cell = Field.Cells[row, col];
            if (cell.IsOpened || cell.HasFlag) return;

            if (cell is MineCell)
            {
                IsGameOver = true;
                IsWin = false;
                OnGameLose?.Invoke();
                return;
            }

            OpenCellRecursively(row, col);
            CheckWinCondition();
        }

        private void OpenCellRecursively(int row, int col)
        {
            if (row < 0 || row >= Field.Height || col < 0 || col >= Field.Width) return;
            var cell = Field.Cells[row, col];
            if (cell.IsOpened || cell.HasFlag) return;
            if (cell is MineCell) return;

            cell.IsOpened = true;
            if (cell.NearbyMinesCount == 0)
            {
                for (int dr = -1; dr <= 1; dr++)
                    for (int dc = -1; dc <= 1; dc++)
                        if (dr != 0 || dc != 0)
                            OpenCellRecursively(row + dr, col + dc);
            }
        }

        public void ToggleFlag(int row, int col)
        {
            if (IsGameOver) return;
            var cell = Field.Cells[row, col];
            if (cell.IsOpened) return;
            cell.HasFlag = !cell.HasFlag;
            CheckWinCondition();
        }

        public void MarkFieldAsGenerated()
        {
            _hasGeneratedField = true;
        }

        partial void CheckWinCondition();
    }
}
