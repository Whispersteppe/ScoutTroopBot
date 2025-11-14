using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutBot.Database.DbModels;

namespace ScoutBot.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<DbUser>
{
    public void Configure(EntityTypeBuilder<DbUser> builder)
    {
        builder.ToTable("User");
        builder.HasKey(x => x.DiscordUserId);
        builder.Property(x => x.DiscordUserId).ValueGeneratedNever();
    }
}