namespace Kunlun.LPS.Worker.Core.Enum
{
    /// <summary>
    /// 交易类型
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// 储值
        /// </summary>
        Topup = 0,

        /// <summary>
        /// 消费支付
        /// </summary>
        Consumption = 1,

        /// <summary>
        /// 储值退费
        /// </summary>
        TopupRefund = 2,

        /// <summary>
        /// 消费退费
        /// </summary>
        ConsumptionRefund = 3,


        /// <summary>
        /// 积分兑换券
        /// </summary>
        PointsExchangeCoupons = 4,

        /// <summary>
        /// 核销券
        /// </summary>
        RedeemCoupons = 5,


        /// <summary>
        /// 取消兑换券
        /// </summary>
        CancelExchangeCoupon = 6,

        /// <summary>
        /// 调整积分
        /// </summary>
        AdjustPoints = 7,

        /// <summary>
        /// 合并会员卡交易流水（由于会员卡交易表没有交易类型这一项，所以加在交易表中）
        /// </summary>
        MergeMembershipCardTransaction = 8,

        /// <summary>
        /// 积分换房
        /// </summary>
        PointExchangeRoom = 9,

        /// <summary>
        /// 取消积分换房
        /// </summary>
        CancelPointExchangeRoom = 10,

        /// <summary>
        /// 积分互换
        /// </summary>
        PointsConversion = 11,

        /// <summary>
        /// 转移积分
        /// </summary>
        PointsTransfer = 12,

        /// <summary>
        /// 调整里程
        /// </summary>
        AdjustMileages = 13,

        /// <summary>
        /// 赠送券
        /// </summary>
        GiftCoupons = 14,

        /// <summary>
        /// 赠送、奖励积分
        /// </summary>
        BonusPoints = 15,

        /// <summary>
        /// 售卖礼品卡
        /// </summary>
        SalesGiftCard = 16,

        /// <summary>
        /// 退礼品卡
        /// </summary>
        RefundGiftCard = 17,

        /// <summary>
        /// 激活礼品卡
        /// </summary>
        ActivationGiftCard = 18,

        /// <summary>
        /// 赠送礼品卡
        /// </summary>
        GiveGiftCard = 19,

        /// <summary>
        /// 接收礼品卡
        /// </summary>
        ReceiveGiftCard = 20,

        /// <summary>
        /// 售卖权益
        /// </summary>
        SalesPrivilege = 21,

        /// <summary>
        /// 使用权益
        /// </summary>
        UsePrivilege = 22,

        /// <summary>
        /// 导入会员积分
        /// </summary>
        ImportPoints = 23,

        /// <summary>
        /// 导入会员卡值
        /// </summary>
        ImportCardValue = 24,

        /// <summary>
        /// 转账
        /// </summary>
        TransferAccount = 25,

        /// <summary>
        /// 调整卡值
        /// </summary>
        Adjust = 26,

        /// <summary>
        /// 购买积分
        /// </summary>
        PurchasePoints = 27,
		
        /// <summary>
        /// 分裂卡
        /// </summary>
        DivisionCard = 28,

        /// <summary>
        /// 券过期
        /// </summary>
        CouponExpires = 29,

        /// <summary>
        /// 签到奖励
        /// </summary>
        DailyCheckIn = 30,

        /// <summary>
        /// 充值活动奖励
        /// </summary>
        RechargeActivity = 31,

        /// <summary>
        /// 升级赠送积分
        /// </summary>
        UpgradeGiftPoint = 32,

        /// <summary>
        /// 积分过期
        /// </summary>
        PointsExpire = 33,
        /// <summary>
        /// 券到期时间更改
        /// </summary>
        CouponExpirationTimeChange = 34,

        /// <summary>
        /// 初始化卡值
        /// </summary>
        InitCardFee = 35,

        /// <summary>
        /// 过期
        /// </summary>
        Expired = 36,
    }
}
