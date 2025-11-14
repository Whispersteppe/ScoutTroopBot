using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace ScoutBot.Modules;

public partial class SbotCommandContainer : InteractionModuleBase<SocketInteractionContext>
{
    [Group("unit", "common commands")]
    public class UnitCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private InteractionService _commands { get; set; }
        private InteractionHandler _handler;
        private readonly ILogger<UnitCommands> _logger;

        public UnitCommands(InteractionHandler handler, ILogger<UnitCommands> logger, InteractionService commands)
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

        [SlashCommand("list", "list")]
        public async Task List()
        {
            await RespondAsync($"list");
        }

    }
}