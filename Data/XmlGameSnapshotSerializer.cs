using System.IO;
using System.Xml.Serialization;

namespace Data;

public sealed class XmlGameSnapshotSerializer : GameSnapshotSerializer
{
    public override bool CanHandle(string path)
    {
        return Path.GetExtension(path).Equals(".xml", System.StringComparison.OrdinalIgnoreCase);
    }

    public override GameSnapshot? Load(string path)
    {
        var serializer = new XmlSerializer(typeof(GameSnapshot));
        using var stream = File.OpenRead(path);
        return serializer.Deserialize(stream) as GameSnapshot;
    }

    public override void Save(GameSnapshot snapshot, string path)
    {
        var serializer = new XmlSerializer(typeof(GameSnapshot));
        using var stream = File.Create(path);
        serializer.Serialize(stream, snapshot);
    }
}
