using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class DicSourceSvcMap : IEntityTypeConfiguration<DicSourceSvc>
    {
        public void Configure(EntityTypeBuilder<DicSourceSvc> builder)
        {
            builder.ToTable("dic_source_svc");

            builder.HasKey(c => c.Code);

            builder.Property(t => t.Name).HasColumnName("name");
            builder.Property(t => t.SortId).HasColumnName("sort_id");
            builder.Property(t => t.PosId).HasColumnName("pos_id");
            builder.Property(t => t.HotelCode).HasColumnName("hotel_code");
        }
    }
}
