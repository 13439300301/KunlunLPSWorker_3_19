using Kunlun.LPS.Worker.Core.Domain.ConsumeGiftCoupons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.ConsumeGiftCoupons
{
    public class FbConsumeGiftCouponsRulePlaceMapMap : IEntityTypeConfiguration<FbConsumeGiftCouponsRulePlaceMap>
    {
        public void Configure(EntityTypeBuilder<FbConsumeGiftCouponsRulePlaceMap> builder)
        {
            builder.ToTable("LPS_FbConsumeGiftCouponsRule_Place_Map");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");

            builder.HasOne(t => t.FbConsumeGiftCouponsRule).WithMany(t => t.FbConsumeGiftCouponsRulePlaceMaps).HasForeignKey(t => t.FbConsumeGiftCouponsRuleId);
        }
    }
}
