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

[SlashCommand("patrol", "Manages patrol setup", DefaultGuildPermissions = Permissions.Administrator)]
public class PatrolSetupCommands(
    ILogger<SetupCommand> logger, 
    RestClient restClient, 
    IOptions<RootConfiguration> rootConfig, 
    RoleChannelBuilder roleChannelBuilder) : ApplicationCommandModule<ApplicationCommandContext>
{
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

    [SubSlashCommand("list", "list existing patrols")]
    public async Task<string> ListPatrolsAsync()
    {
        var patrolRoles = Context.Guild.Roles.Values.Where(r => r.Name.EndsWith(" Patrol")).Select(r => r.Name).ToList();
        if (patrolRoles.Count == 0)
            return "No patrols found.";
        return "Existing patrols:\n" + string.Join("\n", patrolRoles);
    }

    [SubSlashCommand("delete", "delete a patrol")]
    public async Task<string> DeletePatrolAsync([SlashCommandParameter(Description = "The name of the patrol to delete")] string patrolName)
    {
        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
            { "name", patrolName },
            { "nameLower", patrolName.ToLower().Replace(' ', '-') },
        };
        roleChannelBuilder.DeleteFromTemplate(Context, rootConfig.Value.PatrolTemplate, substitutions, restClient);

        return "Continuing Removal in background";
    }

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


