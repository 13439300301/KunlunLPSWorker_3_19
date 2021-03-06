using Kunlun.LPS.Worker.Core.Domain.CustomDateCouponRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.CustomDateCouponRule
{
    public class CustomDateCouponRulesCouponsDetailMap : IEntityTypeConfiguration<CustomDateCouponRulesCouponsDetail>
    {
        public void Configure(EntityTypeBuilder<CustomDateCouponRulesCouponsDetail> builder)
        {
            builder.ToTable("LPS_CustomDateCouponRules_Coupons_Detail");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.CustomDateCouponRules).WithMany().HasForeignKey(t => t.CustomDateCouponRulesId);
        }
    }
}

