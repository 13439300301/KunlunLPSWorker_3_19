namespace Kunlun.LPS.Worker.Services.StoredValue
{
    /// <summary>
    /// 从redis获取信息的服务
    /// </summary>
    public interface IGetOrUpdateInfoFromRedisService
    {
        bool IsExistAuthByApprovalCode(long cardId, string approvalCode);

        /// <summary>
        /// 变更储值账户余额，增减皆可（会员卡不存在透支账户的情况下）
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        string[] ChangeStoredValueAccountBalance(long cardId, string jsonStr);

        /// <summary>
        /// 增加储值账户余额，按账户（会员卡存在透支账户的情况下）
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        string[] AddStoredValueAccountBalanceExistOverdraft(long cardId, string jsonStr);

        /// <summary>
        /// 按账户扣减账户余额
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        string[] DeductAccountBalance(long cardId, string jsonStr);

        /// <summary>
        /// 获取redis中的账户信息
        /// </summary>
        /// <param name="cardId">卡Id</param>
        /// <param name="membershipCardAccountId">账户类型Id</param>
        /// <param name="accountType">账户类型（储值，积分，成长值）</param>
        string[] GetRedisAccountInfo(long cardId, long membershipCardAccountId, string accountType);

        string[] GetRedisAccountHash(string key, string fieid);

        /// <summary>
        /// 获取卡的透支余额和透支额度
        /// </summary>
        /// <param name="cardId">卡id</param>
        /// <returns></returns>
        string[] GetRedisOverdraft(long cardId);

        /// <summary>
        /// 获取Redis的卡总余额
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        string[] GetRedisCardTotalAmount(long cardId, string accountType);

        bool UpdateRedisAccountBalance(long cardId, long membershipCardAccountId, decimal accountValue, string accountType);

        /// <summary>
        /// 更新本金和透支账户类型对应的账户id
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="membershipCardAccountId"></param>
        /// <param name="storedValueAccountType">固定值“OverdraftAccount”或“PrincipalAccount”</param>
        /// <returns></returns>
        bool UpdateRedisStoredValueAccountType(long cardId, long membershipCardAccountId, string storedValueAccountType);

        bool UpdateRedisOverdraftAccountDefineAmount(long cardId, long membershipCardAccountId, decimal overdraftDefineAmount);

        string[] CancelAuthorization(long currentCardId, long? mainCardId, string approvalCode);

        string[] AppendAuthorization(long currentCardId, long? mainCardId, string approvalCode, decimal amount);

        string[] CreateAuthorization(long currentCardId, long? mainCardId, decimal amount);

        string[] CalculateAndUpdatePointAccountBalance(long cardId, long membershipCardAccountId, decimal transactionPoint);

        string[] CalculateAndUpdateGrowthAccountBalance(long cardId, long membershipCardAccountId, decimal transactionGrowth);

        string[] DeductCardFee(long cardId, string jsonStr, decimal transactionAmount, string approvalCode = "");

        /// <summary>
        /// 获取卡预授权总金额
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        string[] GetCardAuthTotalAmount(long cardId);

        /// <summary>
        /// 调整会员透支账户的定义额度，同时更新透支账户余额
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="newDefineAmount"></param>
        /// <returns></returns>
        string[] AdjustOverdraftAccountDefineAmount(long cardId, decimal newDefineAmount);

        string CreateAccount(string cardId, string accountTypeId, string accountType, string overdraftAccount, string value = "", string creditLine = "");

        decimal GetRedisCardTotal(long cardId, string accountType);

        string[] PointPoolBalance(string jsonStr, decimal transactionPoint);
        int? GetCouponInventory(long couponTypeId);
        void UpdateRedisCouponInventory(long couponTypeId, int Inventory);

        bool IsExistRedisAccountInfo(long cardId, long membershipCardAccountId, string accountType);

        void DeleteAccount(string cardId, string accountTypeId, string accountType);

    }
}
