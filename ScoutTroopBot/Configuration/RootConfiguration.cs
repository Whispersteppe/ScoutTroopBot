using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutTroopBot.Configuration;

/*
 *    
 *    list of positions
 *    list of ranks
 *    list of merit badges
 */
public class RootConfiguration
{
    public NetcordConfiguration Discord { get; set; }
    public TemplateConfiguration PatrolTemplate { get; set; }
    public TemplateConfiguration UnitTemplate { get; set; }
    public TemplateConfiguration MeritBadgeTemplate { get; set; }
    public TemplateConfiguration CommonTemplate { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtraProperties { get; set; }
}
