using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using ScoutTroopBot.Configuration;

namespace ScoutTroopBot.AppCommands;

public class RoleChannelBuilder(ILogger<RoleChannelBuilder> logger) 
{
    public async Task CreateFromTemplate(
        ApplicationCommandContext Context,
        TemplateConfiguration template,
        Dictionary<string, string> substitutions)
    {
        List<Role> roles = Context.Guild.Roles.Values.ToList();

        foreach (var roleTemplate in template.Roles)
        {
            var newRole = await TranslateRole(roleTemplate, roles, substitutions, Context);
            if (!roles.Any(r => r.Id == newRole.Id))
            {
                roles.Add(newRole);
            }
        }

        List<IGuildChannel> channels = Context.Guild.Channels.Values.ToList();
        foreach(var categoryTemplate in template.Categories)
        {
            var netCategory = await TranslateCategory(categoryTemplate, channels, substitutions, roles, Context);
            if (netCategory != null && !channels.Any(x => x.Name == netCategory.Name))
            {
                channels.Add(netCategory);
            }

            // Now create child channels
            foreach(var channelTemplate in categoryTemplate.Channels)
            {
                var childChannel = await TranslateChannel(channelTemplate, netCategory, channels, substitutions, Context);
                if (childChannel != null && !channels.Any(x => x.Name == childChannel.Name))
                {
                    channels.Add(childChannel);
                }
            }
        }

        await Context.Channel.SendMessageAsync($"{template.ItemType} created");

    }

    public async Task DeleteFromTemplate(
        ApplicationCommandContext Context,
        TemplateConfiguration template,
        Dictionary<string, string> substitutions, 
        RestClient restClient)
    {
        await Context.Channel.SendMessageAsync($"Deleting {template.ItemType}...");

        List<Role> roles = Context.Guild.Roles.Values.ToList();
        List<IGuildChannel> channels = Context.Guild.Channels.Values.ToList();

        foreach (var categoryTemplate in template.Categories)
        {
            foreach (var channelTemplate in categoryTemplate.Channels)
            {
                var channelName = DoSubstitutions(channelTemplate.Name, substitutions);
                var channel = channels.FirstOrDefault(x => x.Name == channelName);
                if (channel != null)
                {
                    await restClient.DeleteChannelAsync(channel.Id);
                    await Context.Channel.SendMessageAsync($"Channel {channelName} removed");
                }
            }

            var categoryName = DoSubstitutions(categoryTemplate.Name, substitutions);
            var category = channels.FirstOrDefault(x => x.Name == categoryName);
            if (category != null)
            {
                await restClient.DeleteChannelAsync(category.Id);
                await Context.Channel.SendMessageAsync($"Category {categoryName} removed");
            }
        }

        foreach (var roleTemplate in template.Roles)
        {
            var roleName = DoSubstitutions(roleTemplate.Name, substitutions);
            var role = roles.FirstOrDefault(x => x.Name == roleName);
            if (role != null)
            {
                await role.DeleteAsync();
                await Context.Channel.SendMessageAsync($"Role {roleName} removed");

            }
        }

        await Context.Channel.SendMessageAsync($"{template.ItemType} Removed");

    }
    public async Task RenameFromTemplate(
        ApplicationCommandContext Context,
        TemplateConfiguration template,
        Dictionary<string, string> substitutions)
    {
        await Context.Channel.SendMessageAsync($"Renaming {template.ItemType}...");

        List<Role> roles = Context.Guild.Roles.Values.ToList();
        List<IGuildChannel> channels = Context.Guild.Channels.Values.ToList();

        foreach (var categoryTemplate in template.Categories)
        {
            foreach (var channelTemplate in categoryTemplate.Channels)
            {
                var channelName = DoSubstitutions(channelTemplate.Name, substitutions);
                var channel = channels.FirstOrDefault(x => x.Name == channelName);
                if (channel != null)
                {
                    await channel.ModifyAsync(x =>
                    {
                        x.Name = channel.Name.Replace(substitutions["nameLower"], substitutions["toNameLower"]);
                    });

                    await Context.Channel.SendMessageAsync($"Channel {channelName} updated");
                }
            }

            var categoryName = DoSubstitutions(categoryTemplate.Name, substitutions);
            var category = channels.FirstOrDefault(x => x.Name == categoryName);
            if (category != null)
            {
                await category.ModifyAsync(x =>
                {
                    x.Name = category.Name.Replace(substitutions["nameLower"], substitutions["toNameLower"]);
                });

                await Context.Channel.SendMessageAsync($"Category {categoryName} updated");
            }
        }

        foreach (var roleTemplate in template.Roles)
        {
            var roleName = DoSubstitutions(roleTemplate.Name, substitutions);
            var role = roles.FirstOrDefault(x => x.Name == roleName);
            if (role != null)
            {
                await role.ModifyAsync(x =>
                {
                    x.Name = role.Name.Replace(substitutions["name"], substitutions["toName"]);
                });
                await Context.Channel.SendMessageAsync($"Role {roleName} updated");

            }
        }

        await Context.Channel.SendMessageAsync($"{template.ItemType} Renamed");

    }

    private string DoSubstitutions(string input, Dictionary<string, string> substitutions)
    {
        var output = input;
        foreach(var kvp in substitutions)
        {
            string key = "{" + kvp.Key + "}";
            output = output.Replace(key, kvp.Value);
        }
        return output;
    }

