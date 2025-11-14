using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.DbModels;

public class DbUnit
{
    public int Id { get; set; }
    public required ulong GuildId { get; set; }
    public string Name { get; set; }

    public DbGuild Guild { get; set; }
    public List<DbPatrol> Patrols { get; set; }
    public List<DbUnitMember> UnitMembers { get; set; }
}
