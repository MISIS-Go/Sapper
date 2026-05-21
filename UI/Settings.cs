using System.Numerics;
using Raylib_cs;

namespace UI;

public class Settings : IUI
{
    private bool isFormatDropdownOpen = false;
    private string selectedFormat = "Json";
    private string[] formatOptions = { "Json", "Xml" };

    private string username = "Some";
    private string appData = "Some";
    private string pathToSettings = "Some";

    public void Draw()
    {
        int labelX = 50;
        int fieldX = 400;
        int fieldWidth = 350;
        int fieldHeight = 60;
        int fontSize = 24;

        Rectangle rectUsername = new Rectangle(fieldX, 80, fieldWidth, fieldHeight);
        Rectangle rectAppData = new Rectangle(fieldX, 200, fieldWidth, fieldHeight);
        Rectangle rectPath = new Rectangle(fieldX, 320, fieldWidth, fieldHeight);
        Rectangle rectFormatDrop = new Rectangle(fieldX, 440, fieldWidth, fieldHeight);

        int bottomBtnWidth = 240;
        int bottomBtnHeight = 70;
        int bottomY = 750;

        Rectangle btnSave = new Rectangle(50, bottomY, bottomBtnWidth, bottomBtnHeight);
        Rectangle btnCancel = new Rectangle(330, bottomY, bottomBtnWidth, bottomBtnHeight);
        Rectangle btnBack = new Rectangle(610, bottomY, bottomBtnWidth, bottomBtnHeight);

        Vector2 mousePos = Raylib.GetMousePosition();
        bool isLeftMouseClicked = Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (Raylib.CheckCollisionPointRec(mousePos, rectFormatDrop) && isLeftMouseClicked)
        {
            isFormatDropdownOpen = !isFormatDropdownOpen;
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnSave) && isLeftMouseClicked)
        {
            // TODO: Сохранение в файл
            InitUI.CurrentState = GameState.Main;
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnCancel) && isLeftMouseClicked)
        {
            InitUI.CurrentState = GameState.Main;
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnBack) && isLeftMouseClicked)
        {
            InitUI.CurrentState = GameState.Main;
        }

        Raylib.DrawText("Username:", labelX, (int)rectUsername.Y + 15, fontSize, Color.Black);
        Raylib.DrawText("App data:", labelX, (int)rectAppData.Y + 15, fontSize, Color.Black);
        Raylib.DrawText("Path to settings:", labelX, (int)rectPath.Y + 15, fontSize, Color.Black);
        Raylib.DrawText("Json or Xml:", labelX, (int)rectFormatDrop.Y + 15, fontSize, Color.Black);

        Lib.DrawButton(rectUsername, username, mousePos);
        Lib.DrawButton(rectAppData, appData, mousePos);
        Lib.DrawButton(rectPath, pathToSettings, mousePos);
        Lib.DrawButton(rectFormatDrop, selectedFormat, mousePos);

        Lib.DrawButton(btnSave, "Save", mousePos);
        Lib.DrawButton(btnCancel, "Cancel", mousePos);
        Lib.DrawButton(btnBack, "Back", mousePos);

        if (isFormatDropdownOpen)
        {
            for (int i = 0; i < formatOptions.Length; i++)
            {
                Rectangle optionRect = new Rectangle(rectFormatDrop.X, rectFormatDrop.Y + rectFormatDrop.Height * (i + 1), rectFormatDrop.Width, rectFormatDrop.Height);
                Lib.DrawButton(optionRect, formatOptions[i], mousePos);

                if (Raylib.CheckCollisionPointRec(mousePos, optionRect) && isLeftMouseClicked)
                {
                    selectedFormat = formatOptions[i];
                    isFormatDropdownOpen = false;
                }
            }
        }
    }
}
