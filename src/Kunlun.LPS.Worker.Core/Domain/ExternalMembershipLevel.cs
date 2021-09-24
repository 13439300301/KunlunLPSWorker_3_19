using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class ExternalMembershipLevel : BaseEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 外部会员级别代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 外部会员级别名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 外部会员提供商 Id
        /// </summary>
        public long ExternalMembershipProviderId { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int Seq { get; set; } 
    }
}
