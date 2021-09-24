using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class StoredValueAccountHistoryMap : IEntityTypeConfiguration<StoredValueAccountHistory>
    {
        public void Configure(EntityTypeBuilder<StoredValueAccountHistory> builder)
        {
            builder.ToTable("LPS_StoredValueAccountHistory");

            builder.HasKey(c => c.Id);
            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");
            builder.Property(c => c.Amount).HasColumnType("decimal(24, 8)");
            builder.Property(c => c.LastBalance).HasColumnType("decimal(24, 8)");
            builder.Property(c => c.ThisBalance).HasColumnType("decimal(24, 8)");

        }
    }
}
