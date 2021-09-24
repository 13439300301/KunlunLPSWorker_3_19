using Kunlun.LPS.Worker.Core.Domain.Configurations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class Profile : BaseEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        [Display(Name = "会员ID")]
        public long Id { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        [Display(Name = "名")]
        public string FirstName { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        [Display(Name = "姓")]
        public string LastName { get; set; }

        /// <summary>
        /// 全名
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 备选名
        /// </summary>
        [Display(Name = "英文名")]
        public string AltFirstName { get; set; }

        /// <summary>
        /// 备选姓
        /// </summary>
        [Display(Name = "英文姓")]
        public string AltLastName { get; set; }

        /// <summary>
        /// 性别代码
        /// </summary>
        [Display(Name = "性别")]
        public string GenderCode { get; set; }

        /// <summary>
        /// 称呼代码
        /// </summary>
        [Display(Name = "称呼")]
        public string TitleCode { get; set; }

        /// <summary>
        /// 首选语言代码
        /// </summary>
        [Display(Name = "首选语言")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// 国籍代码
        /// </summary>
        [Display(Name = "国籍")]
        public string NationalityCode { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [Display(Name = "生日")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 首选证件类型代码
        /// </summary>
        [Display(Name = "证件类型")]
        public string IdTypeCode { get; set; }

        /// <summary>
        /// 首选证件号
        /// </summary>
        [Display(Name = "证件号")]
        public string IdNumber { get; set; }

        /// <summary>
        /// 首选证件到期时间
        /// </summary>
        [Display(Name = "证件到期日期")]
        public DateTime? IdExpireDate { get; set; }

        /// <summary>
        /// 手机号国家代码
        /// </summary>
        [Display(Name = "手机号国家代码")]
        public string MobilePhoneCountryNumber { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        public string MobilePhoneNumber { get; set; }

        /// <summary>
        /// 手机号是否通过验证
        /// </summary>
        [Display(Name = "手机号是否通过验证")]
        public bool? IsMobilePhoneValidated { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// Email 是否通过验证
        /// </summary>
        [Display(Name = "Email是否通过验证")]
        public bool? IsEmailValidated { get; set; }

        /// <summary>
        /// 邮寄地址的国家代码
        /// </summary>
        [Display(Name = "邮寄地址国家")]
        public string AddressCountryCode { get; set; }

        /// <summary>
        /// 邮寄地址的省份代码
        /// </summary>
        [Display(Name = "邮寄地址省份")]
        public string AddressProvinceCode { get; set; }

        /// <summary>
        /// 邮寄地址的城市代码
        /// </summary>
        [Display(Name = "邮寄地址城市")]
        public string AddressCityCode { get; set; }

        /// <summary>
        /// 邮寄地址的城市名称
        /// </summary>
        [Display(Name = "邮寄地址城市(自定义)")]
        public string AddressCityName { get; set; }

        /// <summary>
        /// 邮寄地址的城区
        /// </summary>
        [Display(Name = "邮寄地址城区名")]
        public string AddressDistrict { get; set; }

        /// <summary>
        /// 邮寄地址的街道、门牌号地址
        /// </summary>
        [Display(Name = "邮寄地址街道，门牌号")]
        public string AddressStreet { get; set; }

        /// <summary>
        /// 邮寄地址的邮政编码
        /// </summary>
        [Display(Name = "邮寄地址的邮政编码")]
        public string AddressPostcode { get; set; }

        /// <summary>
        /// 档案建立时间
        /// </summary>
        [Display(Name = "档案建立日期")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 会员登陆网站使用的密码
        /// </summary>
        [Display(Name = "会员登录网站使用的密码")]
        public string Password { get; set; }

        /// <summary>
        /// 密码的 Salt（暂时无用）
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// 介绍人会员卡 Id
        /// </summary>
        [Display(Name = "介绍人会员卡号")]
        public long? IntroducerMembershipCardId { get; set; }

        /// <summary>
        /// 是否计算过介绍人积分
        /// </summary>
        [Display(Name = "介绍人会员卡积分")]
        public bool? IsIntroducerPointsCalculated { get; set; }

        /// <summary>
        /// 是否已合并
        /// </summary>
        [Display(Name = "是否合并")]
        public bool IsMerged { get; set; }

        /// <summary>
        /// 客户 Id
        /// </summary>
        public int? CustId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 是否记名
        /// </summary>
        public bool IsAnonymous { get; set; }

        /// <summary>
        /// 数据是否加密
        /// </summary>
        public bool IsDbDataEncrypt { get; set; }

        /// <summary>
        /// 是否为预制档案
        /// </summary>
        public bool IsPreCreated { get; set; }

        /// <summary>
        /// 会员所属酒店
        /// </summary>
        public string HotelCode { get; set; }

        /// <summary>
        /// 是否禁用通知
        /// </summary>
        [Display(Name = "禁用通知")]
        public bool DisableNotification { get; set; }

        [Display(Name = "性别")]
        public virtual Gender Gender { get; set; }

        [Display(Name = "称呼")]
        public virtual Title Title { get; set; }

        [Display(Name = "首选语言")]
        public virtual ECRSLanguage Language { get; set; }

        [Display(Name = "是否积订房人积分")]
        public bool CanGetBookerPoints { get; set; }

        [Display(Name = "结婚纪念日")]
        public DateTime? WeddingDate { get; set; }

        public virtual ICollection<MembershipCard> MembershipCards { get; set; }

        public bool? IsRegisterRewardPointsCalculated { get; set; }

    }
}
