using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class MembershipCardTransactionMap : IEntityTypeConfiguration<MembershipCardTransaction>
    {
        public void Configure(EntityTypeBuilder<MembershipCardTransaction> builder)
        {
            builder.ToTable("LPS_MembershipCardTransaction");

            builder.HasKey(t => t.Id);

            builder.Property(c => c.Amount).HasColumnType("decimal(24, 8)");
            builder.Property(c => c.LastBalance).HasColumnType("decimal(24, 8)");
            builder.Property(c => c.Points).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.RealAmount).HasColumnType("decimal(24, 8)");
            builder.Property(c => c.ThisBalance).HasColumnType("decimal(24, 8)");
            builder.Property(c => c.Points).HasColumnType("decimal(24, 8)");

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.Transaction).WithMany(t => t.MembershipCardTransactions).HasForeignKey(t => t.TransactionId);
            builder.HasOne(t => t.MembershipCard).WithMany(t => t.MembershipCardTransactions).HasForeignKey(t => t.MembershipCardId);
        }
    }
}
