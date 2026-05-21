using Raylib_cs;

namespace UI;

public class InitUI
{
    public void Init()
    {
        Raylib.InitWindow(900, 900, "Sapper");
        Raylib.SetTargetFPS(60);
        Raylib.ClearBackground(Color.White);

        End mainUi = new End();
        mainUi.Draw();

        Raylib.CloseWindow();
    }
}
