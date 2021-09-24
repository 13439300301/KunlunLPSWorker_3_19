using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using System;
using System.Collections.Generic;

namespace Kunlun.LPS.Worker.Services.Points
{
    public interface IPointsService
    {
        void GiftPoints(RegisterPointsMessage message, MembershipCard membershipCard, DateTime date, decimal points);

        void ExpirePoints();

        List<MembershipCard> GetSharedMembershipCard(long cardId = 0);

        string[] OperationPoints(long cardId = 0, decimal value = 0);

        long? GetSharedPointsAccountId(long cardId = 0);
        void FirstStayGiftPoints();
    }
}
