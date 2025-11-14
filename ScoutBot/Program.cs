using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScoutBot.Configuration;
using ScoutBot.Database;
using ScoutBot.Modules;
using System.Globalization;
using System.Reflection;

namespace ScoutBot;

internal class Program
{

    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


        DiscordSocketConfig _socketConfig = new()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true,
        };

        InteractionServiceConfig _interactionServiceConfig = new()
        {
            //            LocalizationManager = new ResxLocalizationManager("InteractionFramework.Resources.CommandLocales", Assembly.GetEntryAssembly(),
            //                new CultureInfo("en-US"), new CultureInfo("ru"))
        };


        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddDiscordNetService(_socketConfig, _interactionServiceConfig, configuration);
        builder.Services.AddDbContext<SbotDbContext>(x =>
            {
                x.UseSqlite(configuration.GetConnectionString("SbotDB"));
            });

        builder.Configuration.AddConfiguration(configuration);

        using IHost host = builder.Build();

        await host.RunAsync();
    }
}

public static class DiscordClientExtensions
{
    public static IServiceCollection AddDiscordNetService(
        this IServiceCollection services, 
        DiscordSocketConfig _socketConfig, 
        InteractionServiceConfig _interactionServiceConfig,
        IConfiguration configuration)
    {
        services.AddHostedService<DiscordHostService>();

        services
            .AddSingleton(_socketConfig)
            //.AddSingleton<IConfiguration>(configuration)
            .AddSingleton(_interactionServiceConfig)
            .AddSingleton<InteractionHandler>()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>(), _interactionServiceConfig))
            //.AddSingleton<InteractionService>()
            ;

        services
            .AddOptions<RootConfiguration>()
                .Configure(x => configuration.Bind(x));
        services
            .AddOptions<NetcordConfiguration>()
                .Configure(x => configuration.GetSection("Discord").Bind(x));
            ;

        return services;
    }
}
