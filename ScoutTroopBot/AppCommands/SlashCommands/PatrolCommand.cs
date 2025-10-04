using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using ScoutTroopBot.Configuration;

namespace ScoutTroopBot.AppCommands.SlashCommands;

/// <summary>
/// Create, remove, and rename patrols
/// </summary>
/// <param name="logger"></param>
/// <param name="restClient"></param>
/// <param name="rootConfig"></param>
/// <param name="roleChannelBuilder"></param>
[SlashCommand("patrol", "Manages patrol setup", DefaultGuildPermissions = Permissions.Administrator)]
public class PatrolSetupCommands(
    ILogger<SetupCommand> logger, 
    RestClient restClient, 
    IOptions<RootConfiguration> rootConfig, 
    RoleChannelBuilder roleChannelBuilder) : ApplicationCommandModule<ApplicationCommandContext>
{
    /// <summary>
    /// create a patrol with roles and channels from the patrol template
    /// </summary>
    /// <param name="patrolName"></param>
    /// <returns></returns>
    [SubSlashCommand("create", "set up the patrol")]
    public async Task<string> SetupPatrolAsync([SlashCommandParameter(Description = "The name of the new patrol")] string patrolName)
    {

        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
            { "name", patrolName },
            { "nameLower", patrolName.ToLower().Replace(' ', '-') },
        };
        roleChannelBuilder.CreateFromTemplate(Context, rootConfig.Value.PatrolTemplate, substitutions);

        return "Continuing Setup in background";

    }

    /// <summary>
    /// list the set of existing patrols
    /// </summary>
    /// <returns></returns>
    [SubSlashCommand("list", "list existing patrols")]
    public async Task<string> ListPatrolsAsync()
    {
        var patrolRoles = Context.Guild.Roles.Values.Where(r => r.Name.EndsWith(" Patrol")).Select(r => r.Name).ToList();
        if (patrolRoles.Count == 0)
            return "No patrols found.";
        return "Existing patrols:\n" + string.Join("\n", patrolRoles);
    }

    /// <summary>
    /// delete a patrol by removing roles and channels from the patrol template
    /// </summary>
    /// <param name="patrolName"></param>
    /// <returns></returns>
    /// <remarks>
    /// currently we're not deleting patrols to avoid accidental data loss
    /// </remarks>
    [SubSlashCommand("delete", "delete a patrol")]
    public async Task<string> DeletePatrolAsync([SlashCommandParameter(Description = "The name of the patrol to delete")] string patrolName)
    {
        return "We're currently not deleting patrols. Please contact an administrator.";

        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
            { "name", patrolName },
            { "nameLower", patrolName.ToLower().Replace(' ', '-') },
        };
        roleChannelBuilder.DeleteFromTemplate(Context, rootConfig.Value.PatrolTemplate, substitutions, restClient);

        return "Continuing Removal in background";
    }

    /// <summary>
    /// rename a patrol by renaming roles and channels from the patrol template
    /// </summary>
    /// <param name="patrolName"></param>
    /// <param name="newPatrolName"></param>
    /// <returns></returns>
    [SubSlashCommand("rename", "rename a patrol")]
    public async Task<string> RenamePatrolAsync(
        [SlashCommandParameter(Description = "The name of the patrol to rename")] string patrolName,
        [SlashCommandParameter(Description = "The new patrol name")] string newPatrolName
        )
    {
        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
            { "name", patrolName },
            { "nameLower", patrolName.ToLower().Replace(' ', '-') },
            { "toName", newPatrolName },
            { "toNameLower", newPatrolName.ToLower().Replace(' ', '-') },
        };

        roleChannelBuilder.RenameFromTemplate(Context, rootConfig.Value.PatrolTemplate, substitutions);

        return $"Renaming patrol {patrolName} to {newPatrolName}";
    }

    //todo /Patrol Roster <patrol name> - lists all members of the patrol
}


