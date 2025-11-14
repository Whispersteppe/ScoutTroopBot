using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutBot.Database.DbModels;

namespace ScoutBot.Database.Configurations;

public class UnitMemberConfiguration : IEntityTypeConfiguration<DbUnitMember>
{
    public void Configure(EntityTypeBuilder<DbUnitMember> builder)
    {
        builder.ToTable("UnitMember");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Unit).WithMany(x => x.UnitMembers).HasForeignKey(x => x.UnitId).HasConstraintName("FK_UnitMember_Unit").OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.GuildUser).WithMany(x => x.UnitMembers).HasForeignKey(x => x.GuildUserId).HasConstraintName("FK_UnitMember_GuildUser").OnDelete(DeleteBehavior.Cascade);

    }
}
