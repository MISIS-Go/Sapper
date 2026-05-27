using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Linq;
using Model;
using Core;
using Data;
using Raylib_cs;

namespace UI;

public class Game : IUI
{
    private GameLogic? _logic;
    private int _width;
    private int _height;
    private double _minePercentage;
    private double _startTime;
    private int _elapsedSeconds;
    private bool _gameFinished;
    private bool _lastResultWasWin;

    public string StatusMessage { get; private set; } = string.Empty;
    public bool LastResultWasWin => _lastResultWasWin;
    public int LastElapsedSeconds => _elapsedSeconds;

    public void StartNewGame(int width, int height, double minePercentage)
    {
        _width = width;
        _height = height;
        _minePercentage = minePercentage;
        _gameFinished = false;
        _lastResultWasWin = false;
        _elapsedSeconds = 0;
        StatusMessage = string.Empty;

        try
        {
            Directory.CreateDirectory(InitUI.GetGameDataFolder());
            _logic = new GameLogic(width, height, minePercentage);
            _logic.OnGameLose += HandleGameLose;
            _logic.OnGameWin += HandleGameWin;
            _startTime = Raylib.GetTime();
        }
        catch (Exception exception)
        {
            _logic = null;
            StatusMessage = exception.Message;
        }
    }

    public bool TryLoadSavedGame(bool preferSelectedFormat)
    {
        foreach (string candidate in GetLoadCandidates(preferSelectedFormat))
        {
            if (!File.Exists(candidate))
                continue;

            GameSnapshot? snapshot = GameSaveStore.Load(candidate);
            if (snapshot is not null)
            {
                RestoreSnapshot(snapshot);
                StatusMessage = string.Empty;
                return true;
            }
        }

        StatusMessage = "Saved game was not found";
        return false;
    }

    public bool TryLoadSavedGame(string path)
    {
        if (!File.Exists(path))
        {
            StatusMessage = "Save file was not found";
            return false;
        }

        GameSnapshot? snapshot = GameSaveStore.Load(path);
        if (snapshot is null)
        {
            StatusMessage = "Save file is invalid";
            return false;
        }

        RestoreSnapshot(snapshot);
        StatusMessage = string.Empty;
        return true;
    }

    public void SaveCurrentGame()
    {
        if (_logic is null || _gameFinished)
            return;

        Directory.CreateDirectory(InitUI.GetGameDataFolder());

        GameSnapshot snapshot = BuildSnapshot();
        GameSaveStore.Save(snapshot, ResolveSavePath(InitUI.SelectedSaveFormat));
    }

    public void Draw()
    {
        Rectangle backButton = new Rectangle(40, 30, 120, 44);
        Vector2 mousePos = Raylib.GetMousePosition();

        if (_logic is null)
        {
            if (Raylib.CheckCollisionPointRec(mousePos, backButton) && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                InitUI.OpenMainMenu();
                return;
            }

            Lib.DrawButton(backButton, "Back", mousePos);
            Raylib.DrawText("No active game", 320, 420, 28, Color.Black);

            if (!string.IsNullOrWhiteSpace(StatusMessage))
                Raylib.DrawText(StatusMessage, 180, 470, 20, Color.Maroon);

            return;
        }

        _elapsedSeconds = GetElapsedSeconds();
        bool leftClick = Raylib.IsMouseButtonPressed(MouseButton.Left);
        bool rightClick = Raylib.IsMouseButtonPressed(MouseButton.Right);

        if (Raylib.CheckCollisionPointRec(mousePos, backButton) && leftClick)
        {
            SaveCurrentGame();
            InitUI.OpenMainMenu();
            return;
        }

        if (!_gameFinished)
        {
            HandleBoardInput(mousePos, leftClick, rightClick);
        }

        DrawHeader(backButton, mousePos);
        DrawBoard(mousePos);

        if (!string.IsNullOrWhiteSpace(StatusMessage))
        {
            Raylib.DrawText(StatusMessage, 50, 850, 18, Color.Maroon);
        }
    }

