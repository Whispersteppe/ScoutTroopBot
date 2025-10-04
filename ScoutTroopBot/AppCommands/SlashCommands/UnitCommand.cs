using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;
using ScoutTroopBot.Configuration;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace ScoutTroopBot.AppCommands.SlashCommands;

[SlashCommand("unit", "Manages unit setup", DefaultGuildPermissions = Permissions.Administrator)]
public class UnitSetupCommands(
    ILogger<SetupCommand> logger, 
    RestClient restClient,
    IOptions<RootConfiguration> rootConfig,
    RoleChannelBuilder roleChannelBuilder) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SubSlashCommand("create", "set up the unit")]
    public async Task<string> SetupUnitAsync([SlashCommandParameter(Description = "The name of the new unit")] string unitName)
    {

        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
            { "name", unitName },
            { "nameLower", unitName.ToLower().Replace(' ', '-') },
        };
        roleChannelBuilder.CreateFromTemplate(Context, rootConfig.Value.UnitTemplate, substitutions);

        return "Continuing Setup in background";
    }

    [SubSlashCommand("list", "list existing units")]
    public async Task<string> ListUnitsAsync()
    {
        var unitRoles = Context.Guild.Roles.Values.Where(r => r.Name.EndsWith(" Unit")).Select(r => r.Name).ToList();
        if (unitRoles.Count == 0)
            return "No units found.";
        return "Existing units:\n" + string.Join("\n", unitRoles);
    }
}


