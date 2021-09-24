using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class MembershipCardMap : IEntityTypeConfiguration<MembershipCard>
    {
        public void Configure(EntityTypeBuilder<MembershipCard> builder)
        {
            builder.ToTable("LPS_MembershipCard");

            builder.HasKey(c => c.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");
        }
    }
}
