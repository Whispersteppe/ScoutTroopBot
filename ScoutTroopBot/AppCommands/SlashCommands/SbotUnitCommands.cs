using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using ScoutTroopBot.Configuration;

namespace ScoutTroopBot.AppCommands.SlashCommands;

/// <summary>
/// set up the server with common roles and channels
/// </summary>
/// <param name="logger"></param>
/// <param name="restClient"></param>
[SlashCommand("sbot-unit", "sbot commands", DefaultGuildPermissions = Permissions.Administrator)]
public class SbotUnitCommands(ILogger<SbotUnitCommands> logger,
    RestClient restClient,
    IOptions<RootConfiguration> rootConfig,
    NetcordComponentBuilder roleChannelBuilder) : ApplicationCommandModule<ApplicationCommandContext>
{
    /// <summary>
    /// create the roles and channels for a new unit
    /// </summary>
    /// <param name="unitName"></param>
    /// <returns></returns>
    [SubSlashCommand("create", "set up the unit")]
    public async Task<string> SetupUnitAsync([SlashCommandParameter(Description = "The name of the new unit")] string unitName)
    {

        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
            { "name", unitName },
            { "nameLower", unitName.ToLower().Replace(' ', '-') },
        };
        roleChannelBuilder.CreateAllFromTemplate(Context, rootConfig.Value.UnitTemplate, substitutions);

        return "Continuing Setup in background";
    }

    /// <summary>
    /// list all units by looking for roles that end with " Unit"
    /// </summary>
    /// <returns></returns>
    [SubSlashCommand("list", "list existing units")]
    public async Task<string> ListUnitsAsync()
    {
        var unitRoles = Context.Guild.Roles.Values.Where(r => r.Name.EndsWith(" Unit")).Select(r => r.Name).ToList();
        if (unitRoles.Count == 0)
            return "No units found.";
        return "Existing units:\n" + string.Join("\n", unitRoles);
    }
}


