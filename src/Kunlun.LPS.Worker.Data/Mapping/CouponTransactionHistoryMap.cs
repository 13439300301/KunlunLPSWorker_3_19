using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class CouponTransactionHistoryMap : IEntityTypeConfiguration<CouponTransactionHistory>
    {
        public void Configure(EntityTypeBuilder<CouponTransactionHistory> builder)
        {
            builder.ToTable("LPS_CouponTransactionHistory");

            builder.HasKey(c => c.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.Transaction).WithMany().HasForeignKey(t => t.TransactionId);

            builder.Property(t => t.FaceValue).HasColumnType("decimal(18,2)");
            builder.Property(t => t.Points).HasColumnType("decimal(18,2)");



        }
    }
}
