using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScoutBot.Database.DtoModels;

public class DtoGuildUserProperty
{
    public required int Id { get; set; }
    public required int GuildUserId { get; set; }
    public required int PropertyId { get; set; }
    public string Value { get; set; }

    public DtoProperty Property { get; set; }
    public DtoGuildUser GuildUser { get; set; }
}
