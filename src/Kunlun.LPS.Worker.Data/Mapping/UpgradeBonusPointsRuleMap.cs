﻿using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    public class UpgradeBonusPointsRuleMap : IEntityTypeConfiguration<UpgradeBonusPointsRule>
    {
        public void Configure(EntityTypeBuilder<UpgradeBonusPointsRule> builder)
        {
            builder.ToTable("LPS_UpgradeBonusPointsRule");

            builder.HasKey(c => c.Id);



            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");
        }
    }
}
