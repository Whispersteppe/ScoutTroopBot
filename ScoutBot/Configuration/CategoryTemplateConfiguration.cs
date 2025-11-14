using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutBot.Configuration;

public class CategoryTemplateConfiguration
{
    public string Name { get; set; }
    public List<ChannelTemplateConfiguration> Channels { get; set; }
    public List<PermissionOverwriteTemplateConfiguration> PermissionOverwrites { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; } = new Dictionary<string, JsonElement>();

}
