using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class ProfileMobilePhoneNumberDetail : BaseEntity
    {
        // <summary>
        /// 证件类型Id
        /// </summary> 
        public long Id { get; set; }

        /// <summary>
        /// 档案Id
        /// </summary> 
        public long ProfileId { get; set; }

        /// <summary>
        /// 手机号国家代码
        /// </summary> 
        public string MobilePhoneCountryNumber { get; set; }

        /// <summary>
        /// 手机号
        /// </summary> 
        public string MobilePhoneNumber { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary> 
        public bool IsDefault { get; set; }

        public virtual Profile Profile { get; set; }

        public string Custom { get; set; }
        public string profileInfoClassIfy { get; set; } 
    }
}
