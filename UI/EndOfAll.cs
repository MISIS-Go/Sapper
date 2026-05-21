using System.Numerics;
using Raylib_cs;

namespace UI;

public class EndGame : IUI
{
    public void Draw()
    {
        int btnWidth = 300;
        int btnHeight = 70;
        int btnX = (900 - btnWidth) / 2;

        Rectangle btnContinue = new Rectangle(btnX, 450, btnWidth, btnHeight);
        Rectangle btnExit = new Rectangle(btnX, 560, btnWidth, btnHeight);

        Vector2 mousePos = Raylib.GetMousePosition();
        bool isLeftMouseClicked = Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (Raylib.CheckCollisionPointRec(mousePos, btnContinue) && isLeftMouseClicked)
        {
            InitUI.CurrentState = GameState.StartMenu;
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnExit) && isLeftMouseClicked)
        {
            InitUI.CurrentState = GameState.Main;
        }

        Raylib.DrawText("YOU DIED", 280, 250, 64, Color.Red);

        Lib.DrawButton(btnContinue, "Continue?", mousePos);
        Lib.DrawButton(btnExit, "EXIT!!!", mousePos);
    }
}
