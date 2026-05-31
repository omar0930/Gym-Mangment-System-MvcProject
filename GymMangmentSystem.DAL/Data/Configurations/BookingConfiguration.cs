using GymMangmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymMangmentSystem.DAL.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(x => new { x.MemberId, x.SessionId });

            builder.HasOne(x => x.Member)
                .WithMany(m => m.MemberSessions)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Session)
                .WithMany(s => s.SessionMembers)
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt)
                .HasColumnName("BookingDate")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.IsAttended).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}
