using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{

    /// <summary>
    /// 实体变更消息
    /// </summary>
    [RoutingKey(RoutingKeys.Definition_Modify)]
    public class EntityChangeMessage : BaseMessage
    {
        /// <summary>
        /// 操作类型：INSERT、UPDATE、DELETE
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 主键 Id 或 code
        /// </summary>
        public string Identity { get; set; }
    }

    /// <summary>
    /// 实体变更操作类型
    /// </summary>
    public static class EntityChangeType
    {
        public const string INSERT = nameof(INSERT);
        public const string UPDATE = nameof(UPDATE);
        public const string DELETE = nameof(DELETE);
    }

}
