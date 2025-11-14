using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScoutBot.Database.DtoModels;

public class DtoPatrolMember
{
    public required int Id { get; set; }
    public required int PatrolId { get; set; }
    public required int UnitMemberId { get; set; }

    public DtoPatrol Patrol { get; set; }
    public DtoUnitMember UnitMember { get; set; }
}
