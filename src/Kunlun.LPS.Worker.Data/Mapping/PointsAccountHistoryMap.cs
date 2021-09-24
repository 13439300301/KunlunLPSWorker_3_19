using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class PointsAccountHistoryMap : IEntityTypeConfiguration<PointsAccountHistory>
    {
        public void Configure(EntityTypeBuilder<PointsAccountHistory> builder)
        {
            builder.ToTable("LPS_PointsAccountHistory");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.LastBalance).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.Points).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.ThisBalance).HasColumnType("decimal(18, 2)");

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.Transaction).WithMany().HasForeignKey(t => t.TransactionId);
            builder.HasOne(t => t.MembershipCardTransaction).WithMany().HasForeignKey(t => t.MembershipCardTransactionId);
        }
    }
}
