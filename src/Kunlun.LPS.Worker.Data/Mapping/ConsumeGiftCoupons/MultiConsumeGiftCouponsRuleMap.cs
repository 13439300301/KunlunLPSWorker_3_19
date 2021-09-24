using Kunlun.LPS.Worker.Core.Domain.ConsumeGiftCoupons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.ConsumeGiftCoupons
{
    public class MultiConsumeGiftCouponsRuleMap : IEntityTypeConfiguration<MultiConsumeGiftCouponsRule>
    {
        public void Configure(EntityTypeBuilder<MultiConsumeGiftCouponsRule> builder)
        {
            builder.ToTable("LPS_MultiConsumeGiftCouponsRule");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Revenue).HasColumnType("decimal(18, 2)");

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");
        }
    }
}
