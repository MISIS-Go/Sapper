using System;

namespace Data;

public abstract class GameSnapshotSerializer
{
    public abstract bool CanHandle(string path);
    public abstract GameSnapshot? Load(string path);
    public abstract void Save(GameSnapshot snapshot, string path);

    public static GameSnapshotSerializer Resolve(string path)
    {
        if (path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            return new XmlGameSnapshotSerializer();

        return new JsonGameSnapshotSerializer();
    }
}
