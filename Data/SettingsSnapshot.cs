using Newtonsoft.Json;

namespace Data;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class SettingsSnapshot
{
    [JsonProperty("saveFolder")]
    public string SaveFolder { get; set; } = string.Empty;

    [JsonProperty("selectedSaveFormat")]
    public string SelectedSaveFormat { get; set; } = "Json";

    [JsonProperty("onlineFetchUrl")]
    public string OnlineFetchUrl { get; set; } = string.Empty;

    [JsonProperty("onlineSubmitUrl")]
    public string OnlineSubmitUrl { get; set; } = string.Empty;
}
