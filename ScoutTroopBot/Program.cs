using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services.ComponentInteractions;
using ScoutTroopBot.AppCommands;
using ScoutTroopBot.Configuration;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScoutTroopBot;

internal class Program
{
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
            .AddScoped<RoleChannelBuilder>()
            ;

        builder.Services
            .AddOptions<RootConfiguration>()
            .BindConfiguration("");
            ;

        var host = builder.Build();
        host.AddModules(typeof(Program).Assembly);

        //var configuration = new ConfigurationBuilder()
        //    .AddCommandLine(args)
        //    .AddJsonFile("appsettings.json")
        //    .Build();

        //var root = configuration.Get<RootConfiguration>();

        //JsonSerializerOptions settings = new JsonSerializerOptions
        //{
        //    WriteIndented = true,
        //    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        //};

        //Debug.WriteLine(JsonSerializer.Serialize(root.PatrolTemplate, settings));

        //root.PatrolTemplate.Categories[0].Channels.Add(new ChannelTemplateConfiguration
        //{
        //    Name = "{PatrolName} - Test",
        //    Type = ChannelType.TextGuildChannel,
        //    PermissionOverwrites = new List<PermissionOverwriteTemplateConfiguration>
        //    {
        //        new PermissionOverwriteTemplateConfiguration
        //        {
        //            RoleName = "{PatrolName} Patrol",
        //            Type = PermissionOverwriteType.Role,
        //            Allowed = new List<Permissions>() { Permissions.ViewChannel , Permissions.SendMessages , Permissions.ReadMessageHistory },
        //            Denied =   new List<Permissions>(){ Permissions.ViewChannel , Permissions.SendMessages , Permissions.ReadMessageHistory }
        //        },
        //        new PermissionOverwriteTemplateConfiguration
        //        {
        //            RoleName = "{PatrolName} Patrol",
        //            Type = PermissionOverwriteType.Role,
        //            Allowed =  new List<Permissions>(){ Permissions.ViewChannel , Permissions.SendMessages , Permissions.ReadMessageHistory },
        //            Denied =  new List<Permissions>() { Permissions.ViewChannel , Permissions.SendMessages , Permissions.ReadMessageHistory }
        //        }
        //    }
        //});
        //Debug.WriteLine(JsonSerializer.Serialize(root.PatrolTemplate, settings));

        await host.RunAsync();
    }


}
