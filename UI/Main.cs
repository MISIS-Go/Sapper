using System.Numerics;
using Model;
using Raylib_cs;

namespace UI;

public class MainUI : IUI
{
    public void Draw()
    {
        int btnWidth = 360;
        int btnHeight = 68;
        int btnX = (900 - btnWidth) / 2;
        int startY = 220;

        Rectangle btnStart = new Rectangle(btnX, startY, btnWidth, btnHeight);
        Rectangle btnContinue = new Rectangle(btnX, startY + 90, btnWidth, btnHeight);
        Rectangle btnLeaderboard = new Rectangle(btnX, startY + 180, btnWidth, btnHeight);
        Rectangle btnSettings = new Rectangle(btnX, startY + 270, btnWidth, btnHeight);

        Vector2 mousePos = Raylib.GetMousePosition();
        bool isLeftMouseClicked = Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (Lib.IsClicked(btnStart, mousePos, isLeftMouseClicked))
        {
            InitUI.OpenStartMenu();
        }

        if (Lib.IsClicked(btnContinue, mousePos, isLeftMouseClicked))
        {
            InitUI.OpenContinueMenu();
        }

        if (Lib.IsClicked(btnLeaderboard, mousePos, isLeftMouseClicked))
        {
            InitUI.OpenLeaderboardMenu();
        }

        if (Lib.IsClicked(btnSettings, mousePos, isLeftMouseClicked))
        {
            InitUI.OpenSettingsMenu();
        }

        Raylib.DrawText("Sapper", 350, 70, 48, Color.Black);
        Raylib.DrawText($"Best score: {Liderboard.GetBestLocalScore()}", 330, 130, 24, Color.DarkGray);
        Raylib.DrawText("Made by err and CVVAROG", 610, 878, 20, Color.DarkGray);

        Lib.DrawButton(btnStart, "New game", mousePos);
        Lib.DrawButton(btnContinue, "Continue", mousePos);
        Lib.DrawButton(btnLeaderboard, "Liderboard", mousePos);
        Lib.DrawButton(btnSettings, "Settings", mousePos);
    }
}
