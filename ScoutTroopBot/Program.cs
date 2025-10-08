using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services.ComponentInteractions;
using ScoutTroopBot.AppCommands;
using ScoutTroopBot.AppCommands.SlashCommands;
using ScoutTroopBot.Configuration;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutTroopBot;

/// <summary>
/// entry point into the bot
/// </summary>
internal class Program
{
    /// <summary>
    /// entry point into the bot
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static async Task Main(string[] args)
    {
        //  bot dev is at https://discord.com/developers/applications/1422324397032738959/information
        // the netcord documentation https://netcord.dev/docs/

        var builder = Host.CreateApplicationBuilder(args);

        builder.Configuration
            .AddCommandLine(args)
            .AddJsonFile("appsettings.json");

        builder.Services
            .AddDiscordGateway(options =>
            {
                var netcordConfig = builder.Configuration.GetSection("Discord").Get<NetcordConfiguration>();

                options.Intents = GatewayIntents.GuildMessages
                    //| GatewayIntents.All
                    | GatewayIntents.DirectMessages
                    | GatewayIntents.MessageContent
                    | GatewayIntents.DirectMessageReactions
                    | GatewayIntents.GuildMessageReactions
                    | GatewayIntents.AllNonPrivileged
                    ;

                options.Token = netcordConfig.Token;
            })
            .AddApplicationCommands()
            //.AddComponentInteractions()
            .AddCommands(options =>
            {
                options.Prefix = "!";
            })
            .AddGatewayHandlers(typeof(Program).Assembly)
            .AddLogging()
            .AddComponentInteractions<ModalInteraction, ModalInteractionContext>()
            .AddComponentInteractions<ButtonInteraction, ButtonInteractionContext>()
            //.AddComponentInteractions<RegisterScoutButtonHandler, ButtonInteractionContext>()
            .AddScoped<NetcordComponentBuilder>()
            ;

        builder.Services
            .AddOptions<RootConfiguration>()
            .BindConfiguration("");
            ;

        var host = builder.Build();
        host.AddModules(typeof(Program).Assembly);

        //DumpConfiguration(args);

        await host.RunAsync();
    }

    /// <summary>
    /// a method I have just to dump the configuration so I can figure out errors in it. Not used in production.
    /// </summary>
    /// <param name="root"></param>
    /// <remarks>
    /// the permission flags were giving me issues.  if they aren't right, then the 
    /// entire structure they are on just 'disappears' when deserialized.
    /// </remarks>
    private static void DumpConfiguration(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .AddJsonFile("appsettings.json")
            .Build();

        var root = configuration.Get<RootConfiguration>();

        JsonSerializerOptions settings = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        Debug.WriteLine(JsonSerializer.Serialize(root.MeritBadgeTemplate, settings));
    }


}
