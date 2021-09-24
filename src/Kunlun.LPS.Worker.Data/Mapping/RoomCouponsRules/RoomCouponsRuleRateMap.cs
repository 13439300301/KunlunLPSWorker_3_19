using Kunlun.LPS.Worker.Core.Domain.RoomCouponRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.RoomCouponsRules
{
    public class RoomCouponsRuleRateMap : IEntityTypeConfiguration<RoomCouponsRuleRate>
    {
        public void Configure(EntityTypeBuilder<RoomCouponsRuleRate> builder)
        {
            builder.ToTable("LPS_RoomCouponsRule_Rate_Map");

            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.RoomCouponsRule).WithMany().HasForeignKey(t => t.RoomCouponsRuleId);
        }
    }
}
