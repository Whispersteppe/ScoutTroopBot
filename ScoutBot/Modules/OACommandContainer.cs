using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScoutBot.Configuration;
using System.Dynamic;

namespace ScoutBot.Modules;
public partial class SbotCommandContainer : InteractionModuleBase<SocketInteractionContext>
{
    [Group("oa", "common commands")]
    public class OACommands : InteractionModuleBase<SocketInteractionContext>
    {
        private InteractionService _commands { get; set; }
        private InteractionHandler _handler;
        private readonly ILogger<OACommands> _logger;
        RootConfiguration _configuration;

        public OACommands(
            InteractionHandler handler, 
            ILogger<OACommands> logger, 
            InteractionService commands,
            IOptions<RootConfiguration> configuration)
        {
            _handler = handler;
            _logger = logger;
            _commands = commands;
            _configuration = configuration.Value;
        }

        [SlashCommand("create", "create")]
        public async Task Create()
        {
            await RespondAsync("Creating OA roles and channels...");

            var oaTemplate = _configuration.Templates["OATemplate"];

            SubstitutionHelper substitutions = new SubstitutionHelper
            {
            };
            
            TemplateComponentBuilder templateBuilder = new TemplateComponentBuilder(_logger, Context, substitutions);

            await templateBuilder.CreateAllFromTemplate(oaTemplate);

            await FollowupAsync("OA Roles and Channels created");
        }

        [SlashCommand("remove", "create")]
        public async Task Remove()
        {
            await RespondAsync("Removing OA roles and channels...");

            var oaTemplate = _configuration.Templates["OATemplate"];

            SubstitutionHelper substitutions = new SubstitutionHelper()
            {
            };
            
            TemplateComponentBuilder templateBuilder = new TemplateComponentBuilder(_logger, Context, substitutions);

            await templateBuilder.DeleteAllFromTemplate(oaTemplate);

            await FollowupAsync("OA Roles and Channels removed");
        }
    }
}