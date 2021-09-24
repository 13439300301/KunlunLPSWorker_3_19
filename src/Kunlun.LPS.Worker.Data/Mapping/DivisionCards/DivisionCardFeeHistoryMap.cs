using Kunlun.LPS.Worker.Core.Domain.DivisionCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.DivisionCards
{
    class DivisionCardFeeHistoryMap : IEntityTypeConfiguration<DivisionCardFeeHistory>
    {
        public void Configure(EntityTypeBuilder<DivisionCardFeeHistory> builder)
        {
            builder.ToTable("LPS_DivisionCardFeeHistory");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Amount).HasColumnType("decimal(24, 8)");
            builder.Property(c => c.LastBalance).HasColumnType("decimal(24, 8)");
            builder.Property(c => c.ThisBalance).HasColumnType("decimal(24, 8)");
            builder.Property(c => c.LastOverdraftBalance).HasColumnType("decimal(24, 8)");
            builder.Property(c => c.ThisOverdraftBalance).HasColumnType("decimal(24, 8)");

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");
        }
    }
}
