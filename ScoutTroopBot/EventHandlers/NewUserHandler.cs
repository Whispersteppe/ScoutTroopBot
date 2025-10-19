using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace ScoutTroopBot.EventHandlers;

public class NewUserHandler(ILogger<NewUserHandler> logger, RestClient client) : IGuildUserAddGatewayHandler
{
    public async ValueTask HandleAsync(GuildUser arg)
    {
        logger.LogInformation("Scoutbot:NewUserHandler");

        logger.LogInformation("{nickname} just joined the server", arg.Nickname);

        var roles = await client.GetGuildRolesAsync(arg.GuildId);
        var unverifiedRole = roles.FirstOrDefault(x => x.Name.Equals("Unverified", StringComparison.CurrentCultureIgnoreCase));
        if (unverifiedRole != null)
        {
            //await client.AddGuildUserRoleAsync(arg.GuildId, arg.Id, unverifiedRole.Id);
            await arg.AddRoleAsync(unverifiedRole.Id);
        }

    }
}
