using Microsoft.EntityFrameworkCore;
using MvcProjectG03.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MvcProjectG03.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasColumnType("varchar(50)").HasMaxLength(50);
            builder.Property(x => x.Description).HasColumnType("varchar(200)").HasMaxLength(200);
            builder.Property(x => x.DurationInDays).IsRequired();
            builder.Property(x => x.Price).HasPrecision(10, 2);
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.ToTable(tb =>
                tb.HasCheckConstraint("DurationDaysCheck", "[DurationInDays] BETWEEN 1 AND 365"));
        }
    }
}