using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScoutBot.Configuration;
using ScoutBot.Extensions;

namespace ScoutBot;

public class DiscordHostService : IHostedService, IHostedLifecycleService
{
    private readonly ILogger _logger;
    IHostApplicationLifetime _appLifetime;
    DiscordSocketClient _client;
    InteractionHandler _interactionHandler;
    IOptions<NetcordConfiguration> _discordOptions;

    public DiscordHostService(
        ILogger<DiscordHostService> logger,
        IHostApplicationLifetime appLifetime,
        DiscordSocketClient client,
        InteractionHandler interactionHandler,
        IOptions<NetcordConfiguration> discordOptions)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _client = client;
        _interactionHandler = interactionHandler;
        _discordOptions = discordOptions;

        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopping.Register(OnStopping);
        _appLifetime.ApplicationStopped.Register(OnStopped);
    }

    private async Task Log(LogMessage msg)
    {
        _logger.Log(msg.Severity.GetLogLevelFromSeverity(), msg.Exception, "Discord Log: {Message}", msg.ToString());
        await Task.CompletedTask;
    }

    async Task IHostedLifecycleService.StartingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("1. StartingAsync has been called.");
        _client.Log += Log;

        await _interactionHandler.InitializeAsync();
    }

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("2. StartAsync has been called.");

        var token = _discordOptions.Value.Token;
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    async Task IHostedLifecycleService.StartedAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("3. StartedAsync has been called.");
    }

    private void OnStarted()
    {
        _logger.LogInformation("4. OnStarted has been called.");
    }

    private void OnStopping()
    {
        _logger.LogInformation("5. OnStopping has been called.");
    }

    async Task IHostedLifecycleService.StoppingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("6. StoppingAsync has been called.");
        //await _client.StopAsync();
    }

    async Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("7. StopAsync has been called.");
        await Task.CompletedTask;

    }

    async Task IHostedLifecycleService.StoppedAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("8. StoppedAsync has been called.");
        await Task.CompletedTask;
    }

    private void OnStopped()
    {
        _logger.LogInformation("9. OnStopped has been called.");
    }

}
