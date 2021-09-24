using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services
{
    public interface  IMembershipCardLevelChangeService
    {
        void UpgradeGiftPoint(long membershipCardId,long cardLevelId, MembershipCardLevelChangeType direction);
        void RechargeAmountUpgrade(StoredValueMessageBase storedValueMessageBase);
    }
}
