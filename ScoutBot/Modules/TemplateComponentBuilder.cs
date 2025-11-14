using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ScoutBot.Configuration;
using System;

namespace ScoutBot.Modules;

/// <summary>
/// Builds roles and channels based on a template configuration
/// </summary>
/// <param name="logger"></param>
public class TemplateComponentBuilder
{
    ILogger _logger;
    SocketInteractionContext _context;
    SubstitutionHelper? _substitutions;

    public TemplateComponentBuilder(ILogger logger,
        SocketInteractionContext context, SubstitutionHelper? substitutions = null)
    {
        _logger = logger;
        _context = context;
        _substitutions = substitutions;
    }

    public SubstitutionHelper Substitutions => _substitutions ?? new SubstitutionHelper();

    /// <summary>
    /// Creates roles and channels in a guild based on the specified template configuration.
    /// </summary>
    /// <remarks>This method processes the provided template to create roles and channels in the specified
    /// guild.  It ensures that roles and channels are only added if they do not already exist in the guild. The method
    /// sends a confirmation message to the command's originating channel upon successful creation.</remarks>
    /// <param name="template">The template configuration that defines the roles, categories, and channels to create.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task CreateAllFromTemplate(
        TemplateConfiguration template)
    {

        foreach (var roleTemplate in template.Roles)
        {
            var newRole = await CreateRoleIfNotExists(roleTemplate);
        }

        foreach (var categoryTemplate in template.Categories)
        {
            var categoryName = Substitutions.DoSubstitutions(categoryTemplate.Name);
            await _context.Channel.SendMessageAsync($"Creating category {categoryName}");

            var netCategory = await CreateCategoryIfNotExists(categoryName, categoryTemplate);

            // Now create child channels
            foreach (var channelTemplate in categoryTemplate.Channels)
            {
                var childChannel = await CreateTextChannelFromTemplate(channelTemplate, netCategory);
            }
        }

        await _context.Channel.SendMessageAsync($"{template.ItemType} created");
    }


    /// <summary>
    /// Deletes channels, categories, and roles from a guild based on the specified template configuration.
    /// </summary>
    /// <remarks>This method iterates through the categories, channels, and roles defined in the provided
    /// template and deletes the corresponding entities in the guild. </remarks>
    /// <param name="template">The template configuration that defines the categories, channels, and roles to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteAllFromTemplate(
        TemplateConfiguration template)
    {
        await _context.Channel.SendMessageAsync($"Deleting {template.ItemType}...");

        foreach (var categoryTemplate in template.Categories)
        {
            foreach (var channelTemplate in categoryTemplate.Channels)
            {
                var channelName = Substitutions.DoSubstitutions(channelTemplate.Name);
                var channel = FindTextChannelByName(channelName);

                if (channel != null)
                {
                    await channel.DeleteAsync();
                    await _context.Channel.SendMessageAsync($"Channel {channelName} removed");
                }
            }

            var categoryName = Substitutions.DoSubstitutions(categoryTemplate.Name);
            var category = FindTextChannelByName(categoryName);

            if (category != null)
            {
                await category.DeleteAsync();
                await _context.Channel.SendMessageAsync($"Category {categoryName} removed");
            }
        }

        foreach (var roleTemplate in template.Roles)
        {
            var roleName = Substitutions.DoSubstitutions(roleTemplate.Name);
            var role = FindRoleByName(roleName);

            if (role != null)
            {
                await role.DeleteAsync();
                await _context.Channel.SendMessageAsync($"Role {roleName} removed");

            }
        }

        await _context.Channel.SendMessageAsync($"{template.ItemType} Removed");

    }

    public async Task CreateMessageFromTemplate(
        ITextChannel targetChannel,
        MessageTemplateConfiguration messageTemplate)
    {
        var messageContent = Substitutions.DoSubstitutions(messageTemplate.Content);

        var builder = new ComponentBuilder();

        foreach (var actionRowTemplate in messageTemplate.ActionRowItems)
        {
            ActionRowBuilder rowBuilder = new ActionRowBuilder();
            foreach (var itemTemplate in actionRowTemplate.Items)
            {
                //TODO will redo this one later, since there are other component types out there
                rowBuilder.AddComponent(new ButtonBuilder()
                {
                    CustomId = Substitutions.DoSubstitutions(itemTemplate.CustomIDName),
                    Label = Substitutions.DoSubstitutions(itemTemplate.Label),
                    Style = Enum.Parse<ButtonStyle>(itemTemplate.Style, true)
                });
            }

            builder.AddRow(rowBuilder);

            await _context.Channel.SendMessageAsync(messageContent, components: builder.Build());
        }
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
    /// <param name="template">The template configuration that defines the structure and naming conventions for categories, channels, and roles
    /// to be renamed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task RenameAllFromTemplate(
        TemplateConfiguration template)
    {
        await _context.Channel.SendMessageAsync($"Renaming {template.ItemType}...");

        foreach (var categoryTemplate in template.Categories)
        {
            foreach (var channelTemplate in categoryTemplate.Channels)
            {
                var channelName = Substitutions.DoSubstitutions(channelTemplate.Name);
                var channel = FindTextChannelByName(channelName);
                if (channel != null)
                {
                    await channel.ModifyAsync(x =>
                    {
                        x.Name = channel.Name.Replace(Substitutions["nameLower"], Substitutions["toNameLower"]);
                    });

                    await _context.Channel.SendMessageAsync($"Channel {channelName} updated");
                }
            }

            var categoryName = Substitutions.DoSubstitutions(categoryTemplate.Name);
            var category =  FindTextChannelByName(categoryName);

            if (category != null)
            {
                await category.ModifyAsync(x =>
                {
                    x.Name = category.Name.Replace(Substitutions["nameLower"], Substitutions["toNameLower"]);
                });

                await _context.Channel.SendMessageAsync($"Category {categoryName} updated");
            }
        }

        foreach (var roleTemplate in template.Roles)
        {
            var roleName = Substitutions.DoSubstitutions(roleTemplate.Name);
            var role = FindRoleByName(roleName);

            if (role != null)
            {
                await role.ModifyAsync(x =>
                {
                    x.Name = role.Name.Replace(Substitutions["name"], Substitutions["toName"]);
                });
                await _context.Channel.SendMessageAsync($"Role {roleName} updated");

            }
        }

        await _context.Channel.SendMessageAsync($"{template.ItemType} Renamed");
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
    /// <param name="roles">A list of roles available in the current context, used to resolve role names to role objects.</param>
    /// <returns>A list of <see cref="PermissionOverwriteProperties"/> representing the resolved permission overwrites. Returns
    /// <see langword="null"/> if no valid permission overwrites are generated.</returns>
    private List<Overwrite>? CreatePermissionOverridesFromTemplate(
        List<PermissionOverwriteTemplateConfiguration> permissionTemplates)
    {

        var PermissionOverwrites = new List<Overwrite>();
        foreach (var permission in permissionTemplates)
        {
            var roleName = Substitutions.DoSubstitutions(permission.RoleName);
            var role = FindRoleByName(roleName);
            if (role == null && roleName.Equals("everyone", StringComparison.CurrentCultureIgnoreCase))
            {
                role = _context.Guild.EveryoneRole;
            }

            if (role != null)
            {
                //TODO have to build this like GetPermissionsFromTemplate
                OverwritePermissions permissions = GetOverwritePermissionsFromTemplate(permission); 

                var overwrite = new Overwrite(role.Id, PermissionTarget.Role, permissions);
                
                PermissionOverwrites.Add(overwrite);
            }
            else
            {
                _logger.LogWarning("Role with ID {roleName} not found for permission overwrite", permission.RoleName);
            }
        }

        return PermissionOverwrites;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roleTemplate"></param>
    /// <returns></returns>
    private OverwritePermissions GetOverwritePermissionsFromTemplate(PermissionOverwriteTemplateConfiguration roleTemplate)
    {
        GuildPermission allowed = 0;
        GuildPermission denied = 0;

        foreach (var permission in roleTemplate.Allowed ?? [])
        {
            allowed |= permission;
        }

        foreach (var permission in roleTemplate.Denied ?? [])
        {
            denied |= permission;
        }

        OverwritePermissions perms = new OverwritePermissions((ulong)allowed, (ulong)denied);
        return perms;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roleTemplate"></param>
    /// <returns></returns>
    private List<Overwrite> GetOverwritePermissionsFromTemplate(List<PermissionOverwriteTemplateConfiguration> roleTemplate)
    {
        List<Overwrite> permissionOverwrites = new List<Overwrite>();
        foreach (var permissionTemplate in roleTemplate)
        {

            var perms = GetOverwritePermissionsFromTemplate(permissionTemplate);
            var role = FindRoleByName(permissionTemplate.RoleName);
            if (role != null)
            {
                var overwrite = new Overwrite(role.Id, PermissionTarget.Role, perms);
                permissionOverwrites.Add(overwrite);
            }
        }

        return permissionOverwrites;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roleTemplate"></param>
    /// <returns></returns>
    private GuildPermissions GetPermissionsFromTemplate(RoleTemplateConfiguration roleTemplate)
    {
        GuildPermission permissions = 0;
        if (roleTemplate.Permissions != null && roleTemplate.Permissions.Any())
        {
            foreach (var permission in roleTemplate.Permissions)
            {
                permissions |= permission;
            }
        }
        GuildPermissions perms = new GuildPermissions((uint)permissions);

        return perms;
    }

    /// <summary>
    /// Creates or retrieves a guild channel based on the specified template and applies the provided substitutions.
    /// </summary>
    /// <remarks>This method ensures that a channel matching the specified template and substitutions is
    /// created if it does not already exist. If a matching channel is found in the provided <paramref name="channels"/>
    /// list, it is returned instead of creating a new one.</remarks>
    /// <param name="channelTemplate">The template configuration that defines the properties of the channel to be created.</param>
    /// <param name="parent">The parent channel under which the new channel will be created, or <c>null</c> if no parent is specified.</param>
    /// <returns>The created or existing guild channel that matches the specified template and substitutions.</returns>
    public async Task<ITextChannel> CreateTextChannelFromTemplate(
        ChannelTemplateConfiguration channelTemplate,
        ICategoryChannel parent)
    {
        var categoryName = Substitutions.DoSubstitutions(channelTemplate.Name);
        await _context.Channel.SendMessageAsync($"Creating category {categoryName}");

        var createdCategory = await CreateTextChannelIfNotExists(categoryName, channelTemplate, parent);

        if (channelTemplate.InitialMessages != null)
        {
            foreach (var messageTemplate in channelTemplate.InitialMessages)
            {
                await CreateMessageFromTemplate(createdCategory, messageTemplate);
            }
        }

        return createdCategory;
    }

    /// <summary>
    /// Creates a role if it does not already exist in the guild.
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="properties"></param>
    /// <param name="roles"></param>
    /// <returns></returns>
    private async Task<IRole> CreateRoleIfNotExists(
        RoleTemplateConfiguration roleTemplate)
    {
        var roleName = Substitutions.DoSubstitutions(roleTemplate.Name);

        var role = FindRoleByName(roleName);

        if (role != null) return role;

        var permissions = GetPermissionsFromTemplate(roleTemplate);

        var newRole = await _context.Guild.CreateRoleAsync(roleName, permissions, Discord.Color.Parse(roleTemplate.Color), false, roleTemplate.Mentionable.GetValueOrDefault(true));

        return newRole;
    }

    /// <summary>
    /// Creates a channel if it does not already exist in the guild.
    /// </summary>
    /// <param name="newChannel"></param>
    /// <param name="channels"></param>
    /// <returns></returns>
    private async Task<ITextChannel> CreateTextChannelIfNotExists(
        string name,
        ChannelTemplateConfiguration newChannel,
        ICategoryChannel parent)
    {
        if (FindTextChannelByName(name) == null)
        {
            try
            {
                var createdChannel = await _context.Guild.CreateTextChannelAsync(name, x =>
                {
                    x.CategoryId = parent.Id;
                    x.ChannelType = ChannelType.Text;
                    x.PermissionOverwrites = GetOverwritePermissionsFromTemplate(newChannel.PermissionOverwrites);
                });

                _logger.LogInformation("Created channel: {channelName}", newChannel.Name);

                return createdChannel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating channel: {channelName}", newChannel.Name);
                throw;
            }
        }
        else
        {
            _logger.LogInformation("Channel already exists: {channelName}", newChannel.Name);
            return FindTextChannelByName(name);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="categoryTemplate"></param>
    /// <returns></returns>
    private async Task<ICategoryChannel> CreateCategoryIfNotExists(
        string name,
        CategoryTemplateConfiguration categoryTemplate)
    {
        if (FindCategoryChannelByName(name) == null)
        {
            try
            {
                var createdChannel = await _context.Guild.CreateCategoryChannelAsync(name, x =>
                {
                    x.PermissionOverwrites = CreatePermissionOverridesFromTemplate(categoryTemplate.PermissionOverwrites);
                });

                _logger.LogInformation("Created channel: {channelName}", name);

                return createdChannel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating channel: {channelName}", name);
                throw;
            }
        }
        else
        {
            _logger.LogInformation("Channel already exists: {channelName}", name);
            return FindCategoryChannelByName(name);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private IRole? FindRoleByName(string name)
    {
        var role = _context.Guild.Roles.FirstOrDefault(r => r.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        return role;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private ICategoryChannel? FindCategoryChannelByName(string name)
    {
        var channel = _context.Guild.CategoryChannels.FirstOrDefault(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        return channel;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private ITextChannel? FindTextChannelByName(string name)
    {
        var channel = _context.Guild.TextChannels.FirstOrDefault(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        return channel;
    }
}
