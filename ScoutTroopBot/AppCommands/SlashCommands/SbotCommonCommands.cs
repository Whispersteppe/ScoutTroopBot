using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using ScoutTroopBot.Configuration;
using System.ComponentModel;

namespace ScoutTroopBot.AppCommands.SlashCommands;

/// <summary>
/// set up the server with common roles and channels
/// </summary>
/// <param name="logger"></param>
/// <param name="restClient"></param>
[SlashCommand("sbot-common", "sbot commands", DefaultGuildPermissions = Permissions.Administrator)]
public class SbotCommonCommands(ILogger<SbotCommonCommands> logger, 
    RestClient restClient,
    IOptions<RootConfiguration> rootConfig,
    NetcordComponentBuilder roleChannelBuilder) : ApplicationCommandModule<ApplicationCommandContext>
{
    /// <summary>
    /// command to set up common roles and channels
    /// </summary>
    /// <returns></returns>
    [SubSlashCommand("create", "set up the server")]
    public async Task<string> SetupServerAsync()
    {
        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
        };
        roleChannelBuilder.CreateAllFromTemplate(Context, rootConfig.Value.CommonTemplate, substitutions);

        return "Continuing Setup in background";
    }
}


