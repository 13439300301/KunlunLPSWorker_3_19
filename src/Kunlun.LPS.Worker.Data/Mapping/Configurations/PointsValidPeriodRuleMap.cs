using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class PointsValidPeriodRuleMap : IEntityTypeConfiguration<PointsValidPeriodRule>
    {
        public void Configure(EntityTypeBuilder<PointsValidPeriodRule> builder)
        {
            builder.ToTable("LPS_PointsValidPeriodRule");

            builder.HasKey(c => c.Id);
            builder.Ignore(t => t.Code);
            builder.Ignore(t => t.Name);
        }
    }
}