using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutBot.Configuration;

/// <summary>
/// the root configuration for the bot, including Discord settings and templates for patrols, units, merit badges, and common settings.
/// </summary>
public class RootConfiguration
{
    public NetcordConfiguration Discord { get; set; }
    public Dictionary<string, TemplateConfiguration> Templates { get; set; }
    //public TemplateConfiguration PatrolTemplate { get; set; }
    //public TemplateConfiguration UnitTemplate { get; set; }
    //public TemplateConfiguration MeritBadgeTemplate { get; set; }
    //public TemplateConfiguration CommonTemplate { get; set; }
    //public TemplateConfiguration OATemplate { get; set; }

    public List<PositionItem> Positions { get; set; }
    public List<RankItem> Ranks { get; set; }
    public List<MeritBadgeItem> MeritBadges { get; set; }


    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; }


}

public class PositionItem
{   public string Name { get; set; }
    public string Description { get; set; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; }
}

public class RankItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; }
}

public class MeritBadgeItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; }
}
