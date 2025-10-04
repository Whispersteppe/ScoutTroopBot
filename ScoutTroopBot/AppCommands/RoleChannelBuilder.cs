using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using ScoutTroopBot.Configuration;

namespace ScoutTroopBot.AppCommands;

/// <summary>
/// Builds roles and channels based on a template configuration
/// </summary>
/// <param name="logger"></param>
public class RoleChannelBuilder(ILogger<RoleChannelBuilder> logger) 
{
    /// <summary>
    /// Creates roles and channels in a guild based on the specified template configuration.
    /// </summary>
    /// <remarks>This method processes the provided template to create roles and channels in the specified
    /// guild.  It ensures that roles and channels are only added if they do not already exist in the guild. The method
    /// sends a confirmation message to the command's originating channel upon successful creation.</remarks>
    /// <param name="Context">The context of the application command, including the guild where the template will be applied.</param>
    /// <param name="template">The template configuration that defines the roles, categories, and channels to create.</param>
    /// <param name="substitutions">A dictionary of placeholder substitutions to apply to the template values.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
    /// <summary>
    /// Deletes channels, categories, and roles from a guild based on the specified template configuration.
    /// </summary>
    /// <remarks>This method iterates through the categories, channels, and roles defined in the provided
    /// template and deletes the corresponding entities in the guild. Placeholders in the template names are replaced
    /// using the provided <paramref name="substitutions"/> dictionary. Messages are sent to the command's channel to
    /// indicate the progress of the deletion process.</remarks>
    /// <param name="Context">The context of the application command, including the guild and channel where the command was invoked.</param>
    /// <param name="template">The template configuration that defines the categories, channels, and roles to delete.</param>
    /// <param name="substitutions">A dictionary of substitution values used to replace placeholders in the template names.</param>
    /// <param name="restClient">The REST client used to perform channel deletion operations.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>
    /// Renames channels, categories, and roles in a guild based on a specified template and a set of substitutions.
    /// </summary>
    /// <remarks>This method iterates through the categories, channels, and roles defined in the template and
    /// applies the specified substitutions to their names. If a matching category, channel, or role is found in the
    /// guild, its name is updated accordingly. Messages are sent to the invoking channel to indicate the progress and
    /// completion of the renaming operation.
    /// 
    /// This is primarily used for renaming patrols, e.g. changing "Alpha Patrol" to "Bravo Patrol".
    /// </remarks>
    /// <param name="Context">The context of the application command, providing access to the guild and channel where the command was invoked.</param>
    /// <param name="template">The template configuration that defines the structure and naming conventions for categories, channels, and roles
    /// to be renamed.</param>
    /// <param name="substitutions">A dictionary of key-value pairs used to replace placeholders in the template with specific values.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>
    /// Replaces placeholders in the input string with corresponding values from the substitutions dictionary.
    /// </summary>
    /// <remarks>Placeholders in the input string are identified by enclosing keys in curly braces (e.g.,
    /// <c>{key}</c>). The method performs a case-sensitive replacement for each key in the dictionary.</remarks>
    /// <param name="input">The input string containing placeholders in the format <c>{key}</c>.</param>
    /// <param name="substitutions">A dictionary where each key represents a placeholder (without braces) and the value is the replacement string.</param>
    /// <returns>A new string with all placeholders replaced by their corresponding values. If a placeholder does not have a
    /// matching key in the dictionary, it remains unchanged.</returns>
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

    /// <summary>
    /// Creates or retrieves a category channel based on the specified template, applying substitutions and permissions.
    /// </summary>
    /// <remarks>This method ensures that a category channel is created or reused based on the provided
    /// template and substitutions. If a matching category already exists, it is returned. Otherwise, a new category is
    /// created with the specified name and permission overwrites. A message is sent to the context channel to indicate
    /// the creation process.</remarks>
    /// <param name="categoryTemplate">The template configuration for the category, including its name and permission overwrites.</param>
    /// <param name="channels">The list of existing guild channels to check for duplicates before creating a new category.</param>
    /// <param name="substitutions">A dictionary of placeholder substitutions to apply to the category name and permissions.</param>
    /// <param name="roles">The list of roles used to configure permission overwrites for the category.</param>
    /// <param name="Context">The context of the application command, used for sending messages and creating channels.</param>
    /// <returns>The created or existing category channel that matches the specified template and substitutions.</returns>
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

    /// <summary>
    /// Translates a list of permission overwrite templates into a collection of permission overwrite properties based
    /// on the provided role mappings and substitutions.
    /// </summary>
    /// <remarks>This method resolves role names from the templates using the provided substitutions and
    /// matches them against the available roles. If a role cannot be resolved and the role name is "everyone", the
    /// guild's default "everyone" role is used. If a role cannot be resolved and is not "everyone", a warning is
    /// logged.</remarks>
    /// <param name="permissionTemplates">A list of permission overwrite templates that define the roles and their associated permissions.</param>
    /// <param name="substitutions">A dictionary of substitution mappings used to replace placeholders in role names with actual values.</param>
    /// <param name="roles">A list of roles available in the current context, used to resolve role names to role objects.</param>
    /// <param name="Context">The application command context, which provides access to the guild and its default "everyone" role.</param>
    /// <returns>A list of <see cref="PermissionOverwriteProperties"/> representing the resolved permission overwrites. Returns
    /// <see langword="null"/> if no valid permission overwrites are generated.</returns>
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

    /// <summary>
    /// Creates or retrieves a guild channel based on the specified template and applies the provided substitutions.
    /// </summary>
    /// <remarks>This method ensures that a channel matching the specified template and substitutions is
    /// created if it does not already exist. If a matching channel is found in the provided <paramref name="channels"/>
    /// list, it is returned instead of creating a new one.</remarks>
    /// <param name="channelTemplate">The template configuration that defines the properties of the channel to be created.</param>
    /// <param name="parent">The parent channel under which the new channel will be created, or <c>null</c> if no parent is specified.</param>
    /// <param name="channels">A list of existing guild channels to check for duplicates before creating a new channel.</param>
    /// <param name="substitutions">A dictionary of key-value pairs used to replace placeholders in the channel template's name and topic.</param>
    /// <param name="Context">The context of the application command, used for sending messages and interacting with the guild.</param>
    /// <returns>The created or existing guild channel that matches the specified template and substitutions.</returns>
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

    /// <summary>
    /// Creates a role based on the specified template, applying substitutions and checking for existing roles.
    /// </summary>
    /// <param name="roleTemplate"></param>
    /// <param name="roles"></param>
    /// <param name="substitutions"></param>
    /// <param name="Context"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Creates a role if it does not already exist in the guild.
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="properties"></param>
    /// <param name="roles"></param>
    /// <param name="Context"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Creates a channel if it does not already exist in the guild.
    /// </summary>
    /// <param name="newChannel"></param>
    /// <param name="channels"></param>
    /// <param name="Context"></param>
    /// <returns></returns>
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
