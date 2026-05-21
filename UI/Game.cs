namespace UI;

using System.Numerics;
using Raylib_cs;

public class Game : IUI
{
    public void Draw()
    {

        //  Прокрастинация наше все! Работаем как haslell

        while (!Raylib.WindowShouldClose())
        {
            Raylib.DrawText("YOU DIED!", 65, 350, 150, Color.Red);
            Raylib.EndDrawing();
        }
    }
}
