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
[SlashCommand("sbot-mb", "sbot commands", DefaultGuildPermissions = Permissions.Administrator)]
public class SbotMeritBadgeCommands(ILogger<SbotMeritBadgeCommands> logger,
    RestClient restClient,
    IOptions<RootConfiguration> rootConfig,
    NetcordComponentBuilder roleChannelBuilder) : ApplicationCommandModule<ApplicationCommandContext>
{
    /// <summary>
    /// Sets up a merit badge by creating the necessary roles and channels based on a predefined template.
    /// </summary>
    /// <remarks>This method uses a predefined template to create roles and channels associated with the
    /// specified merit badge. The <paramref name="badgeName"/> is used to customize the template, including generating
    /// a lowercase, hyphenated version of the name for certain identifiers.</remarks>
    /// <param name="badgeName">The name of the merit badge. This value is used to generate role and channel names.</param>
    /// <returns>A string indicating that the setup process is continuing in the background.</returns>
    [SubSlashCommand("create", "set up the merit badge")]
    public async Task<string> SetupMeritBadgeAsync([SlashCommandParameter(Description = "The name of the merit badge")] string badgeName)
    {

        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
            { "name", badgeName },
            { "nameLower", badgeName.ToLower().Replace(' ', '-') },
        };
        roleChannelBuilder.CreateAllFromTemplate(Context, rootConfig.Value.MeritBadgeTemplate, substitutions);

        return "Continuing Setup in background";
    }

    /// <summary>
    /// Lists the existing merit badge units in the current guild.
    /// </summary>
    /// <remarks>This method retrieves all roles in the current guild whose names end with " Merit Badge" and
    /// returns a formatted string containing their names. If no such roles are found, the method returns a message
    /// indicating that no units exist.</remarks>
    /// <returns>A string containing the names of all existing merit badge units, each on a new line, or a message indicating
    /// that no units were found.</returns>
    [SubSlashCommand("list", "list existing merit badges")]
    public async Task<string> ListMeritBadgesAsync()
    {
        var unitRoles = Context.Guild.Roles.Values.Where(r => r.Name.EndsWith(" Merit Badge")).Select(r => r.Name).ToList();
        if (unitRoles.Count == 0)
            return "No units found.";
        return "Existing units:\n" + string.Join("\n", unitRoles);
    }
}


