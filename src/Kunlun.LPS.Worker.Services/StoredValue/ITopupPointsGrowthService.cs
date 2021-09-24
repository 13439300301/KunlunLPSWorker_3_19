using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System.Collections.Generic;

namespace Kunlun.LPS.Worker.Services.StoredValue
{
    public interface ITopupPointsGrowthService
    {
        public void TopupPointsGrow(TopupMessage topupPointsGrowth);
    }
}
