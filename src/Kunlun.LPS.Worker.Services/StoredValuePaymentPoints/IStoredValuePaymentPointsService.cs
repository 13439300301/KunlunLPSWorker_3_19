using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.StoredValuePaymentPoints
{
    public interface IStoredValuePaymentPointsService
    {
        void StoredValuePaymentPoints(StoredValueMessageBase storedValueMessageBase);

        void StoredValueCancelPaymentPoints(StoredValueMessageBase storedValueMessageBase);
    }
}
