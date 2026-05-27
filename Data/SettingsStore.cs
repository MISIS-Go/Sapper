using System;
using System.IO;
using Newtonsoft.Json;

namespace Data;

public static class SettingsStore
{
    private static string SettingsPath => Path.Combine(AppContext.BaseDirectory, "Settings.json");

    public static SettingsSnapshot Load()
    {
        try
        {
            if (!File.Exists(SettingsPath))
            {
                SettingsSnapshot defaults = CreateDefault();
                Save(defaults);
                return defaults;
            }

            string json = File.ReadAllText(SettingsPath);
            return JsonConvert.DeserializeObject<SettingsSnapshot>(json) ?? CreateDefault();
        }
        catch
        {
            return CreateDefault();
        }
    }

    public static void Save(SettingsSnapshot settings)
    {
        string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText(SettingsPath, json);
    }

    private static SettingsSnapshot CreateDefault()
    {
        return new SettingsSnapshot
        {
            SaveFolder = Path.Combine(AppContext.BaseDirectory, "Saves"),
            SelectedSaveFormat = "Json",
            OnlineFetchUrl = string.Empty,
            OnlineSubmitUrl = string.Empty
        };
    }
}
