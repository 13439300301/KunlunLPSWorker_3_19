using Kunlun.LPS.Worker.Core.Domain.RoomPointsRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Data.Mapping.RoomPointsRule
{
    public class RoomPointsRuleHotelRoomTypeMap : IEntityTypeConfiguration<RoomPointsRuleHotelRoomType>
    {  
        public void Configure(EntityTypeBuilder<RoomPointsRuleHotelRoomType> builder)
        {
            builder.ToTable("LPS_RoomPointsRule_Hotel_RoomType_Map");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.RoomPointsRule).WithMany().HasForeignKey(t => t.RoomPointsRuleId);
        }
    }
}
