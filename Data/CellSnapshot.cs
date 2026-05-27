using Newtonsoft.Json;

namespace Data;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class CellSnapshot
{
    [JsonProperty("isMine")]
    public bool IsMine { get; set; }

    [JsonProperty("isOpened")]
    public bool IsOpened { get; set; }

    [JsonProperty("hasFlag")]
    public bool HasFlag { get; set; }

    [JsonProperty("nearbyMinesCount")]
    public int NearbyMinesCount { get; set; }
}
