using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    public class RechargeAmountUpgradeRuleDetailMap : IEntityTypeConfiguration<RechargeAmountUpgradeRuleDetail>
    {
        public void Configure(EntityTypeBuilder<RechargeAmountUpgradeRuleDetail> builder)
        {
            builder.ToTable("LPS_RechargeAmountUpgradeRuleDetail");

            builder.HasKey(c => c.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");
            builder.HasOne(t => t.RechargeAmountUpgradeRule).WithMany().HasForeignKey(t => t.RechargeAmountUpgradeRuleId);



            builder.Property(t => t.MaxLimitAmount).HasColumnType("decimal(18,2)");
            builder.Property(t => t.MinimumLimitAmount).HasColumnType("decimal(18,2)");



        }
    }
}
