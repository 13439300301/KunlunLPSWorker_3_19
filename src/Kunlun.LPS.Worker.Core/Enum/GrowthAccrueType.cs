namespace Kunlun.LPS.Worker.Core.Enum
{
    /// <summary>
    /// 成长值类型枚举
    /// </summary>
    public enum GrowthAccrueType
    {
        /// <summary>
        /// 客房
        /// </summary>
        Room,

        /// <summary>
        /// 餐饮
        /// </summary>
        Fb,

        /// <summary>
        /// 升降级
        /// </summary>
        UpgradeAndDowngrade,

        /// <summary>
        /// 挂房账餐饮
        /// </summary>
        GuestLedger,

        /// <summary>
        /// 客房促销
        /// </summary>
        RoomPromotion,

        /// <summary>
        /// 餐饮促销
        /// </summary>
        FbPromotion,

        /// <summary>
        /// 宴会
        /// </summary>
        Banquet = 6,

        /// <summary>
        /// 宴会促销
        /// </summary>
        BanquetPromotion = 7,

        /// <summary>
        /// 挂房账宴会
        /// </summary>
        BanquetGuestLedger = 8,

        /// <summary>
        /// 调整成长值
        /// </summary>
        AdjustGrowth = 9,

        /// <summary>
        /// 多业态成长值
        /// </summary>
        MultiGrowth = 10,

        /// <summary>
        /// 储值成长值
        /// </summary>
        StoreValue = 11,

        /// <summary>
        /// 间夜成长值
        /// </summary>
        Nights = 12,

        /// <summary>
        /// 合并成长值
        /// </summary>
        MergeGrowthHistory = 13,

        /// <summary>
        /// 累计卡值支付金额成长值
        /// </summary>
        MembershipCardTransactionGrowth = 14,

        /// <summary>
        /// 卡余额成长值
        /// </summary>
        MembershipCardBalanceGrowth = 15,

        /// <summary>
        /// 累计消费金额成长值
        /// </summary>
        ConsumeGrowth = 16,

        /// <summary>
        /// 累计间夜成长值
        /// </summary>
        NightsGrowth = 17,

        /// <summary>
        /// 累计客房消费次数成长值
        /// </summary>
        RoomConsumeTimesGrowth = 18,

        /// <summary>
        /// 签到成长值
        /// </summary>
        DailyCheckIn = 19,

        /// <summary>
        /// 签到奖励成长值
        /// </summary>
        DailyCheckInBonus = 20,

        /// <summary>
        /// 充值活动奖励成长值
        /// </summary>
        RechargeActivityInBonus = 21,
    }
}
