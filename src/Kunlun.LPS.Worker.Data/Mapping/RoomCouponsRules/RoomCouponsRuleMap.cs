using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.RoomCouponRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.RoomCouponsRules
{
    public class RoomCouponsRuleMap : IEntityTypeConfiguration<RoomCouponsRule>
    {
        public void Configure(EntityTypeBuilder<RoomCouponsRule> builder)
        {
            builder.ToTable("LPS_RoomCouponsRule");

            builder.HasKey(t => t.Id);

            builder.HasMany(t => t.RoomCouponsRuleChannel).WithOne(t => t.RoomCouponsRule).HasForeignKey(t => t.RoomCouponsRuleId);
            builder.HasMany(t => t.RoomCouponsRuleCouponsDetail).WithOne(t => t.RoomCouponsRule).HasForeignKey(t => t.RoomCouponsRuleId);
            builder.HasMany(t => t.RoomCouponsRuleMarket).WithOne(t => t.RoomCouponsRule).HasForeignKey(t => t.RoomCouponsRuleId);
            builder.HasMany(t => t.RoomCouponsRuleMemberSource).WithOne(t => t.RoomCouponsRule).HasForeignKey(t => t.RoomCouponsRuleId);
            builder.HasMany(t => t.RoomCouponsRulePayment).WithOne(t => t.RoomCouponsRule).HasForeignKey(t => t.RoomCouponsRuleId);
            builder.HasMany(t => t.RoomCouponsRuleRate).WithOne(t => t.RoomCouponsRule).HasForeignKey(t => t.RoomCouponsRuleId);


            builder.Property(t => t.Revenue).HasColumnType("decimal(18,2)");

        }
    }
}
