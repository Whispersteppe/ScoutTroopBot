using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutTroopBot.Configuration;

public class TemplateConfiguration
{
    public string ItemType { get; set; }
    public List<CategoryTemplateConfiguration> Categories { get; set; }
    public List<RoleTemplateConfiguration> Roles { get; set; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; } = new Dictionary<string, JsonElement>();

}
