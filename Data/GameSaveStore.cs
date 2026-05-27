using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Data;

public static class GameSaveStore
{
    public static GameSnapshot? Load(string path)
    {
        try
        {
            if (Path.GetExtension(path).Equals(".xml", StringComparison.OrdinalIgnoreCase))
            {
                var serializer = new XmlSerializer(typeof(GameSnapshot));
                using var stream = File.OpenRead(path);
                return serializer.Deserialize(stream) as GameSnapshot;
            }

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<GameSnapshot>(json);
        }
        catch
        {
            return null;
        }
    }

    public static void Save(GameSnapshot snapshot, string path)
    {
        string extension = Path.GetExtension(path);
        if (extension.Equals(".xml", StringComparison.OrdinalIgnoreCase))
        {
            var serializer = new XmlSerializer(typeof(GameSnapshot));
            using var stream = File.Create(path);
            serializer.Serialize(stream, snapshot);
            return;
        }

        string json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}
