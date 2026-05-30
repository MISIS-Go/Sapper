using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Numerics;
using Model;
using Newtonsoft.Json;
using Raylib_cs;

namespace UI;

public class Liderboard : IUI
{
    private const string OnlineBaseUrl = "http://someerr.online:8080";
    public static string SelectedMode = "Local";
    public static string SelectedFilter = "All";

    private static readonly List<LeaderboardEntry> OfflineEntries = new();
    private static List<LeaderboardEntry> OnlineEntries = new();
    private static bool offlineLoaded;
    private static bool onlineLoaded;
    private static string onlineLoadedFilter = string.Empty;
    private static string? onlineStatusMessage;
    private static readonly HttpClient HttpClient = new HttpClient();

    static Liderboard()
    {
        HttpClient.Timeout = TimeSpan.FromSeconds(3);
    }

    public void Draw()
    {
        EnsureOfflineLoaded();

        int btnWidth = 180;
        int btnHeight = 54;

        Rectangle btnBack = new Rectangle(50, 30, btnWidth, btnHeight);
        Rectangle btnMode = new Rectangle(270, 30, btnWidth, btnHeight);
        Rectangle btnFilter = new Rectangle(490, 30, btnWidth, btnHeight);

        Vector2 mousePos = Raylib.GetMousePosition();
        bool isLeftMouseClicked = Raylib.IsMouseButtonPressed(MouseButton.Left);

        if (Lib.IsClicked(btnBack, mousePos, isLeftMouseClicked))
        {
            InitUI.OpenMainMenu();
        }

        if (Lib.IsClicked(btnMode, mousePos, isLeftMouseClicked))
        {
            SelectedMode = SelectedMode == "Local" ? "Online" : "Local";
            if (SelectedMode == "Online")
                EnsureOnlineLoaded();
        }

        if (Lib.IsClicked(btnFilter, mousePos, isLeftMouseClicked))
        {
            SelectedFilter = NextFilter(SelectedFilter);
            if (SelectedMode == "Online")
                EnsureOnlineLoaded();
        }

        Raylib.DrawText("Liderboard", 330, 110, 42, Color.Black);
        Lib.DrawButton(btnBack, "Back", mousePos);
        Lib.DrawButton(btnMode, SelectedMode, mousePos);
        Lib.DrawButton(btnFilter, SelectedFilter, mousePos);

        if (SelectedMode == "Local")
        {
            DrawEntries(GetFilteredEntries(OfflineEntries));
        }
        else
        {
            DrawEntries(GetFilteredEntries(OnlineEntries));

            if (!string.IsNullOrWhiteSpace(onlineStatusMessage))
                Raylib.DrawText(onlineStatusMessage, 180, 190, 22, Color.Maroon);
        }
    }

    public static void AddResult(int width, int height, double minePercentage, int elapsedSeconds)
    {
        EnsureOfflineLoaded();

        var entry = new LeaderboardEntry
        {
            Score = CalculateScore(width, height, minePercentage, elapsedSeconds),
            Width = width,
            Height = height,
            MinePercentage = minePercentage,
            Seconds = elapsedSeconds,
            Mode = SelectedMode,
            AchievedAt = DateTime.UtcNow,
            IsUploaded = false
        };

        OfflineEntries.Add(entry);
        SortAndTrim(OfflineEntries);

        if (TrySubmitOnlineEntry(entry))
            entry.IsUploaded = true;

        SaveOfflineEntries();
    }

    public static void SyncPendingOfflineEntries()
    {
        EnsureOfflineLoaded();
        if (!IsOnlineServerAvailable())
            return;

        bool hasChanges = false;
        foreach (LeaderboardEntry entry in OfflineEntries)
        {
            if (entry.IsUploaded)
                continue;

            if (!TrySubmitOnlineEntry(entry))
                continue;

            entry.IsUploaded = true;
            hasChanges = true;
        }

        if (hasChanges)
            SaveOfflineEntries();
    }

    public static int GetBestLocalScore()
    {
        EnsureOfflineLoaded();
        return OfflineEntries.Count == 0 ? 0 : OfflineEntries[0].Score;
    }

    private static void DrawEntries(List<LeaderboardEntry> entries)
    {
        int startY = 210;
        int rowHeight = 48;

        if (entries.Count == 0)
        {
            Raylib.DrawText("No results yet", 330, startY + 20, 24, Color.DarkGray);
            return;
        }

        for (int i = 0; i < Math.Min(10, entries.Count); i++)
        {
            LeaderboardEntry entry = entries[i];
            string line = $"{i + 1}. {entry.Score} pts   {entry.Width}x{entry.Height}   {FormatDifficulty(entry.MinePercentage)}   {Lib.FormatTime(entry.Seconds)}";
            Raylib.DrawText(line, 110, startY + i * rowHeight, 22, Color.Black);
        }
    }

    private static List<LeaderboardEntry> GetFilteredEntries(IEnumerable<LeaderboardEntry> source)
    {
        var filtered = new List<LeaderboardEntry>();
        foreach (LeaderboardEntry entry in source)
        {
            if (SelectedFilter == "All" || SelectedFilter == $"{entry.Width}x{entry.Height}")
                filtered.Add(entry);
        }

        filtered.Sort((left, right) => right.Score.CompareTo(left.Score));
        return filtered;
    }

