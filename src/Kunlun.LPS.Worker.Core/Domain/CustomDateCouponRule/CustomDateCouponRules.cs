using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.CustomDateCouponRule
{
    public class CustomDateCouponRules : BaseEntity
    {
        public long Id { get; set; }

        public long MembershipCardTypeId { get; set; }

        public long MembershipCardLevelId { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        /// <summary>
        /// 使用场景(0:会员生日;1:会员日/节日)
        /// </summary>
        public int UseScenario { get; set; }
        /// <summary>
        /// 下发时间选项（0:月控制;1:周控制;2:日控制;3:生日第几天;4:生日前几天;）
        /// </summary>
        public int SendStatus { get; set; }
        /// <summary>
        /// 月控制，每月第几日
        /// </summary>
        public int? MonthControl { get; set; }
        /// <summary>
        /// 周控制
        /// </summary>
        public string WeekControl { get; set; }
        /// <summary>
        /// 日控制，可多选
        /// </summary>
        public string DayControl { get; set; }
        /// <summary>
        /// 生日 前/第 发送天数
        /// </summary>
        public int? DirthdayDay { get; set; }

        public string Description { get; set; }
        //public virtual ICollection<CustomDateCouponRulesMemberSource> MemberSource { get; set; }

        //public virtual ICollection<CustomDateCouponRulesCouponsDetail> CustomDateCouponRulesCouponsDetail { get; set; }
    }
}
