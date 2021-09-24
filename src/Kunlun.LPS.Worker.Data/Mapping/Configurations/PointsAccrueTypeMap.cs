using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class PointsAccrueTypeMap : IEntityTypeConfiguration<PointsAccrueType>
    {
        public void Configure(EntityTypeBuilder<PointsAccrueType> builder)
        {
            builder.ToTable("LPS_PointsAccrueType");

            builder.HasKey(c => c.Id);
        }
    }
}
