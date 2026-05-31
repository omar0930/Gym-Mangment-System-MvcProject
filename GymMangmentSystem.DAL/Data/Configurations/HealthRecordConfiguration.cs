using GymMangmentSystem.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymMangmentSystem.DAL.Data.Configurations
{
    public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Height).HasPrecision(5, 2);
            builder.Property(x => x.Width).HasPrecision(5, 2);
            builder.Property(x => x.Weight).HasPrecision(5, 2);

            builder.Property(x => x.BloodType).HasConversion<string>().HasMaxLength(15);

            builder.HasOne(x => x.Member)
                .WithOne(m => m.HealthRecord)
                .HasForeignKey<HealthRecord>("MemberId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
