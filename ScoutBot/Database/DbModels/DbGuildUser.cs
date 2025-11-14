using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.DbModels;

public class DbGuildUser
{
    public required int Id { get; set; }
    public required ulong DiscordUserId { get; set; }
    public required ulong DiscordGuildId { get; set; }
    public string UserName { get; set; }

    public List<DbGuildUserProperty> Properties { get; set; }

    public DbUser User { get; set; }
    public DbGuild Guild { get; set; }
    public List<DbUnitMember> UnitMembers { get; set; }

}
