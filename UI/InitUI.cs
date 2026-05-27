using System;
using System.IO;
using Data;
using Core;
using Raylib_cs;

namespace UI;

public class InitUI
{
    public static GameState CurrentState = GameState.Main;
    public static int SelectedWidth = 5;
    public static int SelectedHeight = 5;
    public static double SelectedMinePercentage = Core.Difficulty.Normal;
    public static string SelectedDifficultyName = "Normal";
    public static string SaveFolder = Path.Combine(AppContext.BaseDirectory, "Saves");
    public static string SelectedSaveFormat = "Json";

    public static readonly MainUI MainScreen = new MainUI();
    public static readonly Start StartScreen = new Start();
    public static readonly Continue ContinueScreen = new Continue();
    public static readonly Settings SettingsScreen = new Settings();
    public static readonly Game GameScreen = new Game();
    public static readonly Liderboard LeaderboardScreen = new Liderboard();
    public static readonly EndGame EndGameScreen = new EndGame();

    public static void OpenMainMenu()
    {
        CurrentState = GameState.Main;
    }

    public static void OpenStartMenu()
    {
        CurrentState = GameState.StartMenu;
    }

    public static void OpenContinueMenu()
    {
        CurrentState = GameState.ContinueMenu;
    }

    public static void OpenSettingsMenu()
    {
        CurrentState = GameState.Settings;
    }

    public static void OpenLeaderboardMenu()
    {
        CurrentState = GameState.Leaderboard;
    }

    public static void StartSelectedGame()
    {
        GameScreen.StartNewGame(SelectedWidth, SelectedHeight, SelectedMinePercentage);
        CurrentState = GameState.Game;
    }

    public static string GetGameDataFolder()
    {
        return Path.Combine(SaveFolder, "GameData");
    }

    public static bool ContinueSelectedGame(bool preferSelectedFormat)
    {
        if (!GameScreen.TryLoadSavedGame(preferSelectedFormat))
            return false;

        CurrentState = GameState.Game;
        return true;
    }

    public static bool ContinueSelectedGame(string savePath)
    {
        if (!GameScreen.TryLoadSavedGame(savePath))
            return false;

        CurrentState = GameState.Game;
        return true;
    }

    public static void SaveGameAndReturnMain()
    {
        GameScreen.SaveCurrentGame();
        CurrentState = GameState.Main;
    }

    public void Init()
    {
        LoadPersistedSettings();
        Raylib.InitWindow(900, 900, "Sapper");
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            switch (CurrentState)
            {
                case GameState.Main:
                    MainScreen.Draw();
                    break;
                case GameState.StartMenu:
                    StartScreen.Draw();
                    break;
                case GameState.ContinueMenu:
                    ContinueScreen.Draw();
                    break;
                case GameState.Settings:
                    SettingsScreen.Draw();
                    break;
                case GameState.Game:
                    GameScreen.Draw();
                    break;
                case GameState.Leaderboard:
                    LeaderboardScreen.Draw();
                    break;
                case GameState.EndGame:
                    EndGameScreen.Draw();
                    break;
            }

            Raylib.EndDrawing();
        }

        GameScreen.SaveCurrentGame();
        Raylib.CloseWindow();
    }

    private static void LoadPersistedSettings()
    {
        SettingsSnapshot settings = SettingsStore.Load();

        if (!string.IsNullOrWhiteSpace(settings.SaveFolder))
            SaveFolder = settings.SaveFolder;

        if (!string.IsNullOrWhiteSpace(settings.SelectedSaveFormat))
            SelectedSaveFormat = settings.SelectedSaveFormat;

        if (!string.IsNullOrWhiteSpace(settings.OnlineFetchUrl))
            Liderboard.OnlineFetchUrl = settings.OnlineFetchUrl;

        if (!string.IsNullOrWhiteSpace(settings.OnlineSubmitUrl))
            Liderboard.OnlineSubmitUrl = settings.OnlineSubmitUrl;
    }
}
