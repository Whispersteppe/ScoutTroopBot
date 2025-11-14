namespace ScoutBot.Database.DbModels;

public class DbGuildUserProperty
{
    public required int Id { get; set; }
    public required int GuildUserId { get; set; }
    public required int PropertyId { get; set; }
    public string Value { get; set; }

    public DbProperty Property { get; set; }
    public DbGuildUser GuildUser { get; set; }
}
