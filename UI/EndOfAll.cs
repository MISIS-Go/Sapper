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

        if (Lib.IsClicked(btnContinue, mousePos, isLeftMouseClicked))
        {
            InitUI.StartSelectedGame();
        }

        if (Lib.IsClicked(btnExit, mousePos, isLeftMouseClicked))
        {
            InitUI.OpenMainMenu();
        }

        string title = InitUI.GameScreen.LastResultWasWin ? "YOU WIN" : "YOU DIED";
        Color titleColor = InitUI.GameScreen.LastResultWasWin ? Color.DarkGreen : Color.Red;
        Raylib.DrawText(title, 300, 220, 64, titleColor);
        Raylib.DrawText($"Time: {Lib.FormatTime(InitUI.GameScreen.LastElapsedSeconds)}", 340, 320, 28, Color.Black);

        Lib.DrawButton(btnContinue, "Play again", mousePos);
        Lib.DrawButton(btnExit, "Main menu", mousePos);
    }

}
