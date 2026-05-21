namespace UI;

using System.Numerics;
using Raylib_cs;

public class Lib
{

    public static void DrawButton(Rectangle rect, string text, Vector2 mousePos)
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
