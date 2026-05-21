using System.Numerics;
using Raylib_cs;

namespace UI;

public class MainUI : IUI
{
    public void Draw()
    {
        var BestScore = 0;
        int btnWidth = 400;
        int btnHeight = 800 / 12;
        int btnX = (900 - btnWidth) / 2;

        Rectangle btnStart = new Rectangle(btnX, 200, btnWidth, btnHeight);
        Rectangle btnContinue = new Rectangle(btnX, 300, btnWidth, btnHeight);
        Rectangle btnLeaderboard = new Rectangle(btnX, 400, btnWidth, btnHeight);
        Rectangle btnSettings = new Rectangle(btnX, 500, btnWidth, btnHeight);

        Vector2 mousePos = Raylib.GetMousePosition();
        bool isLeftMouseClicked = Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (Raylib.CheckCollisionPointRec(mousePos, btnStart) && isLeftMouseClicked)
        {
            InitUI.CurrentState = GameState.StartMenu;
        }
        if (Raylib.CheckCollisionPointRec(mousePos, btnContinue) && isLeftMouseClicked)
        {
            InitUI.CurrentState = GameState.ContinueMenu;
        }
        if (Raylib.CheckCollisionPointRec(mousePos, btnSettings) && isLeftMouseClicked)
        {
            InitUI.CurrentState = GameState.Settings;
        }
        if (Raylib.CheckCollisionPointRec(mousePos, btnLeaderboard) && isLeftMouseClicked)
        {
            InitUI.CurrentState = GameState.Leaderboard;
        }

        Raylib.DrawText("Sapper", 400, 12, 20, Color.Black);
        Raylib.DrawText("High Score: " + BestScore, 370, 60, 20, Color.Black);
        Raylib.DrawText("Made by err and CVVAROG", 610, 878, 20, Color.DarkGray);

        Lib.DrawButton(btnStart, "Start new game", mousePos);
        Lib.DrawButton(btnContinue, "Continue?", mousePos);
        Lib.DrawButton(btnLeaderboard, "Leaderboard", mousePos);
        Lib.DrawButton(btnSettings, "Settings", mousePos);
    }
}
