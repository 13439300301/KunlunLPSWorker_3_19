using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class CouponTypeMap : IEntityTypeConfiguration<CouponType>
    {
        public void Configure(EntityTypeBuilder<CouponType> builder)
        {
            builder.ToTable("LPS_CouponType");

            builder.HasKey(c => c.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");


            builder.Property(t => t.DiscountRate).HasColumnType("decimal(18,2)");
            builder.Property(t => t.ExchangeNeedPoints).HasColumnType("decimal(18,2)");
            builder.Property(t => t.ExternalPoints).HasColumnType("decimal(18,2)");
            builder.Property(t => t.FaceValue).HasColumnType("decimal(18,2)");
            builder.Property(t => t.MinimumAmount).HasColumnType("decimal(18,2)");








        }
    }
}
