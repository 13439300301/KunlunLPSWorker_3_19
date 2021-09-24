using Kunlun.LPS.Worker.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.Model
{
    public class StoredValueAccountHistoryToSendInfoShown
    {
        public long ProfileId { get; set; }
        public long MembershipCardId { get; set; }

        public string MembershipCardNumber { get; set; }

        public string MembershipCardTypeName { get; set; }
        public string MembershipCardTypeCode { get; set; }
        public string MembershipCardLevelName { get; set; }
        public string MembershipCardLevelCode { get; set; }

        public string ChName { get; set; }

        public string EngName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Title { get; set; }

        public StoredValueAccountHistoryOperationType OperType { get; set; }

        public string OperTypeName { get; set; }

        public string AccrueType { get; set; }

        public string AccrueTypeName { get; set; }

        //本次金额（账户内剩余金额）
        public decimal Balance { get; set; }

        //金额（流水的操作金额）
        public decimal OperValue { get; set; }
        //实际流水的操作金额
        public decimal RealOperValue { get; set; }

        //账户内剩余积分
        public decimal Points { get; set; }

        public DateTime Date { get; set; }

        //交易地点
        public string Place { get; set; }

        //酒店名（暂时默认酒店集团）
        public string ChHotel { get; set; }

        //酒店英文名（暂时默认酒店集团）
        public string EngHote { get; set; }

        //电话号（和注册电话是一个）
        public string MobilePhoneNumber { get; set; }

        public string SmsTitle { get; set; }

        //LPS_MembershipCard
        public DateTime? BindingDate { get; set; }

        //LPS_MembershipCard
        public DateTime? ExpireDate { get; set; }

        public string Email { get; set; }

        public string Operator { get; set; }

        public TransactionType TransactionType { get; set; }

        public string TransactionTypeName { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
