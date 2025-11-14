using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutBot.Database.DbModels;

namespace ScoutBot.Database.Configurations;

public class PatrolConfiguration : IEntityTypeConfiguration<DbPatrol>
{
    public void Configure(EntityTypeBuilder<DbPatrol> builder)
    {
        builder.ToTable("Patrol");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Unit).WithMany(x => x.Patrols).HasForeignKey(x => x.UnitId).HasConstraintName("FK_Patrol_Unit").OnDelete(DeleteBehavior.Cascade);
    }
}
