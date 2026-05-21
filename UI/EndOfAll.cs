namespace UI;

using System.Numerics;
using Raylib_cs;

public class End : IUI
{
    public void Draw()
    {
        int btnWidth = 400;
        int btnHeight = 800 / 12;
        int btnX = (900 - btnWidth) / 2;

        Rectangle btnContinue = new Rectangle(btnX, 600, btnWidth, btnHeight);
        Rectangle btnExit = new Rectangle(btnX, 700, btnWidth, btnHeight);
        while (!Raylib.WindowShouldClose())
        {
            Vector2 mousePos = Raylib.GetMousePosition();
            Raylib.BeginDrawing();

            Raylib.DrawText("YOU DIED!", 65, 350, 150, Color.Red);


            Lib.DrawButton(btnContinue, "Continue?", mousePos);
            Lib.DrawButton(btnExit, "EXIT!!!", mousePos);
            Raylib.EndDrawing();
        }
    }
}
