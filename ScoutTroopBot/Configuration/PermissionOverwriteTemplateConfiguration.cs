using NetCord;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutTroopBot.Configuration;

public class PermissionOverwriteTemplateConfiguration
{
    public string RoleName { get; set; }
    public PermissionOverwriteType Type { get; set; }
    public List<Permissions> Allowed { get; set; }
    public List<Permissions> Denied { get; set; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; }

}
