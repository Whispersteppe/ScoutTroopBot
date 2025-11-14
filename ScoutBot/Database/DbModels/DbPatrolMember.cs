namespace ScoutBot.Database.DbModels;

public class DbPatrolMember
{
    public required int Id { get; set; }
    public required int PatrolId { get; set; }
    public required int UnitMemberId { get; set; }

    public DbPatrol Patrol { get; set; }
    public DbUnitMember UnitMember { get; set; }
}
