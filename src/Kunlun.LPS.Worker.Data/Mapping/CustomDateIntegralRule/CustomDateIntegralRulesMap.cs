using Kunlun.LPS.Worker.Core.Domain.CustomDateIntegralRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.CustomDateIntegralRule
{
    public class CustomDateIntegralRulesMap : IEntityTypeConfiguration<CustomDateIntegralRules>
    {
        public void Configure(EntityTypeBuilder<CustomDateIntegralRules> builder)
        {
            builder.ToTable("LPS_CustomDateIntegralRules");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            // builder.HasMany(t => t.MemberSource).WithOne(t => t.CustomDateIntegralRules).HasForeignKey(t => t.CustomDateIntegralRulesId);
        }
    }
}
