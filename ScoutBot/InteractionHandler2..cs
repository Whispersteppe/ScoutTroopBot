using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScoutBot.Extensions;
using System.Reflection;

namespace ScoutBot;


public interface IInteractionComponentHandler
{
    void RegisterForEvents(DiscordSocketClient client);
}

public class InteractionHandler2
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _handler;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    ILogger<InteractionHandler> _logger;

    List<IInteractionComponentHandler> _componentHandlers = new();

    public InteractionHandler2(
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

        //  get the list of handlers that have been registered
        //  alternatively, register them first and then resolve them
        _componentHandlers = _services.GetServices<IInteractionComponentHandler>().ToList();

        //  wire up the handlers with the client events
        foreach (var eventHandler in _componentHandlers)
        {
            if (eventHandler is IInteractionComponentHandler clientEventHandler)
            {
                clientEventHandler.RegisterForEvents(_client);
            }
        }
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

    private void RegisterEvent(IInteractionComponentHandler handler)
    {
        //  what event is this for?
        //  probably have the attributes with the event type stuff in there.  possibly a type reference to the event on DiscordSocketClient
        //  probably do a big ole switch statement in there
    }

}


public class DiscordEventAttribute : Attribute
{

    public EventInfo EventType => typeof(DiscordSocketClient).GetEvent(EventName);

    public virtual string EventName  => nameof(DiscordSocketClient.UserJoined);

    public void RegisterHandler(DiscordSocketClient client, Func<SocketGuildUser, Task> handler)
    {
        EventType.AddEventHandler(client, handler);    
    }

    public void UnregisterHandler(DiscordSocketClient client, Func<SocketGuildUser, Task> handler)
    {
        EventType.RemoveEventHandler(client, handler);
    }

}

public class UserJoinedEventAttribute : DiscordEventAttribute
{
    override public string EventName => nameof(DiscordSocketClient.UserJoined);
}