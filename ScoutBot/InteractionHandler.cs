using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ScoutBot.Extensions;
using System.Reflection;

namespace ScoutBot;

//TODO possibly merge this into the DiscordHostService
public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _handler;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    ILogger<InteractionHandler> _logger;
    public InteractionHandler(
        DiscordSocketClient client,
        InteractionService handler,
        IServiceProvider services,
        IConfiguration config,
        ILogger<InteractionHandler> logger)
    {
        _client = client;
        _handler = handler;
        _services = services;
        _configuration = config;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        // Process when the client is ready, so we can register our commands.
        _client.Ready += ReadyHandler;
        _handler.Log += LogHandler;

        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        // Process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += InteractionCreatedHandler;

        // Also process the result of the command execution.
        _handler.InteractionExecuted += HandleInteractionExecute;

        _client.UserJoined += UserJoinedHandler;
    }

    private async Task UserJoinedHandler(SocketGuildUser user)
    {
        var guild = user.Guild;
        var channelId = _configuration.GetValue<ulong?>("WelcomeChannelId");
        if (channelId.HasValue)
        {
            var welcomeChannel = guild.GetTextChannel(channelId.Value);
            if (welcomeChannel != null)
            {
                await welcomeChannel.SendMessageAsync($"Welcome to the server, {user.Mention}!");
            }
        }

        var dmChannel = await user.CreateDMChannelAsync();
        await dmChannel.SendMessageAsync("Welcome! Here are some important links and information to get started: [Link 1], [Link 2]");

        await CreateStarterButton(dmChannel);
    }

    private async Task CreateStarterButton(IDMChannel dmChannel)
    {
        ActionRowBuilder rowBuilder = new ActionRowBuilder();
        rowBuilder.AddComponent(new ButtonBuilder()
        {
            CustomId = $"sbot_interaction_begin:hello", 
            Label = "Press to Start",
        });

        var builder = new ComponentBuilder();
        builder.AddRow(rowBuilder);

        await  dmChannel.SendMessageAsync("Welcome to the server.  Click on the button below to get started.  we need to ask a few questions before we can assign you to the appropriate roles", components: builder.Build());
    }


    //TODO this is copied from DiscordHostService, refactor to avoid duplication
    private async Task LogHandler(LogMessage msg)
    {
        _logger.Log(msg.Severity.GetLogLevelFromSeverity(), msg.Exception, "Discord Log: {Message}", msg.ToString());
        await Task.CompletedTask;
    }


    private async Task ReadyHandler()
    {
        // Register the commands globally.
        // alternatively you can use _handler.RegisterCommandsGloballyAsync() to register commands to a specific guild.
        await _handler.RegisterCommandsGloballyAsync();
    }

    private async Task InteractionCreatedHandler(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(_client, interaction);

            // Execute the incoming command.
            var result = await _handler.ExecuteCommandAsync(context, _services);

            // Due to async nature of InteractionFramework, the result here may always be success.
            // That's why we also need to handle the InteractionExecuted event.
            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    default:
                        break;
                }
        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
        }
    }

    private Task HandleInteractionExecute(ICommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess)
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                default:
                    break;
            }

        return Task.CompletedTask;
    }
}
