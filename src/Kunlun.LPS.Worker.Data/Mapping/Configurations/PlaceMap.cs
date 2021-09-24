using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class PlaceMap : IEntityTypeConfiguration<Place>
    {
        public void Configure(EntityTypeBuilder<Place> builder)
        {
            builder.ToTable("dic_place");

            builder.HasKey(c => c.Code);

            builder.Property(t => t.Name).HasColumnName("name");
            builder.Property(t => t.MCode).HasColumnName("m_code");
            builder.Property(t => t.PosPlaceCode).HasColumnName("pos_code");
            builder.Property(t => t.Flag).HasColumnName("flag");
        }
    }
}
