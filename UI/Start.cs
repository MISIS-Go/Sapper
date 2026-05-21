using System.Numerics;
using Raylib_cs;

namespace UI;

public class Start : IUI
{
    private bool isSizeDropdownOpen = false;
    private string selectedSize = "5x5";
    private string[] sizeOptions = { "5x5", "5x10", "10x10" };

    private bool isDifficultyDropdownOpen = false;
    private string selectedDifficulty = "Hardcore";
    private string[] difficultyOptions = { "Normal", "Hard", "Expert" };

    public void Draw()
    {
        int dropWidth = 320;
        int dropHeight = 800 / 12;

        Rectangle rectSizeDrop = new Rectangle(110, 200, dropWidth, dropHeight);
        Rectangle rectDiffDrop = new Rectangle(470, 200, dropWidth, dropHeight);

        int btnWidth = 400;
        int btnHeight = 800 / 12;
        int btnX = (900 - btnWidth) / 2;
        Rectangle btnStart = new Rectangle(btnX, 400, btnWidth, btnHeight * 2);
        Rectangle btnBack = new Rectangle(btnX, 650, btnWidth, btnHeight);

        Vector2 mousePos = Raylib.GetMousePosition();
        bool isLeftMouseClicked = Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (Raylib.CheckCollisionPointRec(mousePos, rectSizeDrop) && isLeftMouseClicked)
        {
            isSizeDropdownOpen = !isSizeDropdownOpen;
            isDifficultyDropdownOpen = false;
        }

        if (Raylib.CheckCollisionPointRec(mousePos, rectDiffDrop) && isLeftMouseClicked)
        {
            isDifficultyDropdownOpen = !isDifficultyDropdownOpen;
            isSizeDropdownOpen = false;
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnStart) && isLeftMouseClicked)
        {
            InitUI.CurrentState = GameState.Game;
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnBack) && isLeftMouseClicked)
        {
            InitUI.CurrentState = GameState.Main;
        }

        Lib.DrawButton(btnStart, "Start", mousePos);
        Lib.DrawButton(btnBack, "Back", mousePos);

        Lib.DrawButton(rectSizeDrop, selectedSize, mousePos);
        Lib.DrawButton(rectDiffDrop, selectedDifficulty, mousePos);

        if (isSizeDropdownOpen)
        {
            for (int i = 0; i < sizeOptions.Length; i++)
            {
                Rectangle optionRect = new Rectangle(rectSizeDrop.X, rectSizeDrop.Y + rectSizeDrop.Height * (i + 1), rectSizeDrop.Width, rectSizeDrop.Height);
                Lib.DrawButton(optionRect, sizeOptions[i], mousePos);

                if (Raylib.CheckCollisionPointRec(mousePos, optionRect) && isLeftMouseClicked)
                {
                    selectedSize = sizeOptions[i];
                    isSizeDropdownOpen = false;
                }
            }
        }

        if (isDifficultyDropdownOpen)
        {
            for (int i = 0; i < difficultyOptions.Length; i++)
            {
                Rectangle optionRect = new Rectangle(rectDiffDrop.X, rectDiffDrop.Y + rectDiffDrop.Height * (i + 1), rectDiffDrop.Width, rectDiffDrop.Height);
                Lib.DrawButton(optionRect, difficultyOptions[i], mousePos);

                if (Raylib.CheckCollisionPointRec(mousePos, optionRect) && isLeftMouseClicked)
                {
                    selectedDifficulty = difficultyOptions[i];
                    isDifficultyDropdownOpen = false;
                }
            }
        }
    }
}
