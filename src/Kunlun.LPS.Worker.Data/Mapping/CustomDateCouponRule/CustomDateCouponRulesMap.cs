using Kunlun.LPS.Worker.Core.Domain.CustomDateCouponRule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.CustomDateCouponRule
{
    public class CustomDateCouponRulesMap : IEntityTypeConfiguration<CustomDateCouponRules>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CustomDateCouponRules> builder)
        {
            builder.ToTable("LPS_CustomDateCouponRules");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");
        }
    }
}

