using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class DicProvinceMap : IEntityTypeConfiguration<DicProvince>
    {
        public void Configure(EntityTypeBuilder<DicProvince> builder)
        {
            builder.ToTable("dic_province");

            builder.HasKey(c => c.Code);

            builder.Property(t => t.Name).HasColumnName("name");
            builder.Property(t => t.SortId).HasColumnName("sort_id");
            builder.Property(t => t.CountryCode).HasColumnName("country_code");
        }
    }
}