    private async Task<IGuildChannel> TranslateCategory(
        CategoryTemplateConfiguration categoryTemplate, 
        List<IGuildChannel> channels, 
        Dictionary<string, string> substitutions, 
        List<Role> roles,
        ApplicationCommandContext Context)
    {
        var categoryName = DoSubstitutions(categoryTemplate.Name, substitutions);
        await Context.Channel.SendMessageAsync($"Creating category {categoryName}");

        var newCategory = new GuildChannelProperties(categoryName, ChannelType.CategoryChannel)
        { 
            PermissionOverwrites = TranslatePermissionOverwrites(categoryTemplate.PermissionOverwrites, substitutions, roles, Context)

        };

        var createdCategory = await CreateChannelIfNotExists(newCategory, channels, Context);
        return createdCategory;
    }

    private List<PermissionOverwriteProperties>? TranslatePermissionOverwrites(
        List<PermissionOverwriteTemplateConfiguration> permissionTemplates, 
        Dictionary<string, string> substitutions, 
        List<Role> roles,
        ApplicationCommandContext Context)
    {

        var PermissionOverwrites = new List<PermissionOverwriteProperties>();
        foreach (var permission in permissionTemplates)
        {
            var roleName = DoSubstitutions(permission.RoleName, substitutions);
            var role = roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null && roleName.Equals("everyone", StringComparison.CurrentCultureIgnoreCase))
            {
                role = Context.Guild.EveryoneRole;
            }

            if (role != null)
            {
                var overwrite = new PermissionOverwriteProperties(role.Id, PermissionOverwriteType.Role)
                {
                    Id = role.Id,
                    Type = PermissionOverwriteType.Role,
                    Allowed = permission.Allowed == null || permission.Allowed.Count == 0
                        ? null
                        : permission.Allowed.Aggregate(permission.Allowed[0], (acc, perm) => acc | perm),
                    Denied = permission.Denied == null || permission.Denied.Count == 0
                        ? null
                        : permission.Denied.Aggregate(permission.Denied[0], (acc, perm) => acc | perm)
                };
                PermissionOverwrites.Add(overwrite);
            }
            else
            {
                logger.LogWarning("Role with ID {roleName} not found for permission overwrite", permission.RoleName);
            }
        }

        return PermissionOverwrites;
    }

    private async Task<IGuildChannel> TranslateChannel(ChannelTemplateConfiguration channelTemplate, IGuildChannel parent, List<IGuildChannel> channels, Dictionary<string, string> substitutions, ApplicationCommandContext Context)
    {
        var categoryName = DoSubstitutions(channelTemplate.Name, substitutions);
        await Context.Channel.SendMessageAsync($"Creating category {categoryName}");

        var newCategory = new GuildChannelProperties(categoryName, channelTemplate.Type)
        {
            Topic = DoSubstitutions(channelTemplate.Topic, substitutions),
            ParentId = parent?.Id,
            PermissionOverwrites = new List<PermissionOverwriteProperties>()
        };

        var createdCategory = await CreateChannelIfNotExists(newCategory, channels, Context);
        return createdCategory;
    }


    private async Task<Role> TranslateRole(
        RoleTemplateConfiguration roleTemplate, 
        List<Role> roles, 
        Dictionary<string, string> substitutions, 
        ApplicationCommandContext Context)
    {
        var roleName = DoSubstitutions(roleTemplate.Name, substitutions);

        var properties = new RoleProperties
        {
            Name = roleName,
            Color = roleTemplate.Color,
            //Hoist = roleTemplate.Hoist,
            Mentionable = roleTemplate.Mentionable,
            Permissions = roleTemplate.Permissions == null || roleTemplate.Permissions.Count == 0 
                    ? null
                    : roleTemplate.Permissions.Aggregate(roleTemplate.Permissions[0], (acc, perm) => acc | perm)
        };

        var newRole = await CreateRoleIfNotExists(roleName, properties, roles, Context);
        return newRole;
    }

    private async Task<Role> CreateRoleIfNotExists(string roleName, RoleProperties properties, List<Role> roles, ApplicationCommandContext Context)
    {
        if (!roles.Any(r => r.Name == roleName))
        {
            var newRole = await Context.Guild.CreateRoleAsync(properties);

            logger.LogInformation("Created role: {roleName}", roleName);
            return newRole;
        }
        else
        {
            logger.LogInformation("Role already exists: {roleName}", roleName);
            return roles.First(r => r.Name == roleName);
        }
    }

    private async Task<IGuildChannel> CreateChannelIfNotExists(
        GuildChannelProperties newChannel, 
        List<IGuildChannel> channels, 
        ApplicationCommandContext Context)
    {
        if (!channels.Any(x => x.Name == newChannel.Name))
        {
            try
            {
                var createdChannel = await Context.Guild.CreateChannelAsync(newChannel);
                logger.LogInformation("Created channel: {channelName}", newChannel.Name);
                return createdChannel;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating channel: {channelName}", newChannel.Name);
                throw;
            }
        }
        else
        {
            logger.LogInformation("Channel already exists: {channelName}", newChannel.Name);
            return channels.First(x => x.Name == newChannel.Name);
        }
    }


}
