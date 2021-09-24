using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    public class MembershipCardLevelMap : IEntityTypeConfiguration<MembershipCardLevel>
    {
        public void Configure(EntityTypeBuilder<MembershipCardLevel> builder)
        {
            builder.ToTable("LPS_MembershipCardLevel");

            builder.HasKey(l => l.Id);

            builder.Property(t => t.DefaultBalance).HasColumnType("decimal(18,2)");
        }
    }
}
