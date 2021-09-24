using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class CouponInventoryMap : IEntityTypeConfiguration<CouponInventory>
    {
        public void Configure(EntityTypeBuilder<CouponInventory> builder)
        {
            builder.ToTable("LPS_CouponInventory");

            builder.HasKey(c => c.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.CouponType).WithMany().HasForeignKey(t => t.CouponTypeId);
        }
    }
}
