using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.ProfileRegister)]
    public class RegisterInfo : BaseMessage
    {
        /// <summary>
        /// 卡号 (LPS_MembershipCard)
        /// </summary>
        public string MembershipCardNumber { get; set; }

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
        /// 卡类型 Name
        /// </summary>
        public string MembershipCardName { get; set; }

        /// <summary>
        /// 卡类型 Code
        /// </summary>
        public string MembershipCardCode { get; set; }

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
        /// 绑定日期 BindingDate(LPS_MembershipCard)
        /// </summary>
        public DateTime BindingDate { get; set; }

        /// <summary>
        /// 到期日期 ExpirationDate(LPS_MembershipCard)
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// 证件号 IdNumber(LPS_Profile)
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 邮寄地址城市 AddressCityName（LPS_Profile)
        /// </summary>
        public string AddressCityName { get; set; }

        /// <summary>
        /// 卡级别 Name
        /// </summary>
        public string CardLevelName { get; set; }

        /// <summary>
        /// 卡级别 Code
        /// </summary>
        public string CardLevelCode { get; set; }

        /// <summary>
        /// 国籍 Name（DIC_Country)
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// 证件类型 Name(DIC_IdType)
        /// </summary>
        public string IdType { get; set; }

        /// <summary>
        /// 来源 Name(DIC_Member_Source)
        /// </summary>
        //public string MemberSource { get; set; }

        /// <summary>
        /// 余额 Value(Lps_Account)
        /// </summary>
        //public decimal Value { get; set; }

        /// <summary>
        /// 酒店名
        /// </summary>
        //public string ChHotel { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPwd { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string AddressPostcode { get; set; }
    }
}
