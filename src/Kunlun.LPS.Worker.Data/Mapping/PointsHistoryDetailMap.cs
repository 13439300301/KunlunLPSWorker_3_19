using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class PointsHistoryDetailMap : IEntityTypeConfiguration<PointsHistoryDetail>
    {
        public void Configure(EntityTypeBuilder<PointsHistoryDetail> builder)
        {
            builder.ToTable("LPS_PointsHistoryDetail");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Points).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.RemainingPoints).HasColumnType("decimal(18, 2)");

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.PointsAccountHistory).WithMany().HasForeignKey(t => t.PointsAccountHistoryId);
        }
    }
}