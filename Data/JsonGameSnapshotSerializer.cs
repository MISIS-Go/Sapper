using System.IO;
using Newtonsoft.Json;

namespace Data;

public sealed class JsonGameSnapshotSerializer : GameSnapshotSerializer
{
    public override bool CanHandle(string path)
    {
        return Path.GetExtension(path).Equals(".json", System.StringComparison.OrdinalIgnoreCase);
    }

    public override GameSnapshot? Load(string path)
    {
        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<GameSnapshot>(json);
    }

    public override void Save(GameSnapshot snapshot, string path)
    {
        string json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}
