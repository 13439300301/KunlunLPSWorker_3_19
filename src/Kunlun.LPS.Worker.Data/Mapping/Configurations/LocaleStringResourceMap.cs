using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class LocaleStringResourceMap : IEntityTypeConfiguration<LocaleStringResource>
    {
        public void Configure(EntityTypeBuilder<LocaleStringResource> builder)
        {
            builder.ToTable("LocaleStringResource");

            builder.HasKey(p => p.Id);
            builder.Property(t => t.Code).HasColumnName("ResourceName");
            builder.Property(t => t.Name).HasColumnName("ResourceValue");
        }
    }
}
