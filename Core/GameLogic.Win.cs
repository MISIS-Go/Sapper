namespace Core
{
    public partial class GameLogic
    {
        partial void CheckWinCondition()
        {
            bool allNonMinesOpened = true;
            for (int r = 0; r < Field.Height; r++)
            for (int c = 0; c < Field.Width; c++)
            {
                var cell = Field.Cells[r, c];
                if (!cell.IsMine && !cell.IsOpened)
                {
                    allNonMinesOpened = false;
                    break;
                }
            }

            if (allNonMinesOpened)
            {
                IsGameOver = true;
                IsWin = true;
                OnGameWin?.Invoke();
            }
        }
    }
}
