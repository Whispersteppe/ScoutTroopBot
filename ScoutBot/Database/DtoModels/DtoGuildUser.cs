using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database.DtoModels;

public class DtoGuildUser
{
    public required int Id { get; set; }
    public required ulong DiscordUserId { get; set; }
    public required ulong DiscordGuildId { get; set; }
    public string UserName { get; set; }

    public List<DtoGuildUserProperty> Properties { get; set; }

    public DtoUser User { get; set; }
    public DtoGuild Guild { get; set; }
    public List<DtoUnitMember> UnitMembers { get; set; }

}
