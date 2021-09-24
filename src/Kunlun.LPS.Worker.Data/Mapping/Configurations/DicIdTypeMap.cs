using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class DicIdTypeMap : IEntityTypeConfiguration<DicIdType>
    {
        public void Configure(EntityTypeBuilder<DicIdType> builder)
        {
            builder.ToTable("dic_idtype");

            builder.HasKey(c => c.Code);

            builder.Property(t => t.Name).HasColumnName("name");
            builder.Property(t => t.SortId).HasColumnName("sort_id");
        }
    }
}
