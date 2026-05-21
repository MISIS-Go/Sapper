using Raylib_cs;
using System.Numerics;


namespace Sapper;

class Program
{
    [System.STAThread]
    public static void Main()
    {
        Raylib.InitWindow(900, 900, "Sapper");
        Raylib.SetTargetFPS(60);


        var BestScore = 0;
        Raylib.ClearBackground(Color.White);

        //Font customFont = Raylib.LoadFontEx("resources/GrapeNuts-Regular.ttf", 48, null, 0);
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

            DrawButton(btnStart, "Start new game", mousePos);
            DrawButton(btnContinue, "Continue?", mousePos);
            DrawButton(btnLeaderboard, "Liderboard", mousePos);
            DrawButton(btnSettings, "Settings", mousePos);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    private static void DrawButton(Rectangle rect, string text, Vector2 mousePos)
    {
        bool isHovered = Raylib.CheckCollisionPointRec(mousePos, rect);

        Color borderColor = isHovered ? Color.Blue : Color.Black;
        Color fontColor = isHovered ? Color.Blue : Color.Black;

        Raylib.DrawRectangleRec(rect, Color.White);
        Raylib.DrawRectangleLinesEx(rect, 2, borderColor);

        int fontSize = 24;
        int textWidth = Raylib.MeasureText(text, fontSize);

        int textX = (int)(rect.X + (rect.Width - textWidth) / 2);
        int textY = (int)(rect.Y + (rect.Height - fontSize) / 2);

        Raylib.DrawText(text, textX, textY, fontSize, fontColor);
    }
}
