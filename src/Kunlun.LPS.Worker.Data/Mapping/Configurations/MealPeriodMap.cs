using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class MealPeriodMap : IEntityTypeConfiguration<MealPeriod>
    {
        public void Configure(EntityTypeBuilder<MealPeriod> builder)
        {
            builder.ToTable("LPS_MealPeriod");

            builder.HasKey(c => c.Id);
        }
    }
}
