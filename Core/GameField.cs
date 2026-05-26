using System;
using System.Collections.Generic;

namespace Model.Core
{
    public class GameField
    {
        public int Width { get; }
        public int Height { get; }
        public int TotalMines { get; private set; }
        public CellBase[,] Cells { get; private set; }

        private readonly Random _random = new Random();

        public GameField(int width, int height, double minePercentage)
        {
            Width = width;
            Height = height;
            int totalCells = width * height;
            TotalMines = (int)(totalCells * minePercentage);
            if (TotalMines < 1) TotalMines = 1;
            if (TotalMines > totalCells - 1) TotalMines = totalCells - 1;
        }

        public bool TryGenerateField(int maxAttempts = 50)
        {
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                GenerateEmptyField();
                PlaceMinesRandomly();
                CalculateNearbyNumbers();
                if (ValidateMinesSurroundings())
                    return true;
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

        private void PlaceMinesRandomly()
        {
            int minesPlaced = 0;
            while (minesPlaced < TotalMines)
            {
                int row = _random.Next(Height);
                int col = _random.Next(Width);
                if (!(Cells[row, col] is MineCell))
                {
                    Cells[row, col] = new MineCell();
                    minesPlaced++;
                }
            }
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