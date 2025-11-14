using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.DbModels;

public class DbGuild
{
    public required ulong DiscordGuildID { get; set; }
    public string GuildName { get; set; }

    public List<DbGuildUser> Users { get; set; }
    public List<DbUnit> Units { get; set; }
}
