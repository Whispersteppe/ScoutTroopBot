namespace ScoutBot.Database.DbModels;

public class DbUnitMember
{
    public required int Id { get; set; }
    public required int UnitId { get; set; }
    public required int GuildUserId { get; set; }
    public DbUnit Unit { get; set; }
    public DbGuildUser GuildUser { get; set; }

    public List<DbPatrolMember> PatrolMembers { get; set; }
}
