using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScoutBot.Database.DbModels;

namespace ScoutBot.Database.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<DbProperty>
{
    public void Configure(EntityTypeBuilder<DbProperty> builder)
    {
        builder.ToTable("Property");
        builder.HasKey(x => x.Id);
    }
}