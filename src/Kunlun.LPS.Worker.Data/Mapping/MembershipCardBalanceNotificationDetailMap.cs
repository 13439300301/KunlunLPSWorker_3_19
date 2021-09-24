using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class MembershipCardBalanceNotificationDetailMap : IEntityTypeConfiguration<MembershipCardBalanceNotificationDetail>
    {

        public void Configure(EntityTypeBuilder<MembershipCardBalanceNotificationDetail> builder)
        {
            builder.ToTable("LPS_MembershipCardBalanceNotificationDetail");

            builder.HasKey(m => m.Id);


            builder.Property(m => m.InsertUser).HasColumnName("Insert_User");
            builder.Property(m => m.InsertDate).HasColumnName("Insert_Date");
            builder.Property(m => m.UpdateUser).HasColumnName("Update_User");
            builder.Property(m => m.UpdateDate).HasColumnName("Update_Date");
        }
    }
}
