using UI;

namespace Sapper;

class Program
{
    [System.STAThread]
    public static void Main()
    {
        InitUI initUi = new InitUI();
        initUi.Init();
    }


}
