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
[SlashCommand("sbot-patrol", "sbot commands", DefaultGuildPermissions = Permissions.Administrator)]
public class SbotPatrolCommands(ILogger<SbotPatrolCommands> logger,
    RestClient restClient,
    IOptions<RootConfiguration> rootConfig,
    NetcordComponentBuilder roleChannelBuilder) : ApplicationCommandModule<ApplicationCommandContext>
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
        roleChannelBuilder.CreateAllFromTemplate(Context, rootConfig.Value.PatrolTemplate, substitutions);

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
    //public async Task<string> DeletePatrolAsync([SlashCommandParameter(Description = "The name of the patrol to delete")] string patrolName)
    public async Task<string> DeletePatrolAsync([SlashCommandParameter(Description = "The name of the patrol")] string patrolName)
    {
        //return "We're currently not deleting patrols. Please contact an administrator.";

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

        roleChannelBuilder.RenameChannelFromTemplate(Context, rootConfig.Value.PatrolTemplate, substitutions);

        return $"Renaming patrol {patrolName} to {newPatrolName}";
    }
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
