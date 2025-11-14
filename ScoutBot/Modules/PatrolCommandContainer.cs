using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace ScoutBot.Modules;

public partial class SbotCommandContainer : InteractionModuleBase<SocketInteractionContext>
{
    [Group("patrol", "Patrol commands")]
    public class PatrolCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private InteractionService _commands { get; set; }
        private InteractionHandler _handler;
        private readonly ILogger<PatrolCommands> _logger;

        public PatrolCommands(InteractionHandler handler, ILogger<PatrolCommands> logger, InteractionService commands)
        {
            _handler = handler;
            _logger = logger;
            _commands = commands;
        }

        [SlashCommand("create", "create")]
        public async Task Create(string patrolName)
        {
            await RespondAsync("Creating...");
        }

        [SlashCommand("rename", "rename")]
        public async Task Rename(string fromName, string toName)
        {
            await RespondAsync($"rename");
        }

        [SlashCommand("list", "list")]
        public async Task List()
        {
            await RespondAsync($"list");
        }
    }
}
