using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutBot.Configuration;
public class NetcordConfiguration
{
    public string Token { get; set; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; }

}
