using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class CustInfoMap : IEntityTypeConfiguration<CustInfo>
    {
        public void Configure(EntityTypeBuilder<CustInfo> builder)
        {
            builder.ToTable("cust_info");

            builder.HasKey(c => c.Id);

            builder.Property(t => t.Id).HasColumnName("id");
            builder.Property(t => t.ChName).HasColumnName("chname");
            builder.Property(t => t.EngName).HasColumnName("engname");
            builder.Property(t => t.CompanyType).HasColumnName("company_type");
            builder.Property(t => t.Tel).HasColumnName("tel");
            builder.Property(t => t.Industry).HasColumnName("industry");
            builder.Property(t => t.Segment).HasColumnName("segment");
            builder.Property(t => t.Source).HasColumnName("source");
            builder.Property(t => t.Potential).HasColumnName("potential");
            builder.Property(t => t.LostReason).HasColumnName("lost_reason");
            builder.Property(t => t.Email).HasColumnName("email");
            builder.Property(t => t.Fax).HasColumnName("fax");
            builder.Property(t => t.Zip).HasColumnName("zip");
            builder.Property(t => t.Address).HasColumnName("address");
            builder.Property(t => t.Address1).HasColumnName("address1");
            builder.Property(t => t.WebSite).HasColumnName("website");
            builder.Property(t => t.Memo).HasColumnName("memo");
            builder.Property(t => t.CreatDt).HasColumnName("creatdt");
            builder.Property(t => t.Creator).HasColumnName("creator");
            builder.Property(t => t.HotelCode).HasColumnName("hotel_code");
            builder.Property(t => t.IsSent).HasColumnName("is_sent");
            builder.Property(t => t.ProfileType).HasColumnName("profile_type");
            builder.Property(t => t.CustType).HasColumnName("cust_type");
            builder.Property(t => t.SalesCode).HasColumnName("sales_code");
            builder.Property(t => t.DepCode).HasColumnName("dep_code");
            builder.Property(t => t.Department).HasColumnName("department");
            builder.Property(t => t.Status).HasColumnName("status");
            builder.Property(t => t.PmsSalesCode).HasColumnName("pms_sales_code");
            builder.Property(t => t.Faccount).HasColumnName("f_account");
            builder.Property(t => t.Statflag).HasColumnName("stat_flag");
            builder.Property(t => t.Account).HasColumnName("account");
            builder.Property(t => t.Faccountx).HasColumnName("f_account_x");
            builder.Property(t => t.Texext).HasColumnName("tel_ext");
            builder.Property(t => t.corcode).HasColumnName("cor_code");
            builder.Property(t => t.seeaboutdate).HasColumnName("see_about_date");
            builder.Property(t => t.CtsGroupId).HasColumnName("cts_group_id");
            builder.Property(t => t.SourceHotel).HasColumnName("source_hotel");
            builder.Property(t => t.Country).HasColumnName("country");
            builder.Property(t => t.Province).HasColumnName("province");
            builder.Property(t => t.City).HasColumnName("city");
            builder.Property(t => t.RelationCustCata).HasColumnName("relation_cust_cata");
            builder.Property(t => t.Commission).HasColumnName("commission");
            builder.Property(t => t.Territories).HasColumnName("territories");
            builder.Property(t => t.StatusBackup).HasColumnName("status_backup");
            builder.Property(t => t.Extracode1).HasColumnName("extracode_1");
            builder.Property(t => t.Extracode2).HasColumnName("extracode_2");
            builder.Property(t => t.Extracode3).HasColumnName("extracode_3");
            builder.Property(t => t.Extracode4).HasColumnName("extracode_4");
            builder.Property(t => t.Extracode5).HasColumnName("extracode_5");
            builder.Property(t => t.Extracode6).HasColumnName("extracode_6");
            builder.Property(t => t.Extracode7).HasColumnName("extracode_7");
            builder.Property(t => t.Extracode8).HasColumnName("extracode_8");
            builder.Property(t => t.Extracode9).HasColumnName("extracode_9");
            builder.Property(t => t.TaxpayerNumber).HasColumnName("taxpayer_number");
            builder.Property(t => t.MembershipCardNumber).HasColumnName("membership_card_number");
            builder.Property(t => t.MembershipCardId).HasColumnName("membership_card_id");
            builder.Property(t => t.Pwd).HasColumnName("pwd");
            builder.Property(t => t.Cata).HasColumnName("cata");
            builder.Property(t => t.SubCata).HasColumnName("subcata");
            builder.Property(t => t.Freq).HasColumnName("freq");
            builder.Property(t => t.Extracode).HasColumnName("extracode");
            builder.Property(t => t.Prefered).HasColumnName("prefered");
            builder.Property(t => t.Payment).HasColumnName("payment");
            builder.Property(t => t.GrandCode).HasColumnName("grand_code");
        }
    }
}
