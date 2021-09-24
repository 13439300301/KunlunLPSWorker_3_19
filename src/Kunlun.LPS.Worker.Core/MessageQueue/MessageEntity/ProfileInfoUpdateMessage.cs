using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Profile_Update)]
    public class ProfileInfoUpdateMessage : BaseMessage
    {
        /// <summary>
        /// 会员Id
        /// </summary>
        public long ProfileId { get; set; }

        /// <summary>
        /// 中文名 lastname+firstname(LPS_Profile)
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 拼音名
        /// </summary>
        public string AltFirstName { get; set; }

        /// <summary>
        /// 拼音姓
        /// </summary>
        public string AltLastName { get; set; }

        /// <summary>
        /// 会员名 firstname(lps_profile)
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 会员姓 lastname(lps_profile)
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 称呼 Name（Dic_Title)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 会员生日 Birthday(LPS_Profile)
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string MobilePhoneNumber { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string GenderCode { get; set; }

        /// <summary>
        /// ID类型
        /// </summary>
        public string IdTypeCode { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 档案建立日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 卡级别Id
        /// </summary>
        public long CardLevelId { get; set; }

        /// <summary>
        /// 卡级别code
        /// </summary>
        public string CardLevelCode { get; set; }

        /// <summary>
        /// 卡级别名
        /// </summary>
        public string CardLevelName { get; set; }

        /// <summary>
        /// 会员卡ID
        /// </summary>
        public long MembershipCardId { get; set; }

        /// <summary>
        /// 会员卡号
        /// </summary>
        public string MembershipCardNumber { get; set; }

        /// <summary>
        /// 升降级 upgrade升级/downgrade
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// 源级别code
        /// </summary>
        public string OriginalLevelCode { get; set; }

        /// <summary>
        /// 源级别name
        /// </summary>
        public string OriginalLevelName { get; set; }
    }
}
