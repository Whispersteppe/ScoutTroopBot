using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace ScoutBot.Configuration;

public class  RoleTemplateConfiguration
{
    public string Name { get; set; }
    public string? Color { get; set; }
    //public bool? Hoist { get; set; }
    //public ImageProperties? Icon { get; set; }
    //public string? UnicodeIcon { get; set; }
    public bool? Mentionable { get; set; }

    public List<GuildPermission>? Permissions { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JObject> ExtraProperties { get; set; }

}
