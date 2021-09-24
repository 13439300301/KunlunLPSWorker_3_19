using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class ConsumeHistoryMap : IEntityTypeConfiguration<ConsumeHistory>
    {
        public void Configure(EntityTypeBuilder<ConsumeHistory> builder)
        {
            builder.ToTable("LPS_ConsumeHistory");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Growth).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.Mileages).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.Points).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.RM_FbRevenue).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.RM_OtherRevenue).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.RM_RoomRate).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.RM_RoomRevenue).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.SubTotalAmount).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.SurCharges).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.Tax).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.TotalAmount).HasColumnType("decimal(18, 2)");

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");
            builder.Ignore(t => t.ImportId);
            builder.Ignore(t => t.FFPCode);
            builder.Ignore(t => t.FFPNumber);
        }
    }
}
