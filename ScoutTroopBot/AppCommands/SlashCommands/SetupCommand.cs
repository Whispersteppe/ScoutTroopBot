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
[SlashCommand("setup", "Manages setup", DefaultGuildPermissions = Permissions.Administrator)]
public class SetupCommand(ILogger<SetupCommand> logger, 
    RestClient restClient,
    IOptions<RootConfiguration> rootConfig,
    RoleChannelBuilder roleChannelBuilder) : ApplicationCommandModule<ApplicationCommandContext>
{

    /// <summary>
    /// command to set up common roles and channels
    /// </summary>
    /// <returns></returns>
    [SubSlashCommand("common", "set up the server")]
    public async Task<string> SetupServerAsync()
    {
        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
        };
        roleChannelBuilder.CreateFromTemplate(Context, rootConfig.Value.CommonTemplate, substitutions);

        return "Continuing Setup in background";
    }
}


