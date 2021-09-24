using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.Accounts
{
    public interface IAccountService
    {
        List<Account> InitAccount(long membershipCardId, MembershipCardAccountType? membershipCardAccountType = null, List<long> membershipCardAccountIds = null, decimal? initCardFee = null, MembershipCard membershipCard = null);

        List<RequestAccount> RequestAccounts(long membershipCardId, MembershipCardAccountType? membershipCardAccountType = null, List<long> membershipCardAccountIds = null, decimal? initCardFee = null);
    }
}
