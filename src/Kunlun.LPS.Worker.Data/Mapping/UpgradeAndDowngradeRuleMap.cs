﻿using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    public class UpgradeAndDowngradeRuleMap : IEntityTypeConfiguration<UpgradeAndDowngradeRule>
    {
        public void Configure(EntityTypeBuilder<UpgradeAndDowngradeRule> builder)
        {
            builder.ToTable("LPS_UpgradeAndDowngradeRule");

            builder.HasKey(c => c.Id); 

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");
        }
    }
}
