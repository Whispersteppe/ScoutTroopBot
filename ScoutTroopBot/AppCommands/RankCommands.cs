using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutTroopBot.AppCommands;

public class RankCommands(ILogger<RankCommands> logger, RestClient client, Interaction interaction) : ApplicationCommandModule<ApplicationCommandContext>
{

    [SlashCommand("pong", "Pong!")]
    public static string Pong() => "Ping!";



    [UserCommand("ID")]
    public static string Id(User user) => user.Id.ToString();

    [MessageCommand("Timestamp")]
    public static string Timestamp(RestMessage message) => message.CreatedAt.ToString();


    [UserCommand("rank")]
    public string ChangeRank(User user)
    {
        var name = user.Username;
        logger.LogInformation("name is {name}", name);
        return "path to eagle";
    }

}
