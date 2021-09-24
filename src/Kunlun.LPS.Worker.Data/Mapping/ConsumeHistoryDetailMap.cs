using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class ConsumeHistoryDetailMap : IEntityTypeConfiguration<ConsumeHistoryDetail>
    {
        public void Configure(EntityTypeBuilder<ConsumeHistoryDetail> builder)
        {
            builder.ToTable("LPS_ConsumeHistoryDetail");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.UnitPrice).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.Quantity).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.Amount).HasColumnType("decimal(18, 2)");
            builder.Property(c => c.Tax).HasColumnType("decimal(18, 2)");

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");
        }
    }
}
