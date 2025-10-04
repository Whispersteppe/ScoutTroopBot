using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutTroopBot.EventHandlers;

public class MyMessageCreateHandler(ILogger<MyMessageCreateHandler> logger) : IMessageCreateGatewayHandler
{
    public ValueTask HandleAsync(Message message)
    {
        logger.LogInformation("Scoutbot:MyMessageCreateHandler{}", message.Content);
        return default;
    }
}

public class MessageReactionAddHandler(ILogger<MessageReactionAddHandler> logger, RestClient client) : IMessageReactionAddGatewayHandler
{
    public async ValueTask HandleAsync(MessageReactionAddEventArgs args)
    {
        logger.LogInformation("Scoutbot:MessageReactionAddHandler");

        await client.SendMessageAsync(args.ChannelId, $"<@{args.UserId}> reacted with {args.Emoji.Name}!");
        var dmChannel = await args.User.GetDMChannelAsync();

        await client.SendMessageAsync(dmChannel.Id, $"<@{args.UserId}> reacted with {args.Emoji.Name}!");

    }
}

public class NewUserHandler(ILogger<NewUserHandler> logger, RestClient client) : IGuildUserAddGatewayHandler
{
    public async ValueTask HandleAsync(GuildUser arg)
    {
        logger.LogInformation("Scoutbot:NewUserHandler");

        logger.LogInformation("{nickname} just joined the server", arg.Nickname);

        //todo - query the user, get the appropriate information, and set them up properly
        
    }
}

public class CloseGatewayHandler(ILogger<CloseGatewayHandler> logger) : ICloseGatewayHandler
{

    public ValueTask HandleAsync()
    {
        logger.LogInformation("Scoutbot:Gateway closed");

        return default;
    }
}

public class ConnectGatewayHandler(ILogger<ConnectGatewayHandler> logger) : IConnectGatewayHandler
{
    public ValueTask HandleAsync()
    {
        logger.LogInformation("Scoutbot:Gateway connected");
        return default;
    }
}

public class ConnectingGatewayHandler(ILogger<ConnectingGatewayHandler> logger) : IConnectGatewayHandler
{
    public ValueTask HandleAsync()
    {
        logger.LogInformation("Scoutbot:Gateway connecting");
        return default;
    }
}

public class DisconnectGatewayHandler(ILogger<ConnectingGatewayHandler> logger) : IDisconnectGatewayHandler
{


    public ValueTask HandleAsync(DisconnectEventArgs arg)
    {
        logger.LogInformation("Scoutbot:Gateway disconnect");
        return default;
    }
}