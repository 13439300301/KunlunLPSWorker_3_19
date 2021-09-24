namespace Kunlun.LPS.Worker.Core.Enum
{
    public class PointsAccrueType
    {
        /// <summary>
        /// 客房
        /// </summary>
        public const string Room = nameof(Room);

        /// <summary>
        /// 餐饮
        /// </summary>
        public const string Fb = nameof(Fb);

        /// <summary>
        /// 订房人
        /// </summary>
        public const string B = nameof(B);

        /// <summary>
        /// 介绍人
        /// </summary>
        public const string I = nameof(I);

        /// <summary>
        /// 注册
        /// </summary>
        public const string Registered = nameof(Registered);

        /// <summary>
        /// 券
        /// </summary>
        public const string Coupons = nameof(Coupons);

        /// <summary>
        /// 积分过期
        /// </summary>
        public const string Expired = nameof(Expired);

        /// <summary>
        /// 挂房账餐饮
        /// </summary>
        public const string GuestLedger = nameof(GuestLedger);

        /// <summary>
        /// 客房促销
        /// </summary>
        public const string RoomPromotion = nameof(RoomPromotion);

        /// <summary>
        /// 餐饮促销
        /// </summary>
        public const string FbPromotion = nameof(FbPromotion);

        /// <summary>
        /// 合并积分流水
        /// </summary>
        public const string MergePointsHistory = nameof(MergePointsHistory);

        /// <summary>
        /// 导入积分流水
        /// </summary>
        public const string Import = nameof(Import);

        /// <summary>
        /// 宴会
        /// </summary>
        public const string Banquet = nameof(Banquet);

        /// <summary>
        /// 宴会促销
        /// </summary>
        public const string BanquetPromotion = nameof(BanquetPromotion);

        /// <summary>
        /// 挂房账宴会
        /// </summary>
        public const string BanquetGuestLedger = nameof(BanquetGuestLedger);

        /// <summary>
        /// 积分兑换房
        /// </summary>
        public const string ExchangeRoom = nameof(ExchangeRoom);

        /// <summary>
        /// 积分支付
        /// </summary>
        public const string Payment = nameof(Payment);

        /// <summary>
        /// 取消积分支付
        /// </summary>
        public const string Void = nameof(Void);

        // 以下为兼容老项目而添加的交易类型
        // 最好是不要让新版操作员随便选这些积分交易类型，但产品觉得无所谓，不需要通过程序控制，应该在培训时说清楚

        // 赠送
        public const string Bonus = nameof(Bonus);

        // 规则自动清除积分
        public const string C = nameof(C);

        // 续费产生积分
        public const string F = nameof(F);

        // 手工调整积分
        public const string M = nameof(M);

        // 会议积分
        public const string Meeting = nameof(Meeting);

        // 购买积分
        public const string P = nameof(P);

        // 推广、促销积分
        public const string Promotion = nameof(Promotion);

        // 转移积分
        public const string T = nameof(T);

        /// <summary>
        /// 签到积分
        /// </summary>
        public const string DailyCheckIn = nameof(DailyCheckIn);

        /// <summary>
        /// 签到奖励积分
        /// </summary>
        public const string DailyCheckInBonus = nameof(DailyCheckInBonus);

        /// <summary>
        /// 充值活动奖励积分
        /// </summary>
        public const string RechargeInBonus = nameof(RechargeInBonus);

        /// <summary>
        /// 升级赠送积分
        /// </summary>
        public const string LevelChangeInBonus = nameof(LevelChangeInBonus);

        /// <summary>
        /// 卡值支付奖励积分
        /// </summary>
        public const string BonusPoints = nameof(BonusPoints);
    }
}
