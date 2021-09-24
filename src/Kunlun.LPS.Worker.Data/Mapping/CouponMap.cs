using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class CouponMap : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("LPS_Coupon");

            builder.HasKey(c => c.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.Property(t => t.DiscountRate).HasColumnType("decimal(18,2)");
            builder.Property(t => t.FaceValue).HasColumnType("decimal(18,2)");
            builder.Property(t => t.Points).HasColumnType("decimal(18,2)");
            builder.Property(t => t.UnitPrice).HasColumnType("decimal(18,2)");



        }
    }
}
