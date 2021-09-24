using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class PlaceMMap : IEntityTypeConfiguration<PlaceM>
    {
        public void Configure(EntityTypeBuilder<PlaceM> builder)
        {
            builder.ToTable("dic_place_m");

            builder.HasKey(c => c.Code);

            builder.Property(t => t.HotelCode).HasColumnName("hotel_code");
        }
    }
}
