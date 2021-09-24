using Kunlun.LPS.Worker.Core.Domain.RoomCouponRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.RoomCouponsRules
{
    public class RoomCouponsRuleMemberSourceMap : IEntityTypeConfiguration<RoomCouponsRuleMemberSource>
    {
        public void Configure(EntityTypeBuilder<RoomCouponsRuleMemberSource> builder)
        {
            builder.ToTable("LPS_RoomCouponsRule_MemberSource_Map");

            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.RoomCouponsRule).WithMany().HasForeignKey(t => t.RoomCouponsRuleId);
        }
    }
}
