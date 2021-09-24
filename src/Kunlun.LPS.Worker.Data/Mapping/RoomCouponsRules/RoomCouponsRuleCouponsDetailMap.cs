using Kunlun.LPS.Worker.Core.Domain.RoomCouponRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.RoomCouponsRules
{
    public class RoomCouponsRuleCouponsDetailMap : IEntityTypeConfiguration<RoomCouponsRuleCouponsDetail>
    {
        public void Configure(EntityTypeBuilder<RoomCouponsRuleCouponsDetail> builder)
        {
            builder.ToTable("LPS_RoomCouponsRule_Coupons_Detail");
            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.RoomCouponsRule).WithMany().HasForeignKey(t => t.RoomCouponsRuleId);
        }
    }
}
