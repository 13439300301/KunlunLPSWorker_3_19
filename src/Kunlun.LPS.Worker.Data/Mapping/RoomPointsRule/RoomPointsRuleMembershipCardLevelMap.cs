using Kunlun.LPS.Worker.Core.Domain.RoomPointsRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; 
namespace Kunlun.LPS.Worker.Data.Mapping.RoomPointsRule
{
    public class RoomPointsRuleMembershipCardLevelMap : IEntityTypeConfiguration<RoomPointsRuleMembershipCardLevel>
    { 

        public void Configure(EntityTypeBuilder<RoomPointsRuleMembershipCardLevel> builder)
        {
            builder.ToTable("LPS_RoomPointsRuleCardLevelDetail");

            builder.HasKey(t => t.Id);
            builder.HasOne(t => t.RoomPointsRule).WithMany().HasForeignKey(t => t.RoomPointsRuleId);
            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");



            builder.Property(t => t.FbPoints).HasColumnType("decimal(18,2)");
            builder.Property(t => t.FbRevenue).HasColumnType("decimal(18,2)");
            builder.Property(t => t.OtherPoints).HasColumnType("decimal(18,2)");
            builder.Property(t => t.OtherRevenue).HasColumnType("decimal(18,2)");
            builder.Property(t => t.Points).HasColumnType("decimal(18,2)");
            builder.Property(t => t.Revenue).HasColumnType("decimal(18,2)");
            builder.Property(t => t.RoomPoints).HasColumnType("decimal(18,2)");
            builder.Property(t => t.RoomRevenue).HasColumnType("decimal(18,2)");
            builder.Property(t => t.Tax).HasColumnType("decimal(18,2)");
            builder.Property(t => t.TaxPoints).HasColumnType("decimal(18,2)");





        }
    }
}
