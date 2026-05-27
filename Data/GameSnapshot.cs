using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class GameSnapshot
{
    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }

    [JsonProperty("minePercentage")]
    public double MinePercentage { get; set; }

    [JsonProperty("elapsedSeconds")]
    public int ElapsedSeconds { get; set; }

    [JsonProperty("cells")]
    public List<CellSnapshot> Cells { get; set; } = new List<CellSnapshot>();
}
