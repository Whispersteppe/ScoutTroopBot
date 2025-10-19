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
[SlashCommand("sbot-utility", "sbot commands", DefaultGuildPermissions = Permissions.Administrator)]
public class SbotUtilityCommands(ILogger<SbotHelpCommands> logger,
    RestClient restClient,
    IOptions<RootConfiguration> rootConfig,
    NetcordComponentBuilder roleChannelBuilder) : ApplicationCommandModule<ApplicationCommandContext>
{

    [SubSlashCommand("trashbot", "Trash the entire server - testing only")]
    public async Task<string> ShazbotAsync(
        [SlashCommandParameter(Description = "type YES if you really want to do this")] string reallyTrashThis
        )
    {
        if (reallyTrashThis != "YES")
        {
            return "You must type YES to confirm.";
        }

        foreach (var channel in Context.Guild.Channels.Values)
        {
            try
            {
                if (channel.Id != Context.Guild.SystemChannelId)
                {
                    await restClient.DeleteChannelAsync(channel.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting channel {ChannelId} ({ChannelName})", channel.Id, channel.Name);
            }
        }

        //  this is tossing errors on all of the roles.  not sure why.
        foreach (var role in Context.Guild.Roles.Values)
        {
            try
            {
                if (role.Name != "Scout Troop Bot")
                {
                    await role.DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting role {RoleId} ({RoleName})", role.Id, role.Name);
            }
        }
        return "Shazbot complete.";
    }
}


