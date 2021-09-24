using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class LPSLog : BaseEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 请求序号，便于找到同一次操作中所有相关的日志
        /// </summary>
        public Guid? RequestId { get; set; }

        /// <summary>
        /// 操作类型枚举：待整理
        /// </summary>
        public string OperationType { get; set; }

        /// <summary>
        /// 操作的数据的
        /// </summary>
        public string OperationMainDataId { get; set; }

        /// <summary>
        /// 操作的数据类型枚举：待整理
        /// </summary>
        public string OperationDataType { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; } 
    }
}
