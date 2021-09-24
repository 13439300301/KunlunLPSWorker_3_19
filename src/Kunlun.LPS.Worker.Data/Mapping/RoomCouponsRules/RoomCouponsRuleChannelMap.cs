using Kunlun.LPS.Worker.Core.Domain.RoomCouponRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.RoomCouponsRules
{
    public class RoomCouponsRuleChannelMap : IEntityTypeConfiguration<RoomCouponsRuleChannel>
    {
        public void Configure(EntityTypeBuilder<RoomCouponsRuleChannel> builder)
        {
            builder.ToTable("LPS_RoomCouponsRule_Channel_Map");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.RoomCouponsRule).WithMany().HasForeignKey(t => t.RoomCouponsRuleId);
        }
    }
}
