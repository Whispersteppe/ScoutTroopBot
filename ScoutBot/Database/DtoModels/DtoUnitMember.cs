using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScoutBot.Database.DtoModels;

public class DtoUnitMember
{
    public required int Id { get; set; }
    public required int UnitId { get; set; }
    public required int GuildUserId { get; set; }
    public DtoUnit Unit { get; set; }
    public DtoGuildUser GuildUser { get; set; }

    public List<DtoPatrolMember> PatrolMembers { get; set; }
}
