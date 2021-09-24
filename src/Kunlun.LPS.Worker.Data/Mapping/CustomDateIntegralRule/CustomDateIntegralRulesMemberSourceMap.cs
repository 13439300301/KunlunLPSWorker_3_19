using Kunlun.LPS.Worker.Core.Domain.CustomDateIntegralRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.CustomDateIntegralRule
{
    public class CustomDateIntegralRulesMemberSourceMap : IEntityTypeConfiguration<CustomDateIntegralRuleMemberSource>
    {
        public void Configure(EntityTypeBuilder<CustomDateIntegralRuleMemberSource> builder)
        {
            builder.ToTable("LPS_CustomDateIntegralRules_MemberSource_Map");


            builder.HasKey(t => t.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.CustomDateIntegralRules).WithMany().HasForeignKey(t => t.CustomDateIntegralRulesId);
        }
    }
}
