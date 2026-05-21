using Raylib_cs;
using System.Numerics;
using UI;

namespace Sapper;

class Program
{
    [System.STAThread]
    public static void Main()
    {
        Raylib.InitWindow(900, 900, "Sapper");
        Raylib.SetTargetFPS(60);
        Raylib.ClearBackground(Color.White);
        MainUI.Draw();
        Raylib.CloseWindow();
    }


}
