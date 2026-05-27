using System;
using System.IO;
using System.Numerics;
using Data;
using Model;
using Raylib_cs;

namespace UI;

public class Settings : IUI
{
    private bool isFormatDropdownOpen = false;
    private string selectedFormat = InitUI.SelectedSaveFormat;
    private readonly string[] formatOptions = { "Json", "Xml" };

    private string selectedSaveFolder = InitUI.SaveFolder;
    private bool isFolderPickerOpen = false;
    private string browsingFolder = InitUI.SaveFolder;
    private string? pickerError;
    private int folderScrollIndex = 0;

    public void Draw()
    {
        int labelX = 70;
        int fieldX = 360;
        int fieldWidth = 440;
        int fieldHeight = 56;
        int fontSize = 22;

        Rectangle rectFolder = new Rectangle(fieldX, 110, fieldWidth, fieldHeight);
        Rectangle rectFormatDrop = new Rectangle(fieldX, 210, fieldWidth, fieldHeight);
        Rectangle rectFetchUrl = new Rectangle(fieldX, 310, fieldWidth, fieldHeight);
        Rectangle rectSubmitUrl = new Rectangle(fieldX, 410, fieldWidth, fieldHeight);

        int bottomBtnWidth = 240;
        int bottomBtnHeight = 70;
        int bottomY = 770;

        Rectangle btnSave = new Rectangle(50, bottomY, bottomBtnWidth, bottomBtnHeight);
        Rectangle btnCancel = new Rectangle(330, bottomY, bottomBtnWidth, bottomBtnHeight);
        Rectangle btnBack = new Rectangle(610, bottomY, bottomBtnWidth, bottomBtnHeight);

        Vector2 mousePos = Raylib.GetMousePosition();
        bool isLeftMouseClicked = Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (Raylib.CheckCollisionPointRec(mousePos, rectFolder) && isLeftMouseClicked)
        {
            OpenFolderPicker();
        }

        if (Raylib.CheckCollisionPointRec(mousePos, rectFormatDrop) && isLeftMouseClicked)
        {
            isFormatDropdownOpen = !isFormatDropdownOpen;
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnSave) && isLeftMouseClicked)
        {
            CommitSettings();
            InitUI.OpenMainMenu();
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnCancel) && isLeftMouseClicked)
        {
            selectedFormat = InitUI.SelectedSaveFormat;
            selectedSaveFolder = InitUI.SaveFolder;
            browsingFolder = selectedSaveFolder;
            isFolderPickerOpen = false;
            pickerError = null;
            InitUI.OpenMainMenu();
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnBack) && isLeftMouseClicked)
        {
            InitUI.OpenMainMenu();
        }

        Raylib.DrawText("Settings", 360, 30, 42, Color.Black);
        Raylib.DrawText("Save folder", labelX, (int)rectFolder.Y + 16, fontSize, Color.Black);
        Raylib.DrawText("Save format", labelX, (int)rectFormatDrop.Y + 16, fontSize, Color.Black);
        Raylib.DrawText("Online fetch URL", labelX, (int)rectFetchUrl.Y + 16, fontSize, Color.Black);
        Raylib.DrawText("Online submit URL", labelX, (int)rectSubmitUrl.Y + 16, fontSize, Color.Black);

        Lib.DrawButton(rectFolder, ShortenPath(selectedSaveFolder), mousePos);
        Lib.DrawButton(rectFormatDrop, selectedFormat, mousePos);
        Lib.DrawButton(rectFetchUrl, ShortenValue(Liderboard.OnlineFetchUrl), mousePos);
        Lib.DrawButton(rectSubmitUrl, ShortenValue(Liderboard.OnlineSubmitUrl), mousePos);

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

        if (isFolderPickerOpen)
        {
            DrawFolderPicker(mousePos, isLeftMouseClicked);
        }

        if (!string.IsNullOrWhiteSpace(pickerError))
        {
            Raylib.DrawText(pickerError, 70, 690, 18, Color.Maroon);
        }
    }

    private void OpenFolderPicker()
    {
        if (!Directory.Exists(selectedSaveFolder))
            selectedSaveFolder = InitUI.SaveFolder;

        browsingFolder = selectedSaveFolder;
        isFolderPickerOpen = true;
        pickerError = null;
        folderScrollIndex = 0;
    }

    private void CommitSettings()
    {
        InitUI.SelectedSaveFormat = selectedFormat;
        InitUI.SaveFolder = selectedSaveFolder;

        SettingsStore.Save(new SettingsSnapshot
        {
            SaveFolder = selectedSaveFolder,
            SelectedSaveFormat = selectedFormat,
            OnlineFetchUrl = Liderboard.OnlineFetchUrl,
            OnlineSubmitUrl = Liderboard.OnlineSubmitUrl
        });
    }

    private void DrawFolderPicker(Vector2 mousePos, bool isLeftMouseClicked)
    {
        Rectangle panel = new Rectangle(30, 455, 840, 300);
        Rectangle btnUp = new Rectangle(50, 700, 110, 42);
        Rectangle btnPrev = new Rectangle(170, 700, 110, 42);
        Rectangle btnNext = new Rectangle(290, 700, 110, 42);
        Rectangle btnUse = new Rectangle(750, 700, 110, 42);

        Raylib.DrawRectangleRec(panel, new Color(250, 250, 248, 255));
        Raylib.DrawRectangleLinesEx(panel, 2, Color.Black);
        Raylib.DrawText("Pick folder", 50, 470, 24, Color.Black);
        Raylib.DrawText(ShortenPath(browsingFolder), 50, 500, 18, Color.DarkGray);

        Lib.DrawButton(btnUp, "Up", mousePos);
        Lib.DrawButton(btnPrev, "Prev", mousePos);
        Lib.DrawButton(btnNext, "Next", mousePos);
        Lib.DrawButton(btnUse, "Use", mousePos);

        if (Raylib.CheckCollisionPointRec(mousePos, btnUp) && isLeftMouseClicked)
        {
            string? parent = Directory.GetParent(browsingFolder)?.FullName;
            if (!string.IsNullOrWhiteSpace(parent))
            {
                browsingFolder = parent;
                pickerError = null;
                folderScrollIndex = 0;
            }
        }

        int visibleRows = 3;
        int columns = 3;
        int pageSize = visibleRows * columns;

        if (Raylib.CheckCollisionPointRec(mousePos, btnUse) && isLeftMouseClicked)
        {
            selectedSaveFolder = browsingFolder;
            isFolderPickerOpen = false;
            pickerError = null;
        }

        string[] directories;
        try
        {
            directories = Directory.GetDirectories(browsingFolder);
        }
        catch (Exception exception)
        {
            pickerError = exception.Message;
            directories = Array.Empty<string>();
        }

        if (directories.Length == 0)
        {
            Raylib.DrawText("No subfolders", 50, 560, 20, Color.DarkGray);
            return;
        }

        int itemWidth = 245;
        int itemHeight = 36;
        int startX = 50;
        int startY = 550;
        int maxScrollIndex = Math.Max(0, directories.Length - pageSize);
        folderScrollIndex = Math.Clamp(folderScrollIndex, 0, maxScrollIndex);

        float wheel = Raylib.GetMouseWheelMove();
        if (Math.Abs(wheel) > 0.01f && Raylib.CheckCollisionPointRec(mousePos, panel))
        {
            folderScrollIndex = Math.Clamp(folderScrollIndex - Math.Sign(wheel) * columns, 0, maxScrollIndex);
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnPrev) && isLeftMouseClicked)
        {
            folderScrollIndex = Math.Max(0, folderScrollIndex - columns);
        }

        if (Raylib.CheckCollisionPointRec(mousePos, btnNext) && isLeftMouseClicked)
        {
            folderScrollIndex = Math.Min(maxScrollIndex, folderScrollIndex + columns);
        }

        int endIndex = Math.Min(directories.Length, folderScrollIndex + pageSize);
        for (int i = folderScrollIndex; i < endIndex; i++)
        {
            int visibleIndex = i - folderScrollIndex;
            int column = visibleIndex % columns;
            int row = visibleIndex / columns;
            Rectangle itemRect = new Rectangle(startX + column * (itemWidth + 10), startY + row * (itemHeight + 10), itemWidth, itemHeight);
            string folderName = Path.GetFileName(directories[i]);
            Lib.DrawButton(itemRect, folderName, mousePos);

            if (Raylib.CheckCollisionPointRec(mousePos, itemRect) && isLeftMouseClicked)
            {
                browsingFolder = directories[i];
                pickerError = null;
                folderScrollIndex = 0;
            }
        }

        string pageLabel = $"{folderScrollIndex + 1}-{endIndex} of {directories.Length}";
        Raylib.DrawText(pageLabel, 430, 705, 18, Color.DarkGray);
    }

    private static string ShortenPath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Not set";

        if (value.Length <= 38)
            return value;

        return "..." + value[^35..];
    }

    private static string ShortenValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Not set";

        if (value.Length <= 38)
            return value;

        return "..." + value[^35..];
    }
}
