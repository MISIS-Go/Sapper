using System.Numerics;
using Raylib_cs;

namespace UI;

public class Continue : IUI
{
    public void Draw()
    {
        int btnWidth = 450;
        int btnHeight = 800 / 12;
        int btnX = (900 - btnWidth) / 2;

        Rectangle btnContinueLast = new Rectangle(btnX, 200, btnWidth, btnHeight);
        Rectangle btnLoadFromFile = new Rectangle(btnX, 450, btnWidth, btnHeight);

        int backWidth = 250;
        int backX = (900 - backWidth) / 2;
        Rectangle btnBack = new Rectangle(backX, 700, backWidth, btnHeight);

        Vector2 mousePos = Raylib.GetMousePosition();
        bool isLeftMouseClicked = Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (Raylib.CheckCollisionPointRec(mousePos, btnContinueLast) && isLeftMouseClicked)
        {
            // TODO: Загрузка последнего сохранения
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnLoadFromFile) && isLeftMouseClicked)
        {
            // TODO: Загрузка из файла
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnBack) && isLeftMouseClicked)
        {
            InitUI.CurrentState = GameState.Main;
        }

        Lib.DrawButton(btnContinueLast, "Continue last game?", mousePos);
        Lib.DrawButton(btnLoadFromFile, "Load from file", mousePos);
        Lib.DrawButton(btnBack, "Back", mousePos);
    }
}
