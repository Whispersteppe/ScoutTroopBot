using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace ScoutBot.Modules;

public partial class SbotCommandContainer : InteractionModuleBase<SocketInteractionContext>
{
    [Group("utility", "common commands")]
    public class UtilityCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private InteractionService _commands { get; set; }
        private InteractionHandler _handler;
        private readonly ILogger<UtilityCommands> _logger;

        public UtilityCommands(InteractionHandler handler, ILogger<UtilityCommands> logger, InteractionService commands)
        {
            _handler = handler;
            _logger = logger;
            _commands = commands;
        }

        [SlashCommand("killall", "kill all channels and roles (TESTING ONLY)")]
        public async Task Create(string affirmation)
        {
            await RespondAsync("killing all...");
        }
    }
}
