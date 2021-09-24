using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class MembershipCardLevelChangeHistoryMap : IEntityTypeConfiguration<MembershipCardLevelChangeHistory>
    {
        public void Configure(EntityTypeBuilder<MembershipCardAccount> builder)
        {
            builder.ToTable("LPS_MembershipCardAccount");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CreditLine).HasColumnType("decimal(18, 2)");
        }

        public void Configure(EntityTypeBuilder<MembershipCardLevelChangeHistory> builder)
        {
            builder.ToTable("LPS_MembershipCardLevelChangeHistory");

            builder.HasKey(c => c.Id);

            builder.Property(p => p.InsertUser).HasColumnName("insert_user");
            builder.Property(p => p.InsertDate).HasColumnName("insert_date");
            builder.Property(p => p.UpdateUser).HasColumnName("update_user");
            builder.Property(p => p.UpdateDate).HasColumnName("update_date");
        }
    }
}
