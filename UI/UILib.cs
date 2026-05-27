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

        int fontSize = Math.Min(24, Math.Max(14, (int)(rect.Height * 0.46f)));
        string fittedText = FitText(text, fontSize, (int)rect.Width - 16);
        int textWidth = Raylib.MeasureText(fittedText, fontSize);

        int textX = (int)(rect.X + (rect.Width - textWidth) / 2);
        int textY = (int)(rect.Y + (rect.Height - fontSize) / 2) - 1;

        Raylib.DrawText(fittedText, textX, textY, fontSize, fontColor);
    }

    private static string FitText(string text, int fontSize, int maxWidth)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        if (Raylib.MeasureText(text, fontSize) <= maxWidth)
            return text;

        string ellipsis = "...";
        if (Raylib.MeasureText(ellipsis, fontSize) > maxWidth)
            return string.Empty;

        string candidate = text;
        while (candidate.Length > 0 && Raylib.MeasureText(candidate + ellipsis, fontSize) > maxWidth)
        {
            candidate = candidate[..^1];
        }

        return candidate.Length == 0 ? ellipsis : candidate + ellipsis;
    }
}
