using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.DbModels;

public class DbUser
{
    public required ulong DiscordUserId { get; set; }
    public string Name { get; set; }

    public List<DbGuildUser> GuildUsers { get; set; }
}
