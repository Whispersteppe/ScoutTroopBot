using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.DtoModels;

public class DtoUnit
{
    public int Id { get; set; }
    public required ulong GuildId { get; set; }
    public string Name { get; set; }

    public DtoGuild Guild { get; set; }
    public List<DtoPatrol> Patrols { get; set; }
    public List<DtoUnitMember> UnitMembers { get; set; }

    public List<DtoPatrolMember> PatrolMembers { get; set; }

}
