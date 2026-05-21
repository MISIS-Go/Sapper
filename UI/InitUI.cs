using Raylib_cs;

namespace UI;

public class InitUI
{
    public static GameState CurrentState = GameState.Main;

    public void Init()
    {
        Raylib.InitWindow(900, 900, "Sapper");
        Raylib.SetTargetFPS(60);

        MainUI mainUI = new MainUI();
        Start startUI = new Start();
        Continue continueUI = new Continue();
        Settings settingsUI = new Settings();
        EndGame endGameUI = new EndGame();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            switch (CurrentState)
            {
                case GameState.Main:
                    mainUI.Draw();
                    break;
                case GameState.StartMenu:
                    startUI.Draw();
                    break;
                case GameState.ContinueMenu:
                    continueUI.Draw();
                    break;
                case GameState.Settings:
                    settingsUI.Draw();
                    break;
                case GameState.Game:
                    CurrentState = GameState.EndGame;
                    break;
                case GameState.Leaderboard:
                    CurrentState = GameState.EndGame;
                    break;
                case GameState.EndGame:
                    endGameUI.Draw();
                    break;
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
