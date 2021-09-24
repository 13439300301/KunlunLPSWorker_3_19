using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class ProfileIdDetail: BaseEntity
    {

        /// <summary>
        /// Id
        /// </summary>
        [Display(Name = "证件类型Id")]
        public long Id { get; set; }

        /// <summary>
        /// 会员档案 Id
        /// </summary>
        [Display(Name = "档案Id")]
        public long ProfileId { get; set; }

        /// <summary>
        /// 证件 Code
        /// </summary>
        [Display(Name = "证件Code")]
        public string IdTypeCode { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        [Display(Name = "证件号")]
        public string IdNumber { get; set; }

        /// <summary>
        /// 证件到期时间
        /// </summary>
        [Display(Name = "证件到期时间")]
        public DateTime? IdExpireDate { get; set; }

        /// <summary>
        /// 是否默认，1：默认，0不默认
        /// </summary>
        [Display(Name = "是否默认")]
        public bool IsDefault { get; set; }
    }
}
