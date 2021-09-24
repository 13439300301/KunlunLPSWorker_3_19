using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class ProfileExternalMembershipMap : IEntityTypeConfiguration<ProfileExternalMembership>
    {
        public void Configure(EntityTypeBuilder<ProfileExternalMembership> builder)
        {
            builder.ToTable("LPS_ProfileExternalMembership");

            builder.HasKey(c => c.Id);

            builder.Property(t => t.InsertUser).HasColumnName("Insert_User");
            builder.Property(t => t.InsertDate).HasColumnName("Insert_Date");
            builder.Property(t => t.UpdateUser).HasColumnName("Update_User");
            builder.Property(t => t.UpdateDate).HasColumnName("Update_Date");


            builder.Property(t => t.Mileage).HasColumnType("decimal(18,2)");
        }
    }
}
