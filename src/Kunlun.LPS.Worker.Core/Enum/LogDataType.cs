using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Enum
{
    /// <summary>
    /// 日志数据类型
    /// </summary>
    public enum LogDataType
    {
        /// <summary>
        /// 档案
        /// </summary>
        Profile,

        /// <summary>
        /// 会员卡
        /// </summary>
        MembershipCard,

        /// <summary>
        /// 会员卡账户
        /// </summary>
        Account,

        /// <summary>
        /// 卡类型定义
        /// </summary>
        MembershipCardType,

        /// <summary>
        /// 卡级别定义
        /// </summary>
        MembershipCardLevel,

        /// <summary>
        /// 卡账户定义
        /// </summary>
        MembershipCardAccount,

        /// <summary>
        /// 卡批次
        /// </summary>
        MembershipCardBatch,

        /// <summary>
        /// 卡批次详情
        /// </summary>
        MembershipCardBatchDetail,

        /// <summary>
        /// 卡号段
        /// </summary>
        MembershipCardNumberSegment,

        // TODO 各种规则、事件通知、原因维护等数据类型

        /// <summary>
        /// 充值规则
        /// </summary>
        RechargeRules,

        /// <summary>
        /// 预设面值
        /// </summary>
        PresetCardValue,

        /// <summary>
        /// 折扣项定义
        /// </summary>
        DiscountConsumptionItems,

        /// <summary>
        /// 折扣规则
        /// </summary>
        DiscountRules,

        /// <summary>
        /// 支付规则
        /// </summary>
        PaymentRules,

        /// <summary>
        /// 事件通知
        /// </summary>
        CustomEvent,

        /// <summary>
        /// 原因维护
        /// </summary>
        Reason,

        /// <summary>
        /// 发票抬头
        /// </summary>
        InvoiceTitle,

        /// <summary>
        /// 发票
        /// </summary>
        Invoice,

        /// <summary>
        /// 费率设置
        /// </summary>
        ManagementFeesRatio,

        /// <summary>
        /// 客房消费积分规则定义
        /// </summary>
        RoomPointsRule,

        /// <summary>
        /// 客房消费里程规则定义
        /// </summary>
        RoomMileagesRule,

        /// <summary>
        /// 客房消费促销积分规则定义
        /// </summary>
        RoomPromotionPointsRule,

        /// <summary>
        /// 客房消费积分规则定义(Trcode模式)
        /// </summary>
        RoomPointsRuleForBucket,

        /// <summary>
        /// 客房消费促销里程规则定义
        /// </summary>
        RoomPromotionMileagesRule,

        /// <summary>
        /// 客房消费促销积分规则定义(Trcode模式)
        /// </summary>
        RoomPromotionPointsRuleForBucket,

        /// <summary>
        /// 餐饮消费积分规则定义
        /// </summary>
        FbPointsRule,

        /// <summary>
        /// 挂房账餐饮消费积分规则定义
        /// </summary>
        FbPointsRuleGuestLedger,
        /// <summary>
        /// 挂房账餐饮消费成长值规则定义
        /// </summary>
        FbGrowthRuleGuestLedger,
        /// <summary>
        /// 餐段设置
        /// </summary>
        MealPeriod,

        /// <summary>
        /// 餐饮消费成长值规则定义
        /// </summary>
        FbGrowthRule,


        /// <summary>
        /// 客房消费成长值规则
        /// </summary>
        RoomGrowthRule,

        /// <summary>
        /// 客房消费促销成长值规则
        /// </summary>
        RoomPromotionGrowthRule,

        /// <summary>
        /// 客房消费成长值规则（trcode模式）
        /// </summary>
        RoomGrowthRuleForBucket,

        /// <summary>
        /// 客房消费促销成长值规则（trcode模式）
        /// </summary>
        RoomPromotionGrowthRuleForBucket,

        /// <summary>
        /// 自定义日期送券规则
        /// </summary>
        CustomDateCouponRules,

        /// <summary>
        /// 自定义日期送积分规则
        /// </summary>
        CustomDateIntegralRules,

        /// <summary>
        /// 升降级规则
        /// </summary>
        UpgradeAndDowngradeRule,

        /// <summary>
        /// 介绍人积分规则
        /// </summary>
        IntroducerPointsRule,

        /// <summary>
        /// 客房消费积分规则定义
        /// </summary>
        BookerPointsRule,

        /// <summary>
        /// 兑换券定义
        /// </summary>
        Coupons,

        /// <summary>
        /// 会员身上的券
        /// </summary>
        Coupon,

        /// <summary>
        /// 兑换券发放渠道定义
        /// </summary>
        CouponChannel,

        /// <summary>
        /// 会员偏好
        /// </summary>
        Preference,

        /// <summary>
        /// 提前发送通知
        /// </summary>
        AdvanceSendInfo,

        /// <summary>
        /// 核销券
        /// </summary>
        UseCoupon,

        /// <summary>
        /// 注销券
        /// </summary>
        CancelCoupon,

        /// <summary>
        /// 合并会员
        /// </summary>
        MergeProfile,

        /// <summary>
        /// 合并会员卡
        /// </summary>
        MergeMembershipCard,

        /// <summary>
        /// 审核礼品发放
        /// </summary>
        VerifyGiftSend,

        /// <summary>
        /// 积分类型
        /// </summary>
        PointsAccrueType,

        /// <summary>
        /// 积分支付比例定义
        /// </summary>
        PointsPaymentRatio,

        /// <summary>
        /// 商户类型
        /// </summary>
        MerchantType,

        /// <summary>
        /// 多业态积分规则
        /// </summary>
        MultiPointsRule,

        /// <summary>
        /// 多业态成长值规则
        /// </summary>
        MultiGrowthRule,

        /// <summary>
        /// 预授权
        /// </summary>
        CardAuthorization,

        /// <summary>
        /// 调整里程
        /// </summary>
        AdjustMileages,

        DbCache,

        /// <summary>
        /// 外部会员
        /// </summary>
        ProfileExternalMembership,

        /// <summary>
        /// 礼品卡
        /// </summary>
        GiftCard,

        /// <summary>
        /// 礼品卡投放渠道
        /// </summary>
        GiftCardChannel,

        /// <summary>
        /// 汇率
        /// </summary>
        ExchangeRate,

        /// <summary>
        /// 消费限制
        /// </summary>
        ConsumeLimitRule,

        /// <summary>
        /// 支付限制
        /// </summary>
        PaymentLimitRule,

        /// <summary>
        /// 储值升级规则
        /// </summary>
        RechargeUpgradeRule,

        /// <summary>
        /// 权益
        /// </summary>
        Privilege,

        /// <summary>
        /// 券包
        /// </summary>
        CouponPackageType,

        /// <summary>
        /// 扩展字段大类
        /// </summary>
        MetadataCategoryType,

        /// <summary>
        /// 扩展字段定义
        /// </summary>
        MetadataType,

        /// <summary>
        /// 标签定义
        /// </summary>
        TagType,

        /// <summary>
        /// 档案标签
        /// </summary>
        ProfileTag,

        /// <summary>
        /// 会员卡有效期
        /// </summary>
        MembershipCardValidPeriod,

        /// <summary>
        /// 累计卡值支付金额成长值规则
        /// </summary>
        MembershipCardTransactionGrowthRule,

        /// <summary>
        /// 卡余额成长值规则
        /// </summary>
        MembershipCardBalanceGrowthRule,

        /// <summary>
        /// 累计消费金额成长值规则
        /// </summary>
        ConsumeGrowthRule,

        /// <summary>
        /// 累计间夜成长值规则
        /// </summary>
        NightsGrowthRule,

        /// <summary>
        /// 累计客房消费次数成长值规则
        /// </summary>
        RoomConsumeTimesGrowthRule,
        /// <summary>
        /// 消费项分组和折扣项
        /// </summary>
        SalesItemizerSendInfo,
        /// <summary>
        /// 权限控制增加卡号参数控制
        /// </summary>
        CardReaderSettingInfo,

        /// <summary>
        /// 签到规则
        /// </summary>
        DailyCheckInRule,

        /// <summary>
        /// 活动规则
        /// </summary>
        ActivityRule,

        /// <summary>
        /// 充值活动规则
        /// </summary>
        RechargeActivityRule,

        /// <summary>
        /// 升级赠送积分
        /// </summary>
        UpgradeBonusPointsRule,

        /// <summary>
        /// 积分上限规则
        /// </summary>
        PointsLimitRule,

        /// <summary>
        /// 积分使用日期规则
        /// </summary>
        PointUseDateRule,

        /// <summary>
        /// 业态类型
        /// </summary>
        ConsumeType,

        /// <summary>
        /// 注册赠送券规则
        /// </summary>
        RegisterCouponsRule,

        /// <summary>
        /// 定期储值清零规则
        /// </summary>
        RegularValueStorageRule,

    }
}
