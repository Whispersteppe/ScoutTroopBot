using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using ScoutTroopBot.Configuration;
using System.ComponentModel;
using System.Text;

namespace ScoutTroopBot.AppCommands.SlashCommands;

/// <summary>
/// set up the server with common roles and channels
/// </summary>
/// <param name="logger"></param>
/// <param name="restClient"></param>
[SlashCommand("sbot", "sbot commands", DefaultGuildPermissions = Permissions.Administrator)]
public class SbotCommands(ILogger<SbotCommands> logger, 
    RestClient restClient,
    IOptions<RootConfiguration> rootConfig,
    NetcordComponentBuilder roleChannelBuilder) : ApplicationCommandModule<ApplicationCommandContext>
{

    #region common

    /// <summary>
    /// command to set up common roles and channels
    /// </summary>
    /// <returns></returns>
    [SubSlashCommand("common_create", "set up the server")]
    public async Task<string> SetupServerAsync()
    {
        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
        };
        roleChannelBuilder.CreateAllFromTemplate(Context, rootConfig.Value.CommonTemplate, substitutions);

        return "Continuing Setup in background";
    }

    #endregion

    #region unit

    /// <summary>
    /// create the roles and channels for a new unit
    /// </summary>
    /// <param name="unitName"></param>
    /// <returns></returns>
    [SubSlashCommand("unit_create", "set up the unit")]
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
    [SubSlashCommand("unit_list", "list existing units")]
    public async Task<string> ListUnitsAsync()
    {
        var unitRoles = Context.Guild.Roles.Values.Where(r => r.Name.EndsWith(" Unit")).Select(r => r.Name).ToList();
        if (unitRoles.Count == 0)
            return "No units found.";
        return "Existing units:\n" + string.Join("\n", unitRoles);
    }

    #endregion

    #region Order of the Arrow

    /// <summary>
    /// create the roles and channels for a new unit
    /// </summary>
    /// <param name="unitName"></param>
    /// <returns></returns>
    [SubSlashCommand("oa_create", "set up Order of the Arrow channels")]
    public async Task<string> SetupOAAsync()
    {

        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
        };
        roleChannelBuilder.CreateAllFromTemplate(Context, rootConfig.Value.OATemplate, substitutions);

        return "Continuing Setup in background";
    }

    #endregion


    #region merit badge

    /// <summary>
    /// Sets up a merit badge by creating the necessary roles and channels based on a predefined template.
    /// </summary>
    /// <remarks>This method uses a predefined template to create roles and channels associated with the
    /// specified merit badge. The <paramref name="badgeName"/> is used to customize the template, including generating
    /// a lowercase, hyphenated version of the name for certain identifiers.</remarks>
    /// <param name="badgeName">The name of the merit badge. This value is used to generate role and channel names.</param>
    /// <returns>A string indicating that the setup process is continuing in the background.</returns>
    [SubSlashCommand("mb_create", "set up the merit badge")]
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
    [SubSlashCommand("mb_list", "list existing merit badges")]
    public async Task<string> ListMeritBadgesAsync()
    {
        var unitRoles = Context.Guild.Roles.Values.Where(r => r.Name.EndsWith(" Merit Badge")).Select(r => r.Name).ToList();
        if (unitRoles.Count == 0)
            return "No units found.";
        return "Existing units:\n" + string.Join("\n", unitRoles);
    }

    #endregion

    #region patrol

    /// <summary>
    /// create a patrol with roles and channels from the patrol template
    /// </summary>
    /// <param name="patrolName"></param>
    /// <returns></returns>
    [SubSlashCommand("patrol_create", "set up the patrol")]
    public async Task<string> SetupPatrolAsync([SlashCommandParameter(Description = "The name of the new patrol")] string patrolName)
    {

        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
            { "name", patrolName },
            { "nameLower", patrolName.ToLower().Replace(' ', '-') },
        };
        roleChannelBuilder.CreateAllFromTemplate(Context, rootConfig.Value.PatrolTemplate, substitutions);

        return "Continuing Setup in background";

    }

    /// <summary>
    /// list the set of existing patrols
    /// </summary>
    /// <returns></returns>
    [SubSlashCommand("patrol_list", "list existing patrols")]
    public async Task<string> ListPatrolsAsync()
    {
        var patrolRoles = Context.Guild.Roles.Values.Where(r => r.Name.EndsWith(" Patrol")).Select(r => r.Name).ToList();
        if (patrolRoles.Count == 0)
            return "No patrols found.";
        return "Existing patrols:\n" + string.Join("\n", patrolRoles);
    }

    [SubSlashCommand("help", "list available sbot commands")]
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

    [SubSlashCommand("shazbot", "Trash the entire server - testing only")]
    public async Task<string> ShazbotAsync()
    {
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

    /// <summary>
    /// delete a patrol by removing roles and channels from the patrol template
    /// </summary>
    /// <param name="patrolName"></param>
    /// <returns></returns>
    /// <remarks>
    /// currently we're not deleting patrols to avoid accidental data loss
    /// </remarks>
    [SubSlashCommand("patrol_delete", "delete a patrol")]
    //public async Task<string> DeletePatrolAsync([SlashCommandParameter(Description = "The name of the patrol to delete")] string patrolName)
    public async Task<string> DeletePatrolAsync([SlashCommandParameter(Description = "The name of the patrol")] string patrolName)
    {
        return "We're currently not deleting patrols. Please contact an administrator.";

        Dictionary<string, string> substitutions = new Dictionary<string, string>()
        {
            { "name", patrolName },
            { "nameLower", patrolName.ToLower().Replace(' ', '-') },
        };
        roleChannelBuilder.DeleteChannelFromTemplate(Context, rootConfig.Value.PatrolTemplate, substitutions, restClient);

        return "Continuing Removal in background";
    }

    /// <summary>
    /// rename a patrol by renaming roles and channels from the patrol template
    /// </summary>
    /// <param name="patrolName"></param>
    /// <param name="newPatrolName"></param>
    /// <returns></returns>
    [SubSlashCommand("patrol_rename", "rename a patrol")]
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

        roleChannelBuilder.RenameChannelFromTemplate(Context, rootConfig.Value.PatrolTemplate, substitutions);

        return $"Renaming patrol {patrolName} to {newPatrolName}";
    }

    #endregion
}


