using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutBot.Database.DbModels;

namespace ScoutBot.Database.Configurations;

public class PatrolMemberConfiguration : IEntityTypeConfiguration<DbPatrolMember>
{
    public void Configure(EntityTypeBuilder<DbPatrolMember> builder)
    {
        builder.ToTable("PatrolMember");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Patrol)
            .WithMany(x => x.PatrolMembers)
            .HasForeignKey(x => x.PatrolId)
            .HasConstraintName("FK_PatrolMember_Patrol")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.UnitMember)
            .WithMany(x => x.PatrolMembers)
            .HasForeignKey(x => x.UnitMemberId)
            .HasConstraintName("FK_PatrolMember_UnitMember")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
