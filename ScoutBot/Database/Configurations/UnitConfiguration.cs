using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutBot.Database.DbModels;

namespace ScoutBot.Database.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<DbUnit>
{
    public void Configure(EntityTypeBuilder<DbUnit> builder)
    {
        builder.ToTable("Unit");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Guild)
            .WithMany(x => x.Units)
            .HasForeignKey(x => x.GuildId)
            .HasConstraintName("FK_Unit_Guild")
            .OnDelete(DeleteBehavior.Cascade);
    }
}