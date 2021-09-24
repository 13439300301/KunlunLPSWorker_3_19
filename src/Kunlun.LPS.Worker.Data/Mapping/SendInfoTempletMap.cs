using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class SendInfoTempletMap : IEntityTypeConfiguration<SendInfoTemplet>
    {
        public void Configure(EntityTypeBuilder<SendInfoTemplet> builder)
        {
            builder.ToTable("email_templet");

            builder.HasKey(c => c.Id);

            builder.Property(t => t.CreateDate).HasColumnName("create_date");
            builder.Property(t => t.TempletType).HasColumnName("templet_type");
            builder.Property(t => t.UsageCode).HasColumnName("usage_code");
            builder.Property(t => t.MessageSupplierCode).HasColumnName("message_supplier_code");
            builder.Property(t => t.MessageChannel).HasColumnName("message_channel");
            builder.Property(t => t.MembershipType).HasColumnName("membership_type");

            builder.Property(p => p.InsertUser).HasColumnName("insert_user");
            builder.Property(p => p.InsertDate).HasColumnName("insert_date");
            builder.Property(p => p.UpdateUser).HasColumnName("update_user");
            builder.Property(p => p.UpdateDate).HasColumnName("update_date");
        }
    }
}
