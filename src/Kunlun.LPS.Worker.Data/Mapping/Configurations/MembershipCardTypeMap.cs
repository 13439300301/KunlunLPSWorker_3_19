using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class MembershipCardTypeMap : IEntityTypeConfiguration<MembershipCardType>
    {
        public void Configure(EntityTypeBuilder<MembershipCardType> builder)
        {
            builder.ToTable("LPS_MembershipCardType");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.LimitBalance).HasColumnType("decimal(24, 8)");
            builder.Property(c => c.RealNameLimitBalance).HasColumnType("decimal(24, 8)");
        }
    }
}
