using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping.Configurations
{
    public class HotelMap : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.ToTable("hotels");

            builder.HasKey(c => c.Code);

            builder.Property(t => t.Name).HasColumnName("name");
            builder.Property(t => t.Flag).HasColumnName("flag");
            builder.Property(t => t.Des).HasColumnName("des");
            builder.Property(t => t.Remark).HasColumnName("remark");
            builder.Property(t => t.Seq).HasColumnName("sort_id");
            builder.Property(t => t.EngName).HasColumnName("eng_name");
            builder.Property(t => t.CityCode).HasColumnName("city_code");
            builder.Property(t => t.Keyword).HasColumnName("keyword");
            builder.Property(t => t.CreatedOn).HasColumnName("dt");
            builder.Property(t => t.Stars).HasColumnName("stars");
            builder.Property(t => t.Address).HasColumnName("address");
            builder.Property(t => t.ProvinceCode).HasColumnName("province_code");
            builder.Property(t => t.CountryCode).HasColumnName("country_code");
            builder.Property(t => t.HotelGroupCode).HasColumnName("group_code");
            builder.Property(t => t.TotalRoom).HasColumnName("total_room");
            builder.Property(t => t.FullName).HasColumnName("full_name");
            builder.Property(t => t.EngAddress).HasColumnName("address_en");
            builder.Property(t => t.TotalArea).HasColumnName("total_area");
            builder.Property(t => t.TotalMeetingArea).HasColumnName("total_meeting_area");
            builder.Property(t => t.TotalFbArea).HasColumnName("total_fb_area");
            builder.Property(t => t.UniteHotels).HasColumnName("unite_hotels");
            builder.Property(t => t.DbName).HasColumnName("dbname");
            builder.Property(t => t.PostCode).HasColumnName("post_code");
            builder.Property(t => t.Fax).HasColumnName("fax");
            builder.Property(t => t.Tel).HasColumnName("tel");
            builder.Property(t => t.Udf1).HasColumnName("udf_1");
            builder.Property(t => t.Udf2).HasColumnName("udf_2");
            builder.Property(t => t.Email).HasColumnName("email");
            builder.Property(t => t.Mobile).HasColumnName("mobile");
            builder.Property(t => t.Currency).HasColumnName("currency");
            builder.Property(t => t.HotelOperationType).HasColumnName("hotel_operation_type");
            builder.Property(t => t.InternalCode).HasColumnName("internal_code");
            builder.Property(t => t.TimeZone).HasColumnName("time_zone");
        }
    }
}
