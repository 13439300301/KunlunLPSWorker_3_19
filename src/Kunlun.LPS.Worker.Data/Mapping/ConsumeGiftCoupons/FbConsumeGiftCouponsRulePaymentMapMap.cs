using Kunlun.LPS.Worker.Core.Domain.ConsumeGiftCoupons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.ConsumeGiftCoupons
{
    public class FbConsumeGiftCouponsRulePaymentMapMap : IEntityTypeConfiguration<FbConsumeGiftCouponsRulePaymentMap>
    {
        public void Configure(EntityTypeBuilder<FbConsumeGiftCouponsRulePaymentMap> builder)
        {
            builder.ToTable("LPS_FbConsumeGiftCouponsRule_Payment_Map");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.FbConsumeGiftCouponsRule).WithMany(t => t.FbConsumeGiftCouponsRulePaymentMaps).HasForeignKey(t => t.FbConsumeGiftCouponsRuleId);
        }
    }
}
