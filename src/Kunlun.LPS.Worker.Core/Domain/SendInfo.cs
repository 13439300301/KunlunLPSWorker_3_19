using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class SendInfo
    {
        public int Id { get; set; }

        public DateTime? CreateDt { get; set; }

        public string SendType { get; set; }

        public string RecipientName { get; set; }

        public string Addressee { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Attachment { get; set; }

        public string Status { get; set; }

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? LastSendTime { get; set; }

        public int? TryNum { get; set; }

        public string UserCode { get; set; }

        public string HotelCode { get; set; }

        public string CardNo { get; set; }

        public string InsertUser { get; set; }

        public DateTime? InsertDate { get; set; }

        public string UpdateUser { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string ContentFlag { get; set; }

        public string Content2 { get; set; }

        public string RelativePath { get; set; }

        public string DbSource { get; set; }

        public string KeyPairs { get; set; }

        public string ModelNo { get; set; }

        public string Reply { get; set; }

        public string EncryptInfo { get; set; }

        public string Encrypt { get; set; }

        public bool IsDbDataEncrypt { get; set; }

    }
}
