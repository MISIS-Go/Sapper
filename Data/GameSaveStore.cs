using System;
using System.Collections.Generic;
using System.IO;

namespace Data;

public static class GameSaveStore
{
    private static readonly IReadOnlyList<GameSnapshotSerializer> Serializers = new GameSnapshotSerializer[]
    {
        new JsonGameSnapshotSerializer(),
        new XmlGameSnapshotSerializer()
    };

    public static GameSnapshot? Load(string path)
    {
        try
        {
            return Resolve(path).Load(path);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"[GameSaveStore] Failed to load save '{path}': {exception.Message}");
            return null;
        }
    }

    public static void Save(GameSnapshot snapshot, string path)
    {
        Resolve(path).Save(snapshot, path);
    }

    public static void CopyFilesWithFormatChange(string folder, string fromFormat, string toFormat)
    {
        if (string.IsNullOrWhiteSpace(folder) ||
            string.Equals(fromFormat, toFormat, StringComparison.OrdinalIgnoreCase) ||
            !Directory.Exists(folder))
        {
            return;
        }

        string fromExtension = GetExtension(fromFormat);
        string toExtension = GetExtension(toFormat);

        foreach (string path in Directory.EnumerateFiles(folder, $"*{fromExtension}", SearchOption.TopDirectoryOnly))
        {
            GameSnapshot? snapshot = Load(path);
            if (snapshot is null)
                continue;

            string targetPath = Path.ChangeExtension(path, toExtension);
            Save(snapshot, targetPath);
        }
    }

    private static GameSnapshotSerializer Resolve(string path)
    {
        foreach (GameSnapshotSerializer serializer in Serializers)
        {
            if (serializer.CanHandle(path))
                return serializer;
        }

        return Serializers[0];
    }

    private static string GetExtension(string format)
    {
        return format.Equals("Xml", StringComparison.OrdinalIgnoreCase) ? ".xml" : ".json";
    }
}
