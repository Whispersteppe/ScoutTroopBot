using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutBot.Database.DbModels;

namespace ScoutBot.Database.Configurations;

public class GuildUserPropertyConfiguration : IEntityTypeConfiguration<DbGuildUserProperty>
{
    public void Configure(EntityTypeBuilder<DbGuildUserProperty> builder)
    {
        builder.ToTable("GuildUserProperty");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Property)
            .WithMany(x => x.GuildUserProperties)
            .HasForeignKey(x => x.PropertyId)
            .HasConstraintName("FK_GuildUserProperty_Property")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.GuildUser)
            .WithMany(x => x.Properties)
            .HasForeignKey(x => x.PropertyId)
            .HasConstraintName("FK_GuildUserProperty_User")
            .OnDelete(DeleteBehavior.Cascade);
    }
}