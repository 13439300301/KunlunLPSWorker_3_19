using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    public class CouponTypePaymentWay_MapMap : IEntityTypeConfiguration<CouponTypePaymentWay_Map>
    {
        public void Configure(EntityTypeBuilder<CouponTypePaymentWay_Map> builder)
        {
            builder.ToTable("LPS_CouponType_PaymentWay_Map");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.CouponType).WithMany().HasForeignKey(t => t.CouponTypeId);
        }
    }
}
