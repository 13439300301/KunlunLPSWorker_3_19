using Kunlun.LPS.Worker.Core.Domain.DivisionCards;
using System.Collections.Generic;

namespace Kunlun.LPS.Worker.Services.DivisionCards
{
    public interface IDivisionCardService
    {
        /// <summary>
        /// 分裂卡
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="membershipCardId"></param>
        void DivisionCard(long? transactionId, long? membershipCardId, List<long> cardIds = null);

        void MergeDivisionCard(List<long> transactionIds);
    }
}