//this doesn't quite do what I want yet
//  it works, but doesn't talk to the roles and other pieces of the command yet.

//EnumTypeReaderManager<SlashCommandEnumTypeReader<TContext>, Type, SlashCommandParameter<TContext>, ApplicationCommandServiceConfiguration<TContext>?> enumInfoManager
//public class PatrolNameChoicesProvider(ApplicationCommandServiceConfiguration<ApplicationCommandContext>? context) : IChoicesProvider<ApplicationCommandContext>
//public class PatrolNameChoicesProvider : IChoicesProvider<ApplicationCommandContext>
//{
//    public async ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(SlashCommandParameter<ApplicationCommandContext> parameter)
//    {
//        //  need to get the guild roles and filter them
//        //        var roles = context.Guild.Roles.Values;

//        //        var patrolRoles = roles.Where(r => r.Name.EndsWith(" Patrol")).Select(r => r.Name).ToList();

//        List<string> patrolRoles = ["patrol 1", "patrol 2", "patrol 3", "squidge monkey"];

//        List<ApplicationCommandOptionChoiceProperties> choices = new List<ApplicationCommandOptionChoiceProperties>();
//        foreach (var patrol in patrolRoles)
//        {
//            choices.Add(new ApplicationCommandOptionChoiceProperties(patrol, patrol));
//        }

//        return choices;
//    }
//}


//public class PatrolNameAutocompleteProvider : IAutocompleteProvider<AutocompleteInteractionContext>
//{

//    public async ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
//    {
//        var roles = context.Guild.Roles.Values;

//        var patrolRoles = roles.Where(r => r.Name.EndsWith(" Patrol")).Select(r => r.Name).ToList();

//        List<ApplicationCommandOptionChoiceProperties> choices = new List<ApplicationCommandOptionChoiceProperties>();
//        foreach (var patrol in patrolRoles)
//        {
//            choices.Add(new ApplicationCommandOptionChoiceProperties(patrol, patrol));
//        }

//        return choices;
//    }
//}
