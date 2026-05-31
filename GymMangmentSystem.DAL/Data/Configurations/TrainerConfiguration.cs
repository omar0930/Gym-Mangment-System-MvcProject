using GymMangmentSystem.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymMangmentSystem.DAL.Data.Configurations
{
    public class TrainerConfiguration : GymUserConfiguration<Trainer>
    {
        public override void Configure(EntityTypeBuilder<Trainer> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.Salary)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Specialty)
                .HasConversion<string>()
                .HasColumnType("varchar(20)");
        }
    }
}
