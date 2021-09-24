using Kunlun.LPS.Worker.Core.MessageQueue.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    [RoutingKey(RoutingKeys.Points_BatchAdjust)]
    public class PointsValueBatchAdjustMessage : PointsValueMessageBase
    {
        /// <summary>
        /// 积分用途，true是使用积分通知 false是获得积分通知，通过调整类型去判断
        /// </summary>
        public bool Purposes { get; set; }
    }
}
