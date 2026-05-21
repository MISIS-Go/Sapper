namespace UI;


using Raylib_cs;
using System.Numerics;

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
        while (!Raylib.WindowShouldClose())
        {
            Vector2 mousePos = Raylib.GetMousePosition();
            Raylib.BeginDrawing();

            Raylib.DrawText("Sapper", 400, 12, 20, Color.Black);
            Raylib.DrawText("High Score: " + BestScore, 370, 60, 20, Color.Black);
            Raylib.DrawText("Made by err and CVVAROG", 610, 878, 20, Color.DarkGray);

            Lib.DrawButton(btnStart, "Start new game", mousePos);
            Lib.DrawButton(btnContinue, "Continue?", mousePos);
            Lib.DrawButton(btnLeaderboard, "Liderboard", mousePos);
            Lib.DrawButton(btnSettings, "Settings", mousePos);

            Raylib.EndDrawing();
        }
    }

}