    private void HandleBoardInput(Vector2 mousePos, bool leftClick, bool rightClick)
    {
        var logic = _logic;
        if (logic is null)
            return;

        Rectangle boardRect = GetBoardRectangle();

        if (!Raylib.CheckCollisionPointRec(mousePos, boardRect))
            return;

        int cellSize = GetCellSize();
        int boardLeft = (int)boardRect.X;
        int boardTop = (int)boardRect.Y;
        int col = (int)((mousePos.X - boardLeft) / cellSize);
        int row = (int)((mousePos.Y - boardTop) / cellSize);

        if (row < 0 || row >= _height || col < 0 || col >= _width)
            return;

        if (leftClick)
        {
            logic.RevealCell(row, col);
        }

        if (rightClick)
        {
            logic.ToggleFlag(row, col);
        }
    }

    private void DrawHeader(Rectangle backButton, Vector2 mousePos)
    {
        Lib.DrawButton(backButton, "Back", mousePos);
        Raylib.DrawText($"Time: {FormatTime(_elapsedSeconds)}", 380, 36, 30, Color.Black);
    }

    private void DrawBoard(Vector2 mousePos)
    {
        var logic = _logic;
        if (logic is null)
            return;

        Rectangle boardRect = GetBoardRectangle();
        int cellSize = GetCellSize();

        Raylib.DrawRectangleRec(boardRect, new Color(245, 245, 240, 255));
        Raylib.DrawRectangleLinesEx(boardRect, 2, Color.Black);

        for (int row = 0; row < _height; row++)
        {
            for (int col = 0; col < _width; col++)
            {
                Core.CellBase cell = logic.Field.Cells[row, col];
                Rectangle cellRect = new Rectangle(boardRect.X + col * cellSize, boardRect.Y + row * cellSize, cellSize, cellSize);
                bool hovered = Raylib.CheckCollisionPointRec(mousePos, cellRect);

                Color fillColor = cell.IsOpened ? new Color(230, 230, 225, 255) : Color.White;
                if (hovered && !_gameFinished)
                    fillColor = cell.IsOpened ? new Color(220, 220, 215, 255) : new Color(250, 250, 245, 255);

                Raylib.DrawRectangleRec(cellRect, fillColor);
                Raylib.DrawRectangleLinesEx(cellRect, 1, Color.Black);

                if (cell.IsOpened)
                {
                    if (cell.IsMine)
                    {
                        DrawMine(cellRect);
                    }
                    else if (cell.NearbyMinesCount > 0)
                    {
                        Raylib.DrawText(cell.NearbyMinesCount.ToString(),
                            (int)cellRect.X + cellSize / 3,
                            (int)cellRect.Y + cellSize / 6,
                            Math.Max(16, cellSize / 2),
                            GetNumberColor(cell.NearbyMinesCount));
                    }
                }
                else if (cell.HasFlag)
                {
                    Raylib.DrawText("F",
                        (int)cellRect.X + cellSize / 3,
                        (int)cellRect.Y + cellSize / 6,
                        Math.Max(16, cellSize / 2),
                        Color.Red);
                }
                else if (_gameFinished && cell.IsMine)
                {
                    DrawMine(cellRect);
                }
            }
        }
    }

    private void DrawMine(Rectangle rect)
    {
        int radius = Math.Max(6, (int)(rect.Width / 4));
        Raylib.DrawCircle((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2), radius, Color.Black);
    }

    private Color GetNumberColor(int count)
    {
        return count switch
        {
            1 => Color.Blue,
            2 => Color.Green,
            3 => Color.Red,
            4 => Color.DarkBlue,
            5 => Color.Maroon,
            6 => new Color(0, 139, 139, 255),
            7 => Color.Black,
            _ => Color.DarkGray
        };
    }

    private int GetCellSize()
    {
        const int screenWidth = 900;
        const int topSpace = 160;
        const int bottomSpace = 70;
        int availableWidth = screenWidth - 80;
        int availableHeight = 900 - topSpace - bottomSpace;
        int widthSize = Math.Max(28, availableWidth / Math.Max(1, _width));
        int heightSize = Math.Max(28, availableHeight / Math.Max(1, _height));
        return Math.Min(widthSize, heightSize);
    }

    private Rectangle GetBoardRectangle()
    {
        int cellSize = GetCellSize();
        int boardWidth = cellSize * _width;
        int boardHeight = cellSize * _height;
        int boardLeft = (900 - boardWidth) / 2;
        int boardTop = 150;
        return new Rectangle(boardLeft, boardTop, boardWidth, boardHeight);
    }

    private int GetElapsedSeconds()
    {
        if (_gameFinished)
            return _elapsedSeconds;

        return Math.Max(0, (int)(Raylib.GetTime() - _startTime));
    }

