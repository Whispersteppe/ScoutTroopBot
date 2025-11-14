using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutBot.Database.DbModels;

namespace ScoutBot.Database.Configurations;

public class GuildConfiguration : IEntityTypeConfiguration<DbGuild>
{
    public void Configure(EntityTypeBuilder<DbGuild> builder)
    {
        builder.ToTable("Guild");
        builder.HasKey(x => x.DiscordGuildID);
        builder.Property(x => x.DiscordGuildID).ValueGeneratedNever();

//        builder.HasMany(x => x.Users).WithOne(x => x.Guild).HasForeignKey(x => x.DiscordGuildId);
//        builder.HasMany(x => x.Units).WithOne(x => x.Guild).HasForeignKey(x => x.GuildId);

    }
}
