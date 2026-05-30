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
        int bottomBtnWidth = 240;
        int bottomBtnHeight = 70;
        int bottomY = 650;

        Rectangle btnSave = new Rectangle(50, bottomY, bottomBtnWidth, bottomBtnHeight);
        Rectangle btnCancel = new Rectangle(330, bottomY, bottomBtnWidth, bottomBtnHeight);
        Rectangle btnBack = new Rectangle(610, bottomY, bottomBtnWidth, bottomBtnHeight);

        Vector2 mousePos = Raylib.GetMousePosition();
        bool isLeftMouseClicked = Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (Lib.IsClicked(rectFolder, mousePos, isLeftMouseClicked))
        {
            OpenFolderPicker();
        }

        if (Lib.IsClicked(rectFormatDrop, mousePos, isLeftMouseClicked))
        {
            isFormatDropdownOpen = !isFormatDropdownOpen;
        }

        if (Lib.IsClicked(btnSave, mousePos, isLeftMouseClicked))
        {
            CommitSettings();
            InitUI.OpenMainMenu();
        }

        if (Lib.IsClicked(btnCancel, mousePos, isLeftMouseClicked))
        {
            selectedFormat = InitUI.SelectedSaveFormat;
            selectedSaveFolder = InitUI.SaveFolder;
            browsingFolder = selectedSaveFolder;
            isFolderPickerOpen = false;
            pickerError = null;
            InitUI.OpenMainMenu();
        }

        if (Lib.IsClicked(btnBack, mousePos, isLeftMouseClicked))
        {
            InitUI.OpenMainMenu();
        }

        Raylib.DrawText("Settings", 360, 30, 42, Color.Black);
        Raylib.DrawText("Save folder", labelX, (int)rectFolder.Y + 16, fontSize, Color.Black);
        Raylib.DrawText("Save format", labelX, (int)rectFormatDrop.Y + 16, fontSize, Color.Black);

        Lib.DrawButton(rectFolder, Lib.ShortenPath(selectedSaveFolder), mousePos);
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

                if (Lib.IsClicked(optionRect, mousePos, isLeftMouseClicked))
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
        string previousGameDataFolder = InitUI.GetGameDataFolder();
        string previousFormat = InitUI.SelectedSaveFormat;
        InitUI.SelectedSaveFormat = selectedFormat;
        InitUI.SaveFolder = selectedSaveFolder;

        GameSaveStore.CopyFilesWithFormatChange(previousGameDataFolder, previousFormat, selectedFormat);

        SettingsStore.Save(new SettingsSnapshot
        {
            SaveFolder = selectedSaveFolder,
            SelectedSaveFormat = selectedFormat
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
        Raylib.DrawText(Lib.ShortenPath(browsingFolder), 50, 500, 18, Color.DarkGray);

        Lib.DrawButton(btnUp, "Up", mousePos);
        Lib.DrawButton(btnPrev, "Prev", mousePos);
        Lib.DrawButton(btnNext, "Next", mousePos);
        Lib.DrawButton(btnUse, "Use", mousePos);

        if (Lib.IsClicked(btnUp, mousePos, isLeftMouseClicked))
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

        if (Lib.IsClicked(btnUse, mousePos, isLeftMouseClicked))
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

        float wheel = Raylib.GetMouseWheelMove();
        folderScrollIndex = Lib.UpdateGridScrollIndex(
            folderScrollIndex,
            directories.Length,
            pageSize,
            columns,
            wheel,
            Raylib.CheckCollisionPointRec(mousePos, panel),
            Lib.IsClicked(btnPrev, mousePos, isLeftMouseClicked),
            Lib.IsClicked(btnNext, mousePos, isLeftMouseClicked));

        int endIndex = Math.Min(directories.Length, folderScrollIndex + pageSize);
        for (int i = folderScrollIndex; i < endIndex; i++)
        {
            int visibleIndex = i - folderScrollIndex;
            int column = visibleIndex % columns;
            int row = visibleIndex / columns;
            Rectangle itemRect = new Rectangle(startX + column * (itemWidth + 10), startY + row * (itemHeight + 10), itemWidth, itemHeight);
            string folderName = Path.GetFileName(directories[i]);
            Lib.DrawButton(itemRect, folderName, mousePos);

            if (Lib.IsClicked(itemRect, mousePos, isLeftMouseClicked))
            {
                browsingFolder = directories[i];
                pickerError = null;
                folderScrollIndex = 0;
            }
        }

        string pageLabel = $"{folderScrollIndex + 1}-{endIndex} of {directories.Length}";
        Raylib.DrawText(pageLabel, 430, 705, 18, Color.DarkGray);
    }

}
