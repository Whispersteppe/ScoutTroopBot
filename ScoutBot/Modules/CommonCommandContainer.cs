using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace ScoutBot.Modules;

public partial class SbotCommandContainer : InteractionModuleBase<SocketInteractionContext>
{
    [Group("common", "common commands")]
    public class CommonCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private InteractionService _commands { get; set; }
        private InteractionHandler _handler;
        private readonly ILogger<CommonCommands> _logger;

        public CommonCommands(InteractionHandler handler, ILogger<CommonCommands> logger, InteractionService commands)
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
    }
}