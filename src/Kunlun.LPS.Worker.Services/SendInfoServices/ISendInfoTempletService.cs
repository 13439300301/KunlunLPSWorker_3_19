using Kunlun.LPS.Worker.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    public interface ISendInfoTempletService
    {

        string GetSendInfoContent(string content, MembershipCard membershipCard);

        string GetSendInfoContent(string content, Profile profile, bool custId = false);

        string GetSendInfoContent(string content, PointsAccountHistory pointsAccountHistory, string languageCode);

        string GetSendInfoContent(string content, Transaction storedValueAccountHistory, string languageCode);

        string GetSendInfoContent(string content, MembershipCardBalanceNotification membershipCardBalanceNotification);

        public string GetSendInfoContent(string content, MembershipCard membershipCard, decimal balance);

        string GetSendInfoContent(string content, Transaction storedValueAccountHistory, string languageCode, bool isPoints);
        
        string GetSendInfoContent(string content, List<CouponTransactionHistory> list, DateTime date, string languageCode);

        string GetSendInfoContent(string content, decimal? balance, decimal? points, DateTime? enrollDate, DateTime? expireDate, DateTime? updateDate);

        string GetSendInfoContent(string content, string membershipCardNumber, string cardFaceNumber, decimal? ExpiredPoints = null, int? NextMonth = null, int? NextMonths = null, int? ExpiredYear = null);
    }
}
