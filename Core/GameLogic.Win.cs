using System.Linq;

namespace Model.Core
{
    public partial class GameLogic
    {
        partial void CheckWinCondition()
        {
            if (IsGameOver) return;

            bool allMinesFlagged = true;
            bool allNonMinesOpened = true;

            for (int i = 0; i < Field.Height; i++)
            {
                for (int j = 0; j < Field.Width; j++)
                {
                    var cell = Field.Cells[i, j];
                    if (cell.IsMine)
                    {
                        if (!cell.HasFlag)
                            allMinesFlagged = false;
                    }
                    else
                    {
                        if (!cell.IsOpened)
                            allNonMinesOpened = false;
                    }
                }
            }

            if (allMinesFlagged && allNonMinesOpened)
            {
                IsGameOver = true;
                IsWin = true;
                OnGameWin?.Invoke();
            }
        }
    }
}