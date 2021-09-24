using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class MembershipCardAccountMap : IEntityTypeConfiguration<MembershipCardAccount>
    {
        public void Configure(EntityTypeBuilder<MembershipCardAccount> builder)
        {
            builder.ToTable("LPS_MembershipCardAccount");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CreditLine).HasColumnType("decimal(18, 2)");
        }
    }
}
