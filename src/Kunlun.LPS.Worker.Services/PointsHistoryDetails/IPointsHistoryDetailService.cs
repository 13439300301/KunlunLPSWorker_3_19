using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.PointsHistoryDetails
{
    public interface IPointsHistoryDetailService
    {
        void InsertPointsHistoryDetail(long pointsAccountHistoryId, decimal points);

        void PointsExchangeCoupon(long transactionId);

        void CancelPointsExchangeCoupon(long transactionId);

        void MergePoints(List<long> transactionIds);
    }
}