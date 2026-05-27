using System.Numerics;
using Core;
using Model;
using Raylib_cs;

namespace UI;

public class Start : IUI
{
    private bool isSizeDropdownOpen = false;
    private string selectedSize = "5x5";
    private readonly string[] sizeOptions = { "5x5", "5x10", "10x10", "12x12", "15x10", "15x15", "20x15", "20x20" };

    private bool isDifficultyDropdownOpen = false;
    private string selectedDifficulty = "Normal";
    private readonly string[] difficultyOptions = { "Normal", "Expert", "Nightmare" };

    public void Draw()
    {
        int dropWidth = 300;
        int dropHeight = 54;

        Rectangle rectSizeDrop = new Rectangle(110, 200, dropWidth, dropHeight);
        Rectangle rectDiffDrop = new Rectangle(490, 200, dropWidth, dropHeight);

        int btnWidth = 360;
        int btnHeight = 68;
        int btnX = (900 - btnWidth) / 2;
        Rectangle btnStart = new Rectangle(btnX, 430, btnWidth, btnHeight);
        Rectangle btnBack = new Rectangle(btnX, 520, btnWidth, btnHeight);

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
            ApplySelection();
            InitUI.StartSelectedGame();
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnBack) && isLeftMouseClicked)
        {
            InitUI.OpenMainMenu();
        }

        Raylib.DrawText("New game", 350, 90, 40, Color.Black);
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

    private void ApplySelection()
    {
        (int width, int height) = ParseSize(selectedSize);
        InitUI.SelectedWidth = width;
        InitUI.SelectedHeight = height;
        InitUI.SelectedDifficultyName = selectedDifficulty;
        InitUI.SelectedMinePercentage = selectedDifficulty switch
        {
            "Normal" => Core.Difficulty.Normal,
            "Expert" => Core.Difficulty.Expert,
            "Nightmare" => Core.Difficulty.Nightmare,
            _ => Core.Difficulty.Normal
        };
    }

    private static (int width, int height) ParseSize(string value)
    {
        string[] parts = value.Split('x', 'X');
        if (parts.Length != 2)
            return (5, 5);

        if (!int.TryParse(parts[0], out int width))
            width = 5;

        if (!int.TryParse(parts[1], out int height))
            height = 5;

        return (Math.Max(5, width), Math.Max(5, height));
    }
}