    private void HandleGameLose()
    {
        FinishGame(false);
    }

    private void HandleGameWin()
    {
        FinishGame(true);
    }

    private void FinishGame(bool win)
    {
        if (_gameFinished)
            return;

        _elapsedSeconds = Math.Max(0, (int)(Raylib.GetTime() - _startTime));
        _gameFinished = true;
        _lastResultWasWin = win;

        if (win)
        {
            Liderboard.AddResult(_width, _height, _minePercentage, _elapsedSeconds);
        }

        InitUI.CurrentState = GameState.EndGame;
    }

    private void RestoreSnapshot(GameSnapshot snapshot)
    {
        StartNewGame(snapshot.Width, snapshot.Height, snapshot.MinePercentage);
        var logic = _logic;
        if (logic is null)
            return;

        _elapsedSeconds = Math.Max(0, snapshot.ElapsedSeconds);
        _startTime = Raylib.GetTime() - _elapsedSeconds;

        if (snapshot.Cells.Count < snapshot.Width * snapshot.Height)
        {
            StatusMessage = "Save file is invalid";
            return;
        }

        int index = 0;
        for (int row = 0; row < snapshot.Height; row++)
        {
            for (int col = 0; col < snapshot.Width; col++)
            {
                CellSnapshot savedCell = snapshot.Cells[index++];
                Core.CellBase restoredCell = savedCell.IsMine
                    ? new MineCell()
                    : new NumberCell(savedCell.NearbyMinesCount);

                restoredCell.IsOpened = savedCell.IsOpened;
                restoredCell.HasFlag = savedCell.HasFlag;
                restoredCell.NearbyMinesCount = savedCell.NearbyMinesCount;
                logic.Field.Cells[row, col] = restoredCell;
            }
        }

        InitUI.SelectedWidth = snapshot.Width;
        InitUI.SelectedHeight = snapshot.Height;
        _minePercentage = snapshot.MinePercentage;
        logic.MarkFieldAsGenerated();
    }

    private GameSnapshot BuildSnapshot()
    {
        var snapshot = new GameSnapshot
        {
            Width = _width,
            Height = _height,
            MinePercentage = _minePercentage,
            ElapsedSeconds = _elapsedSeconds,
            Cells = new List<CellSnapshot>()
        };

        for (int row = 0; row < _height; row++)
        {
            for (int col = 0; col < _width; col++)
            {
                Core.CellBase cell = _logic!.Field.Cells[row, col];
                snapshot.Cells.Add(new CellSnapshot
                {
                    IsMine = cell.IsMine,
                    IsOpened = cell.IsOpened,
                    HasFlag = cell.HasFlag,
                    NearbyMinesCount = cell.NearbyMinesCount
                });
            }
        }

        return snapshot;
    }

    private string[] GetLoadCandidates(bool preferSelectedFormat)
    {
        string gameDataFolder = InitUI.GetGameDataFolder();
        if (!Directory.Exists(gameDataFolder))
            return Array.Empty<string>();

        IEnumerable<string> files = Directory.EnumerateFiles(gameDataFolder, "*.*", SearchOption.TopDirectoryOnly)
            .Where(path => path.EndsWith(".json", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(path => File.GetLastWriteTimeUtc(path));

        if (preferSelectedFormat)
        {
            string selectedExtension = InitUI.SelectedSaveFormat.Equals("Xml", StringComparison.OrdinalIgnoreCase) ? ".xml" : ".json";

            return files
                .OrderByDescending(path => path.EndsWith(selectedExtension, StringComparison.OrdinalIgnoreCase))
                .ThenByDescending(path => File.GetLastWriteTimeUtc(path))
                .ToArray();
        }

        return files.ToArray();
    }

    private string ResolveSavePath(string format)
    {
        string extension = format.Equals("Xml", StringComparison.OrdinalIgnoreCase) ? "xml" : "json";
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        string difficulty = SanitizeFilePart(InitUI.SelectedDifficultyName);
        string size = $"{_width}x{_height}";
        string fileName = $"{timestamp}_{difficulty}_{size}.{extension}";
        return Path.Combine(InitUI.GetGameDataFolder(), fileName);
    }

    private static string SanitizeFilePart(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Unknown";

        foreach (char invalid in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalid, '_');
        }

        return value.Replace(' ', '_');
    }

    private static string FormatTime(int seconds)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return $"{minutes:00}:{remainingSeconds:00}";
    }
}
