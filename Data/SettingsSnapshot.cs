using Newtonsoft.Json;

namespace Data;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class SettingsSnapshot
{
    [JsonProperty("saveFolder")]
    public string SaveFolder { get; set; } = string.Empty;

    [JsonProperty("selectedSaveFormat")]
    public string SelectedSaveFormat { get; set; } = "Json";
}