    private static string NextFilter(string current)
    {
        string[] filters = { "All", "5x5", "5x10", "10x10" };
        int index = Array.IndexOf(filters, current);
        if (index < 0)
            index = 0;

        return filters[(index + 1) % filters.Length];
    }

    private static string FormatDifficulty(double minePercentage)
    {
        if (minePercentage <= 0.20)
            return "Normal";

        if (minePercentage <= 0.30)
            return "Expert";

        return "Nightmare";
    }

    private static int CalculateScore(int width, int height, double minePercentage, int elapsedSeconds)
    {
        double timeFactor = Math.Max(1, elapsedSeconds);
        double difficultyFactor = 1.0 + minePercentage;
        double sizeFactor = width * height;
        return (int)Math.Round((sizeFactor * difficultyFactor * 1000.0) / timeFactor);
    }

    private static void EnsureOfflineLoaded()
    {
        if (offlineLoaded)
            return;

        offlineLoaded = true;
        string path = GetOfflinePath();
        if (!File.Exists(path))
            return;

        try
        {
            string json = File.ReadAllText(path);
            List<LeaderboardEntry>? entries = JsonConvert.DeserializeObject<List<LeaderboardEntry>>(json);
            if (entries is not null)
            {
                OfflineEntries.Clear();
                OfflineEntries.AddRange(entries);
                SortAndTrim(OfflineEntries);
            }
        }
        catch (Exception exception)
        {
            LogError("Failed to load offline leaderboard", exception);
            OfflineEntries.Clear();
        }
    }

    private static void EnsureOnlineLoaded()
    {
        if (onlineLoaded && string.Equals(onlineLoadedFilter, SelectedFilter, StringComparison.OrdinalIgnoreCase))
            return;

        onlineLoaded = true;
        onlineLoadedFilter = SelectedFilter;

        if (!IsOnlineServerAvailable())
        {
            OnlineEntries = new List<LeaderboardEntry>();
            onlineStatusMessage = "Online leaderboard server is unavailable";
            return;
        }

        try
        {
            string requestUrl = BuildLeaderboardUrl(SelectedFilter);
            string json = HttpClient.GetStringAsync(requestUrl).GetAwaiter().GetResult();
            List<LeaderboardEntry>? entries = JsonConvert.DeserializeObject<List<LeaderboardEntry>>(json);
            OnlineEntries = entries ?? new List<LeaderboardEntry>();
            SortAndTrim(OnlineEntries);
            onlineStatusMessage = null;
        }
        catch (Exception exception)
        {
            LogError("Failed to load online leaderboard", exception);
            OnlineEntries = new List<LeaderboardEntry>();
            onlineStatusMessage = "Failed to load online leaderboard";
        }
    }

    private static bool TrySubmitOnlineEntry(LeaderboardEntry entry)
    {
        if (!IsOnlineServerAvailable())
            return false;

        try
        {
            string payload = JsonConvert.SerializeObject(entry, Formatting.None);
            using var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = HttpClient.PostAsync($"{OnlineBaseUrl}/leaderboard", content).GetAwaiter().GetResult();
            return response.IsSuccessStatusCode;
        }
        catch (Exception exception)
        {
            LogError("Failed to submit online leaderboard entry", exception);
            return false;
        }
    }

    private static bool IsOnlineServerAvailable()
    {
        try
        {
            HttpResponseMessage response = HttpClient.GetAsync($"{OnlineBaseUrl}/status").GetAwaiter().GetResult();
            return response.IsSuccessStatusCode;
        }
        catch (Exception exception)
        {
            LogError("Online leaderboard server is unavailable", exception);
            return false;
        }
    }

    private static string BuildLeaderboardUrl(string filter)
    {
        string queryFilter = filter == "All" ? string.Empty : $"&filter={Uri.EscapeDataString(filter)}";
        return $"{OnlineBaseUrl}/leaderboard?limit=10{queryFilter}";
    }

    private static void SaveOfflineEntries()
    {
        string path = GetOfflinePath();
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? InitUI.SaveFolder);
        string json = JsonConvert.SerializeObject(OfflineEntries, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    private static string GetOfflinePath()
    {
        return Path.Combine(InitUI.SaveFolder, "leaderboard.json");
    }

    private static void SortAndTrim(List<LeaderboardEntry> entries)
    {
        entries.Sort((left, right) => right.Score.CompareTo(left.Score));
        if (entries.Count > 10)
            entries.RemoveRange(10, entries.Count - 10);
    }

    private static void LogError(string message, Exception exception)
    {
        Console.Error.WriteLine($"[Liderboard] {message}: {exception.Message}");
    }
}

public class LeaderboardEntry
{
    public int Score { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double MinePercentage { get; set; }
    public int Seconds { get; set; }
    public string Mode { get; set; } = string.Empty;
    public DateTime AchievedAt { get; set; }
    public bool IsUploaded { get; set; }
}
