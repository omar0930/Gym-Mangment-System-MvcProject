using GymMangmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymMangmentSystem.DAL.Configurations
{
    public class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Name)
                .HasColumnType("varchar(100)")
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .HasColumnType("varchar(100)")
                .HasMaxLength(100);

            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Phone).IsUnique();

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("EmailCheck", "Email LIKE '%@%'");
                tb.HasCheckConstraint("PhoneCheck", "Phone LIKE '01%' AND LEN(Phone) = 11");
            });

            builder.OwnsOne(x => x.Address, address =>
            {
                address.Property(x => x.Street)
                    .HasColumnType("varchar")
                    .HasMaxLength(100)
                    .HasColumnName("Street");
                address.Property(x => x.City).HasColumnName("City").HasColumnType("varchar")
                    .HasMaxLength(100);
            });

        }
    }
}
