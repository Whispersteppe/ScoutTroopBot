using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutBot.Database.DbModels;

namespace ScoutBot.Database.Configurations;

public class GuildUserConfiguration : IEntityTypeConfiguration<DbGuildUser>
{
    public void Configure(EntityTypeBuilder<DbGuildUser> builder)
    {
        builder.ToTable("GuildUser");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User).WithMany(x => x.GuildUsers).HasForeignKey(x => x.DiscordUserId).HasConstraintName("FK_GuildUser_User").OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Guild).WithMany(x => x.Users).HasForeignKey(x => x.DiscordUserId).HasConstraintName("FK_GuildUser_Guild").OnDelete(DeleteBehavior.Cascade);
    }
}
