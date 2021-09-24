using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data
{
    class RechargeActivityRuleCouponsDetailMap : IEntityTypeConfiguration<RechargeActivityRuleCouponsDetail>
    {
        public void Configure(EntityTypeBuilder<RechargeActivityRuleCouponsDetail> builder)
        {
            builder.ToTable("LPS_RechargeActivityRule_Coupons_Detail");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Quantity).HasColumnType("int");
            builder.Property(c => c.StartAmount).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.EndAmount).HasColumnType("decimal(18, 2)");

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");


            //builder.Ignore(t => t.RechargeActivityRuleId);
            //builder.Ignore(t => t.CouponTypeId);
        }
    }
}
