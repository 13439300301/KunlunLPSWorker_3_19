using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{

    class ProfileIdDetailMap : IEntityTypeConfiguration<ProfileIdDetail>
    {
        public void Configure(EntityTypeBuilder<ProfileIdDetail> builder)
        {
            builder.ToTable("LPS_ProfileIdentityDetail");

            builder.HasKey(c => c.Id);

            builder.Property(p => p.InsertUser).HasColumnName("insert_user");
            builder.Property(p => p.InsertDate).HasColumnName("insert_date");
            builder.Property(p => p.UpdateUser).HasColumnName("update_user");
            builder.Property(p => p.UpdateDate).HasColumnName("update_date");
        }
    }
}
