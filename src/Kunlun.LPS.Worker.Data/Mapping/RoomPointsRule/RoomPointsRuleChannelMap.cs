using Kunlun.LPS.Worker.Core.Domain.RoomPointsRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; 

namespace Kunlun.LPS.Worker.Data.Mapping.RoomPointsRule
{
    public class RoomPointsRuleChannelMap : IEntityTypeConfiguration<RoomPointsRuleChannel>
    { 
        public void Configure(EntityTypeBuilder<RoomPointsRuleChannel> builder)
        {
            builder.ToTable("LPS_RoomPointsRule_Channel_Map");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.RoomPointsRule).WithMany().HasForeignKey(t => t.RoomPointsRuleId);  
        }
    }
}
