using NetCord;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutTroopBot.Configuration;

public class  RoleTemplateConfiguration
{
    public string Name { get; set; }
    public Color? Color { get; set; }
    //public bool? Hoist { get; set; }
    //public ImageProperties? Icon { get; set; }
    //public string? UnicodeIcon { get; set; }
    public bool? Mentionable { get; set; }

    public List<Permissions>? Permissions { get; set; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; }

}
