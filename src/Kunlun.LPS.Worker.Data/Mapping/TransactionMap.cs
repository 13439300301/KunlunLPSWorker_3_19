using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class TransactionMap : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("LPS_Transaction");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Amount).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.Fee).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.Points).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.RealAmount).HasColumnType("decimal(18, 2)");

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasMany(t => t.MembershipCardTransactions).WithOne(t => t.Transaction).HasForeignKey(t => t.TransactionId);
        }
    }
}
