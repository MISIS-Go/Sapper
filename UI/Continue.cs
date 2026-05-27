using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Data;
using Model;
using Raylib_cs;

namespace UI;

public class Continue : IUI
{
     private bool isFilePickerOpen = false;
     private string browsingFolder = InitUI.GetGameDataFolder();
     private int fileScrollIndex = 0;
     private string? pickerError;

     public void Draw()
     {
          int btnWidth = 400;
          int btnHeight = 68;
          int btnX = (900 - btnWidth) / 2;

          Rectangle btnContinueLast = new Rectangle(btnX, 230, btnWidth, btnHeight);
          Rectangle btnLoadFromFile = new Rectangle(btnX, 320, btnWidth, btnHeight);

          int backWidth = 220;
          int backX = (900 - backWidth) / 2;
          Rectangle btnBack = new Rectangle(backX, 700, backWidth, btnHeight);

          Vector2 mousePos = Raylib.GetMousePosition();
          bool isLeftMouseClicked = Raylib.IsMouseButtonPressed(MouseButton.Left);

          if (!isFilePickerOpen)
          {
               if (Raylib.CheckCollisionPointRec(mousePos, btnContinueLast) && isLeftMouseClicked)
               {
                    InitUI.ContinueSelectedGame(true);
               }

               if (Raylib.CheckCollisionPointRec(mousePos, btnLoadFromFile) && isLeftMouseClicked)
               {
                    OpenFilePicker();
               }

               if (Raylib.CheckCollisionPointRec(mousePos, btnBack) && isLeftMouseClicked)
               {
                    InitUI.OpenMainMenu();
               }

               Raylib.DrawText("Continue", 370, 100, 42, Color.Black);
               Lib.DrawButton(btnContinueLast, "Continue latest", mousePos);
               Lib.DrawButton(btnLoadFromFile, "Load from GameData", mousePos);
               Lib.DrawButton(btnBack, "Back", mousePos);

               if (!string.IsNullOrWhiteSpace(InitUI.GameScreen.StatusMessage))
               {
                    Raylib.DrawText(InitUI.GameScreen.StatusMessage, 120, 420, 22, Color.Maroon);
               }

               return;
          }

          DrawFilePicker(mousePos, isLeftMouseClicked);
     }

     private void OpenFilePicker()
     {
          browsingFolder = InitUI.GetGameDataFolder();

          isFilePickerOpen = true;
          fileScrollIndex = 0;
          pickerError = null;
     }

