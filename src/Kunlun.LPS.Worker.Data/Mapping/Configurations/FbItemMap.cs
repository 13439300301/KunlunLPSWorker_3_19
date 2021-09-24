using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class FbItemMap : IEntityTypeConfiguration<FbItem>
    {
        public void Configure(EntityTypeBuilder<FbItem> builder)
        {
            builder.ToTable("dic_fb_item");

            builder.HasKey(c => c.Code);

            builder.Property(t => t.Name).HasColumnName("name1");
            builder.Property(t => t.Price).HasColumnName("price").HasColumnType("decimal(18,2)");
            builder.Property(t => t.HotelCode).HasColumnName("hotel_code");
            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");
        }
    }
}
