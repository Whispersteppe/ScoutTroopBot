using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace ScoutTroopBot.AppCommands.SlashCommands;

/// <summary>
/// set up the server with common roles and channels
/// </summary>
/// <param name="logger"></param>
/// <param name="restClient"></param>
[SlashCommand("setup", "Manages setup", DefaultGuildPermissions = Permissions.Administrator)]
public class SetupCommand(ILogger<SetupCommand> logger, RestClient restClient) : ApplicationCommandModule<ApplicationCommandContext>
{
    /// <summary>
    /// command to set up common roles and channels
    /// </summary>
    /// <returns></returns>
    [SubSlashCommand("common", "set up the server")]
    public async Task<string> SetupServerAsync()
    {
        //TODO use the RoleChannelBuilder to do this from a template

        ServerSetupBackgroundAsync();

        return "Continuing Setup in background";
    }

    //  todo need some utility functions to normalize names (lowercase, replace spaces with dashes, etc)
    //  todo need some DI utilities to wortk with the configuration templates

    /// <summary>
    /// background task to set up the server
    /// </summary>
    /// <returns></returns>
    private async Task ServerSetupBackgroundAsync()
    {
        await Context.Channel.SendMessageAsync("Setting up server...");

        var currentRoles = Context.Guild.Roles;
        var currentChannels = Context.Guild.Channels;

        await SetupRoles(currentRoles);
        await SetupChannels(currentChannels, currentRoles);

        await Context.Channel.SendMessageAsync("Setup complete!");
    }
    private async Task SetupChannels(IReadOnlyDictionary<ulong,IGuildChannel> channels, IReadOnlyDictionary<ulong, Role> roles)
    {
        await Context.Channel.SendMessageAsync("Setting up channels...");

        var parentCategory = await CreateChannelIfNotExists(new GuildChannelProperties("welcome", ChannelType.CategoryChannel)
        {
            //we set NOTHING on a category and like it that way
            //Topic = "Welcome to the Troop! Please read the rules and get verified.", 
            //Type = ChannelType.CategoryChannel, 
            //Nsfw = false, 
            //Name = "welcome", 
            //ParentId = null, 

        }, channels);

        await CreateChannelIfNotExists(new GuildChannelProperties("rules", ChannelType.TextGuildChannel)
        {
            Topic = "Please read the rules before participating in the server.",
            ParentId = parentCategory?.Id,
        }, channels);

        await CreateChannelIfNotExists(new GuildChannelProperties("announcements", ChannelType.TextGuildChannel)
        {
            Topic = "Official announcements from the troop leadership.",
            ParentId = parentCategory?.Id,
        }, channels);
        await Context.Channel.SendMessageAsync("Channel setup complete...");
    }

    private async Task<IGuildChannel> CreateChannelIfNotExists(GuildChannelProperties newChannel, IReadOnlyDictionary<ulong, IGuildChannel> channels)
    {
 
        if (!channels.Values.Any(x => x.Name == newChannel.Name))
        {
            try
            {
                var createdChannel = await Context.Guild.CreateChannelAsync(newChannel);
                return createdChannel;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating channel: {channelName}", newChannel.Name);
            }

            logger.LogInformation("Created channel: {channelName}", newChannel.Name);
        }
        else
        {
            logger.LogInformation("Channel already exists: {channelName}", newChannel.Name);
        }

        return null;
    }

    private async Task SetupRoles(IReadOnlyDictionary<ulong, Role> roles)
    {
        await Context.Channel.SendMessageAsync("Setting up roles...");

        await CreateRoleIfNotExists("Unverified", new RoleProperties()
        {
            Name = "Unverified",
            Color = new Color(0x808080),
            Permissions = Permissions.ViewChannel | Permissions.SendMessages | Permissions.ChangeNickname,
            Mentionable = true,
            Hoist = true
        }, roles);

        await CreateRoleIfNotExists("Registered Adult", new RoleProperties()
        {
            Name = "Registered Adult",
            Color = new Color(0x808080),
            Permissions = Permissions.ViewChannel | Permissions.SendMessages | Permissions.ChangeNickname,
            Mentionable = true,
            Hoist = true
        }, roles);

        await CreateRoleIfNotExists("Registered Scout", new RoleProperties()
        {
            Name = "Registered Scout",
            Color = new Color(0x808080),
            Permissions = Permissions.ViewChannel | Permissions.SendMessages | Permissions.ChangeNickname,
            Mentionable = true,
            Hoist = true
        }, roles);

        await CreateRoleIfNotExists("Registered Crew Member", new RoleProperties()
        {
            Name = "Registered Crew Member",
            Color = new Color(0x808080),
            Permissions = Permissions.ViewChannel | Permissions.SendMessages | Permissions.ChangeNickname,
            Mentionable = true,
            Hoist = true
        }, roles);

        await CreateRoleIfNotExists("Committee Member", new RoleProperties()
        {
            Name = "Committee Member",
            Color = new Color(0x808080),
            Permissions = Permissions.ViewChannel | Permissions.SendMessages | Permissions.ChangeNickname,
            Mentionable = true,
            Hoist = true
        }, roles);

        await CreateRoleIfNotExists("Crew Council", new RoleProperties()
        {
            Name = "Crew Council",
            Color = new Color(0x808080),
            Permissions = Permissions.ViewChannel | Permissions.SendMessages | Permissions.ChangeNickname,
            Mentionable = true,
            Hoist = true
        }, roles);

        await CreateRoleIfNotExists("PLC", new RoleProperties()
        {
            Name = "PLC",
            Color = new Color(0x808080),
            Permissions = Permissions.ViewChannel | Permissions.SendMessages | Permissions.ChangeNickname,
            Mentionable = true,
            Hoist = true
        }, roles);

        await CreateRoleIfNotExists("Merit Badge Councilor", new RoleProperties()
        {
            Name = "Merit Badge Councilor",
            Color = new Color(0x808080),
            Permissions = Permissions.ViewChannel | Permissions.SendMessages | Permissions.ChangeNickname,
            Mentionable = true,
            Hoist = true
        }, roles);

        await CreateRoleIfNotExists("Moderator", new RoleProperties()
        {
            Name = "Moderator",
            Color = new Color(0x808080),
            Permissions = Permissions.ModerateUsers | Permissions.ManageMessages | Permissions.KickUsers | Permissions.BanUsers | Permissions.ViewAuditLog | Permissions.ViewChannel | Permissions.SendMessages | Permissions.ChangeNickname,
            Mentionable = true,
            Hoist = true
        }, roles);

        await Context.Channel.SendMessageAsync("Role setup complete...");
    }

    private async Task CreateRoleIfNotExists(string roleName, RoleProperties properties, IReadOnlyDictionary<ulong, Role> roles)
    {
        //var roles = await Context.Guild.GetRolesAsync();
        if (!roles.Values.Any(r => r.Name == roleName))
        {
            var newRole = await Context.Guild.CreateRoleAsync(properties);

            logger.LogInformation("Created role: {roleName}", roleName);
        }
        else
        {
            logger.LogInformation("Role already exists: {roleName}", roleName);
        }
    }

}


