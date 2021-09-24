using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    public class MembershipCardBalanceNotificationMap: IEntityTypeConfiguration<MembershipCardBalanceNotification>
    {

        public void Configure(EntityTypeBuilder<MembershipCardBalanceNotification> builder)
        {
            builder.ToTable("LPS_MembershipCardBalanceNotification");

            builder.HasKey(m => m.Id);

            builder.Property(m=>m.Balance).HasColumnType("decimal(18, 2)");

            builder.Property(m => m.InsertUser).HasColumnName("Insert_User");
            builder.Property(m => m.InsertDate).HasColumnName("Insert_Date");
            builder.Property(m => m.UpdateUser).HasColumnName("Update_User");
            builder.Property(m => m.UpdateDate).HasColumnName("Update_Date");
        }
    }
}
