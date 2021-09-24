using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class PointsValidPeriodAdvanceSendInfoMap : IEntityTypeConfiguration<PointsValidPeriodAdvanceSendInfo>
    {
        public void Configure(EntityTypeBuilder<PointsValidPeriodAdvanceSendInfo> builder)
        {
            builder.ToTable("LPS_PointsValidPeriodAdvanceSendInfo");

            builder.HasKey(t => t.Id);

            builder.Ignore(t => t.Code);
            builder.Ignore(t => t.Name);
        }
    }
}
