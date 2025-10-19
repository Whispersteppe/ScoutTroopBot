using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using ScoutTroopBot.Configuration;
using System.Text;

namespace ScoutTroopBot.AppCommands.SlashCommands;

/// <summary>
/// set up the server with common roles and channels
/// </summary>
/// <param name="logger"></param>
/// <param name="restClient"></param>
public class SbotHelpCommands(ILogger<SbotHelpCommands> logger,
    RestClient restClient,
    IOptions<RootConfiguration> rootConfig,
    NetcordComponentBuilder roleChannelBuilder) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("sbot-help", "list available sbot commands", DefaultGuildPermissions = Permissions.Administrator)]
    public async Task<string> HelpAsync()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Available sbot commands:");
        sb.AppendLine("/sbot common_create - set up common roles and channels");
        sb.AppendLine("/sbot unit_create <unitName> - create a new unit");
        sb.AppendLine("/sbot unit_list - list existing units");
        sb.AppendLine("/sbot oa_create - set up Order of the Arrow channels");
        sb.AppendLine("/sbot mb_create <badgeName> - create a new merit badge");
        sb.AppendLine("/sbot mb_list - list existing merit badges");
        sb.AppendLine("/sbot patrol_create <patrolName> - create a new patrol");
        sb.AppendLine("/sbot patrol_list - list existing patrols");
        sb.AppendLine("/sbot patrol_rename <patrolName> <newPatrolName> - rename a patrol");
        sb.AppendLine("/sbot patrol_delete <patrolName> - delete a patrol (currently disabled)");

        return sb.ToString();
    }
}


