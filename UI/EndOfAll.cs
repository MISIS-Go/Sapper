using System.Numerics;
using Model;
using Raylib_cs;

namespace UI;

public class EndGame : IUI
{
    public void Draw()
    {
        int btnWidth = 300;
        int btnHeight = 68;
        int btnX = (900 - btnWidth) / 2;

        Rectangle btnContinue = new Rectangle(btnX, 470, btnWidth, btnHeight);
        Rectangle btnExit = new Rectangle(btnX, 560, btnWidth, btnHeight);

        Vector2 mousePos = Raylib.GetMousePosition();
        bool isLeftMouseClicked = Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (Raylib.CheckCollisionPointRec(mousePos, btnContinue) && isLeftMouseClicked)
        {
            InitUI.StartSelectedGame();
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnExit) && isLeftMouseClicked)
        {
            InitUI.OpenMainMenu();
        }

        string title = InitUI.GameScreen.LastResultWasWin ? "YOU WIN" : "YOU DIED";
        Color titleColor = InitUI.GameScreen.LastResultWasWin ? Color.DarkGreen : Color.Red;
        Raylib.DrawText(title, 300, 220, 64, titleColor);
        Raylib.DrawText($"Time: {FormatTime(InitUI.GameScreen.LastElapsedSeconds)}", 340, 320, 28, Color.Black);

        Lib.DrawButton(btnContinue, "Play again", mousePos);
        Lib.DrawButton(btnExit, "Main menu", mousePos);
    }

    private static string FormatTime(int seconds)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return $"{minutes:00}:{remainingSeconds:00}";
    }
}