     private void DrawFilePicker(Vector2 mousePos, bool isLeftMouseClicked)
     {
          Rectangle panel = new Rectangle(30, 455, 840, 300);
          Rectangle btnPrev = new Rectangle(50, 700, 110, 42);
          Rectangle btnNext = new Rectangle(170, 700, 110, 42);
          Rectangle btnClose = new Rectangle(750, 700, 110, 42);

          Raylib.DrawRectangleRec(panel, new Color(250, 250, 248, 255));
          Raylib.DrawRectangleLinesEx(panel, 2, Color.Black);
          Raylib.DrawText("Pick save file", 50, 470, 24, Color.Black);
          Raylib.DrawText(ShortenPath(browsingFolder), 50, 500, 18, Color.DarkGray);

          Lib.DrawButton(btnPrev, "Prev", mousePos);
          Lib.DrawButton(btnNext, "Next", mousePos);
          Lib.DrawButton(btnClose, "Close", mousePos);

          if (Raylib.CheckCollisionPointRec(mousePos, btnClose) && isLeftMouseClicked)
          {
               isFilePickerOpen = false;
               pickerError = null;
               return;
          }

          List<FileEntry> files = GetFiles(browsingFolder);
          if (files.Count == 0)
          {
               Raylib.DrawText("No save files", 50, 560, 20, Color.DarkGray);
               if (!string.IsNullOrWhiteSpace(pickerError))
                    Raylib.DrawText(pickerError, 50, 590, 18, Color.Maroon);
               return;
          }

          int columns = 3;
          int visibleRows = 3;
          int pageSize = columns * visibleRows;
          int itemWidth = 245;
          int itemHeight = 36;
          int startX = 50;
          int startY = 550;

          int maxScrollIndex = Math.Max(0, files.Count - pageSize);
          fileScrollIndex = Math.Clamp(fileScrollIndex, 0, maxScrollIndex);

          float wheel = Raylib.GetMouseWheelMove();
          if (Math.Abs(wheel) > 0.01f && Raylib.CheckCollisionPointRec(mousePos, panel))
          {
               fileScrollIndex = Math.Clamp(fileScrollIndex - Math.Sign(wheel) * columns, 0, maxScrollIndex);
          }

          if (Raylib.CheckCollisionPointRec(mousePos, btnPrev) && isLeftMouseClicked)
          {
               fileScrollIndex = Math.Max(0, fileScrollIndex - columns);
          }

          if (Raylib.CheckCollisionPointRec(mousePos, btnNext) && isLeftMouseClicked)
          {
               fileScrollIndex = Math.Min(maxScrollIndex, fileScrollIndex + columns);
          }

          int endIndex = Math.Min(files.Count, fileScrollIndex + pageSize);
          for (int i = fileScrollIndex; i < endIndex; i++)
          {
               int visibleIndex = i - fileScrollIndex;
               int column = visibleIndex % columns;
               int row = visibleIndex / columns;
               Rectangle itemRect = new Rectangle(startX + column * (itemWidth + 10), startY + row * (itemHeight + 10), itemWidth, itemHeight);
               FileEntry file = files[i];

               DrawFileButton(itemRect, file.DisplayName, mousePos, file.IsValid);

               if (file.IsValid && Raylib.CheckCollisionPointRec(mousePos, itemRect) && isLeftMouseClicked)
               {
                    if (InitUI.ContinueSelectedGame(file.Path))
                         return;

                    pickerError = InitUI.GameScreen.StatusMessage;
               }
          }

          string pageLabel = $"{fileScrollIndex + 1}-{endIndex} of {files.Count}";
          Raylib.DrawText(pageLabel, 430, 705, 18, Color.DarkGray);

          if (!string.IsNullOrWhiteSpace(pickerError))
               Raylib.DrawText(pickerError, 50, 590, 18, Color.Maroon);
     }

     private static List<FileEntry> GetFiles(string folder)
     {
          var files = new List<FileEntry>();

          try
          {
               if (!Directory.Exists(folder))
                    return files;

               foreach (string path in Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly))
               {
                    if (!IsSupportedExtension(Path.GetExtension(path)))
                         continue;

                    bool contentValid = GameSaveStore.Load(path) is not null;
                    files.Add(new FileEntry(path, Path.GetFileName(path), contentValid));
               }

               files.Sort((left, right) => File.GetLastWriteTimeUtc(right.Path).CompareTo(File.GetLastWriteTimeUtc(left.Path)));
          }
          catch
          {
          }

          return files;
     }

     private static bool IsSupportedExtension(string extension)
     {
          return extension.Equals(".json", StringComparison.OrdinalIgnoreCase) || extension.Equals(".xml", StringComparison.OrdinalIgnoreCase);
     }

     private static void DrawFileButton(Rectangle rect, string text, Vector2 mousePos, bool isValid)
     {
          bool isHovered = Raylib.CheckCollisionPointRec(mousePos, rect);
          Color borderColor = isValid ? (isHovered ? Color.Blue : Color.Black) : Color.Maroon;
          Color fontColor = isValid ? (isHovered ? Color.Blue : Color.Black) : Color.Maroon;
          Color backgroundColor = isValid ? Color.White : new Color(255, 235, 235, 255);

          Raylib.DrawRectangleRec(rect, backgroundColor);
          Raylib.DrawRectangleLinesEx(rect, 2, borderColor);

          int fontSize = Math.Min(24, Math.Max(14, (int)(rect.Height * 0.46f)));
          string fittedText = FitText(text, fontSize, (int)rect.Width - 16);
          int textWidth = Raylib.MeasureText(fittedText, fontSize);
          int textX = (int)(rect.X + (rect.Width - textWidth) / 2);
          int textY = (int)(rect.Y + (rect.Height - fontSize) / 2) - 1;

          Raylib.DrawText(fittedText, textX, textY, fontSize, fontColor);
     }

     private static string FitText(string text, int fontSize, int maxWidth)
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

     private static string ShortenPath(string value)
     {
          if (string.IsNullOrWhiteSpace(value))
               return "Not set";

          if (value.Length <= 38)
               return value;

          return "..." + value[^35..];
     }

     private sealed record FileEntry(string Path, string DisplayName, bool IsValid);
}
