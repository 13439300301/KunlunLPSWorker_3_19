using Kunlun.LPS.Worker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Data.Mapping
{
    class SendInfoMap: IEntityTypeConfiguration<SendInfo>
    {
        public void Configure(EntityTypeBuilder<SendInfo> builder)
        {
            builder.ToTable("send_info");

            builder.HasKey(c => c.Id);

            builder.Property(p => p.CreateDt).HasColumnName("create_dt");
            builder.Property(p => p.SendType).HasColumnName("send_type");
            builder.Property(p => p.RecipientName).HasColumnName("recipient_name");
            builder.Property(p => p.Addressee).HasColumnName("addressee");
            builder.Property(p => p.Title).HasColumnName("title");
            builder.Property(p => p.Content).HasColumnName("content");
            builder.Property(p => p.Attachment).HasColumnName("attachment");
            builder.Property(p => p.Status).HasColumnName("status");
            builder.Property(p => p.BeginTime).HasColumnName("begin_time");
            builder.Property(p => p.EndTime).HasColumnName("end_time");
            builder.Property(p => p.LastSendTime).HasColumnName("last_send_time");
            builder.Property(p => p.TryNum).HasColumnName("try_num");
            builder.Property(p => p.UserCode).HasColumnName("user_code");
            builder.Property(p => p.HotelCode).HasColumnName("hotel_code");
            builder.Property(p => p.CardNo).HasColumnName("card_no");
            builder.Property(p => p.InsertUser).HasColumnName("insert_user");
            builder.Property(p => p.InsertDate).HasColumnName("insert_date");
            builder.Property(p => p.UpdateUser).HasColumnName("update_user");
            builder.Property(p => p.UpdateDate).HasColumnName("update_date");
            builder.Property(p => p.ContentFlag).HasColumnName("content_flag");
            builder.Property(p => p.Content2).HasColumnName("content2");
            builder.Property(p => p.RelativePath).HasColumnName("relative_path");
            builder.Property(p => p.DbSource).HasColumnName("db_source");
            builder.Property(p => p.KeyPairs).HasColumnName("key_pairs");
            builder.Property(p => p.ModelNo).HasColumnName("model_no");
            builder.Property(p => p.Reply).HasColumnName("reply");
            builder.Property(p => p.EncryptInfo).HasColumnName("encrypt_info");
            builder.Property(p => p.Encrypt).HasColumnName("encrypt");
            builder.Property(p => p.IsDbDataEncrypt).HasColumnName("IsDbDataEncrypt");
        }
    }
}
