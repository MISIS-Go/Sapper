namespace UI;

using System.Numerics;
using Raylib_cs;

public class Lib
{

    public static bool IsClicked(Rectangle rect, Vector2 mousePos, bool isLeftMouseClicked)
    {
        return isLeftMouseClicked && Raylib.CheckCollisionPointRec(mousePos, rect);
    }

    public static int UpdateGridScrollIndex(
        int currentIndex,
        int itemCount,
        int pageSize,
        int step,
        float wheelDelta,
        bool isMouseInsidePanel,
        bool isPrevClicked,
        bool isNextClicked)
    {
        int maxScrollIndex = Math.Max(0, itemCount - pageSize);
        int scrollIndex = Math.Clamp(currentIndex, 0, maxScrollIndex);

        if (Math.Abs(wheelDelta) > 0.01f && isMouseInsidePanel)
        {
            scrollIndex = Math.Clamp(scrollIndex - Math.Sign(wheelDelta) * step, 0, maxScrollIndex);
        }

        if (isPrevClicked)
        {
            scrollIndex = Math.Max(0, scrollIndex - step);
        }

        if (isNextClicked)
        {
            scrollIndex = Math.Min(maxScrollIndex, scrollIndex + step);
        }

        return scrollIndex;
    }

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

    public static string FitText(string text, int fontSize, int maxWidth)
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

    public static string ShortenPath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Not set";

        if (value.Length <= 38)
            return value;

        return "..." + value[^35..];
    }

    public static string FormatTime(int seconds)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return $"{minutes:00}:{remainingSeconds:00}";
    }
}
