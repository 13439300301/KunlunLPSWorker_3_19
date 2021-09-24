using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class DicHistoryTypeMap : IEntityTypeConfiguration<DicHistoryType>
    {
        public void Configure(EntityTypeBuilder<DicHistoryType> builder)
        {
            builder.ToTable("dic_history_type");

            builder.HasKey(c => c.Code);

            builder.Property(t => t.Name).HasColumnName("name");
            builder.Property(t => t.SortId).HasColumnName("sort_id");
        }
    }
}
