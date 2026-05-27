using System;

namespace Core
{
    public class GameField
    {
        public int Width { get; }
        public int Height { get; }
        public int TotalMines { get; private set; }
        public CellBase[,] Cells { get; private set; } = null!;
        public bool IsGenerated { get; private set; }

        private readonly Random _random = new Random();

        public GameField(int width, int height, double minePercentage)
        {
            Width = width;
            Height = height;
            int totalCells = width * height;
            int requestedMines = (int)Math.Round(totalCells * minePercentage);
            int minMines = Math.Max(1, (int)Math.Ceiling(totalCells * 0.20));
            int maxMines = Math.Max(minMines, (int)Math.Floor(totalCells * 0.40));

            TotalMines = Math.Clamp(requestedMines, minMines, maxMines);
            TotalMines = Math.Min(TotalMines, Math.Max(0, totalCells - 1));
            GenerateEmptyField();
            IsGenerated = false;
        }

        public bool TryGenerateField(int safeRow, int safeCol, int safeRadius = 1, int maxAttempts = 5000)
        {
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                GenerateEmptyField();
                PlaceMinesRandomly(safeRow, safeCol, safeRadius);
                CalculateNearbyNumbers();
                if (ValidateMinesSurroundings())
                {
                    IsGenerated = true;
                    return true;
                }
            }
            return false;
        }

        private void GenerateEmptyField()
        {
            Cells = new CellBase[Height, Width];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    Cells[i, j] = new NumberCell(0);
        }

        private void PlaceMinesRandomly(int safeRow, int safeCol, int safeRadius)
        {
            bool[,] protectedCells = BuildProtectedCells(safeRow, safeCol, safeRadius);
            int protectedCount = CountProtectedCells(protectedCells);
            TotalMines = Math.Min(TotalMines, Math.Max(0, Width * Height - protectedCount));

            int minesPlaced = 0;
            while (minesPlaced < TotalMines)
            {
                int row = _random.Next(Height);
                int col = _random.Next(Width);
                if (protectedCells[row, col])
                    continue;

                if (!(Cells[row, col] is MineCell))
                {
                    Cells[row, col] = new MineCell();
                    minesPlaced++;
                }
            }
        }

        private bool[,] BuildProtectedCells(int safeRow, int safeCol, int safeRadius)
        {
            bool[,] protectedCells = new bool[Height, Width];
            for (int row = safeRow - safeRadius; row <= safeRow + safeRadius; row++)
            {
                for (int col = safeCol - safeRadius; col <= safeCol + safeRadius; col++)
                {
                    if (row >= 0 && row < Height && col >= 0 && col < Width)
                        protectedCells[row, col] = true;
                }
            }

            return protectedCells;
        }

        private int CountProtectedCells(bool[,] protectedCells)
        {
            int count = 0;
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    if (protectedCells[row, col])
                        count++;
                }
            }

            return count;
        }

        private void CalculateNearbyNumbers()
        {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    if (Cells[i, j] is MineCell) continue;
                    int minesAround = CountMinesAround(i, j);
                    Cells[i, j] = new NumberCell(minesAround);
                }
        }

        private int CountMinesAround(int row, int col)
        {
            int count = 0;
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int r = row + dr, c = col + dc;
                    if (r >= 0 && r < Height && c >= 0 && c < Width && Cells[r, c] is MineCell)
                        count++;
                }
            return count;
        }

        private bool ValidateMinesSurroundings()
        {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    if (Cells[i, j] is MineCell)
                    {
                        int nonMineNeighbors = 0;
                        for (int dr = -1; dr <= 1; dr++)
                            for (int dc = -1; dc <= 1; dc++)
                            {
                                if (dr == 0 && dc == 0) continue;
                                int r = i + dr, c = j + dc;
                                if (r >= 0 && r < Height && c >= 0 && c < Width && !(Cells[r, c] is MineCell))
                                    nonMineNeighbors++;
                            }
                        if (nonMineNeighbors < 3)
                            return false;
                    }
                }
            return true;
        }
    }
}
