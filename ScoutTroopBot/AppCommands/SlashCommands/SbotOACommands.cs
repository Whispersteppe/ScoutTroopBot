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
[SlashCommand("sbot-oa", "sbot commands", DefaultGuildPermissions = Permissions.Administrator)]
public class SbotOACommands(ILogger<SbotOACommands> logger,
    RestClient restClient,
    IOptions<RootConfiguration> rootConfig,
    NetcordComponentBuilder roleChannelBuilder) : ApplicationCommandModule<ApplicationCommandContext>
{
    /// <summary>
    /// create the roles and channels for a new unit
    /// </summary>
    /// <param name="unitName"></param>
    /// <returns></returns>
    [SubSlashCommand("create", "set up Order of the Arrow channels")]
    public async Task<string> SetupOAAsync()
    {

        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
        };
        roleChannelBuilder.CreateAllFromTemplate(Context, rootConfig.Value.OATemplate, substitutions);

        return "Continuing Setup in background";
    }
}


