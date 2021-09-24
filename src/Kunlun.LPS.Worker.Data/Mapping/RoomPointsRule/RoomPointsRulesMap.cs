using Kunlun.LPS.Worker.Core.Domain.RoomPointsRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; 

namespace Kunlun.LPS.Worker.Data.Mapping.RoomPointsRule
{
    public class RoomPointsRulesMap : IEntityTypeConfiguration<RoomPointsRules>
    {
        public RoomPointsRulesMap()
        {
           

        }

        public void Configure(EntityTypeBuilder<RoomPointsRules> builder)
        {
            builder.ToTable("LPS_RoomPointsRule");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");


            builder.Property(t => t.Limit).HasColumnType("decimal(18,2)");

            //builder.HasMany(t => t.MembershipCardLevel).WithRequired(t => t.RoomPointsRule).HasForeignKey(t => t.RoomPointsRuleId);

            //builder.HasMany(t => t.RoomPointsRuleChannel).WithRequired(t => t.RoomPointsRule).HasForeignKey(t => t.RoomPointsRuleId);

            //builder.HasMany(t => t.RoomPointsRuleHotel).WithRequired(t => t.RoomPointsRule).HasForeignKey(t => t.RoomPointsRuleId);

            //builder.HasMany(t => t.RoomPointsRulePayment).WithRequired(t => t.RoomPointsRule).HasForeignKey(t => t.RoomPointsRuleId);

            //builder.HasMany(t => t.RoomPointsRuleMarket).WithRequired(t => t.RoomPointsRule).HasForeignKey(t => t.RoomPointsRuleId);

            //builder.HasMany(t => t.RoomPointsRuleRateTemplate).WithRequired(t => t.RoomPointsRule).HasForeignKey(t => t.RoomPointsRuleId);

            //builder.HasMany(t => t.RoomPointsRuleHotelRoomType).WithRequired(t => t.RoomPointsRule).HasForeignKey(t => t.RoomPointsRuleId);

        }
    }
}
