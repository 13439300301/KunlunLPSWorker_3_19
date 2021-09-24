using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class SysparamMap : IEntityTypeConfiguration<Sysparam>
    {
        public void Configure(EntityTypeBuilder<Sysparam> builder)
        {
            builder.ToTable("sysparam");

            builder.HasKey(c => new { c.Code, c.HotelCode });
            builder.Ignore(c => c.Name);
            builder.Property(t => t.ParValue).HasColumnName("par_value");
            builder.Property(t => t.HotelCode).HasColumnName("hotel_code");
            builder.Property(t => t.ControlType).HasColumnName("control_type");
            builder.Property(t => t.ControlSql).HasColumnName("control_sql");
        }
    }
}
