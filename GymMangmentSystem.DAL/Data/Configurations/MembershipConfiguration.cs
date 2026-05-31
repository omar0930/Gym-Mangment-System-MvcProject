using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymMangmentSystem.DAL.Data.Configurations
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.HasKey(x => new { x.MemberId, x.PlanId });

            builder.HasOne(x => x.Member)
                .WithMany(m => m.Memberships)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Plan)
                .WithMany(p => p.Memberships)
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt)
                .HasColumnName("StartDate")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.EndDate).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}
