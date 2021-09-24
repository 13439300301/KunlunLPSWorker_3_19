using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Enum
{
    /// <summary>
    /// 日志操作类型
    /// </summary>
    public enum LogOperationType
    {
        #region Profile

        /// <summary>
        /// 添加档案
        /// </summary>
        AddProfile,

        /// <summary>
        /// 修改档案信息
        /// </summary>
        EditProfile,

        /// <summary>
        /// 修改档案密码
        /// </summary>
        ChangeProfilePassword,
        /// <summary>
        /// 修改档案手机号
        /// </summary>
        ChangeProfilePhoneNumber,

        /// <summary>
        /// 重置档案密码
        /// </summary>
        ResetProfilePassword,

        #endregion

        #region ProfileExternalMembership
        /// <summary>
        /// 添加外部会员
        /// </summary>
        AddProfileExternalMembership,
        #endregion

        #region MembershipCard

        /// <summary>
        /// 添加会员卡
        /// </summary>
        AddMembershipCard,

        /// <summary>
        /// 修改会员卡信息
        /// </summary>
        EditMembershipCard,

        /// <summary>
        /// 修改会员卡支付密码
        /// </summary>
        ChangeMembershipCardPassword,

        /// <summary>
        /// 重置会员卡支付密码
        /// </summary>
        ResetMembershipCardPassword,

        /// <summary>
        /// 修改会员卡状态
        /// </summary>
        ChangeMembershipCardStatus,

        /// <summary>
        /// 变更会员卡级别
        /// </summary>
        ChangeMembershipCardLevel,

        /// <summary>
        /// 保级
        /// </summary>
        KeepMembershipCardLevel,

        /// <summary>
        /// 变更会员渠道
        /// </summary>
        ChangeMembershipCardchannel,
        /// <summary>
        /// 挂失旧卡并发新卡
        /// </summary>
        MergeMembershipCard,

        /// <summary>
        /// 变更会员卡有效期
        /// </summary>
        ChangeMembershipCardValidPeriod,
        /// <summary>
        /// 变更会员卡注册日期
        /// </summary>
        ChangeCardRegistrationDate,

        /// <summary>
        /// 修改会员卡卡面号
        /// </summary>
        EditMembershipCardFaceNumber,
        /// <summary>
        /// 修改会员卡开卡日期
        /// </summary>
        EditMembershipCardOpenCardDate,
        #endregion

        #region MembershipCardType

        /// <summary>
        /// 添加卡类型
        /// </summary>
        AddMembershipCardType,

        /// <summary>
        /// 修改卡类型
        /// </summary>
        EditMembershipCardType,

        /// <summary>
        /// 删除卡类型
        /// </summary>
        DeleteMembershipCardType,

        #endregion

        #region MembershipCardLevel

        /// <summary>
        /// 添加卡级别
        /// </summary>
        AddMembershipCardLevel,

        /// <summary>
        /// 修改卡级别
        /// </summary>
        EditMembershipCardLevel,


        /// <summary>
        /// 删除卡级别
        /// </summary>
        DeleteMembershipCardLevel,

        #endregion MembershipCardNumberSegment

        #region MembershipCardAccount

        /// <summary>
        /// 添加卡账户
        /// </summary>
        AddMembershipCardAccount,

        /// <summary>
        /// 修改卡账户
        /// </summary>
        EditMembershipCardAccount,

        /// <summary>
        /// 删除卡账户
        /// </summary>
        DeleteMembershipCardAccount,

        #endregion

        #region MembershipCardBatch

        /// <summary>
        /// 添加会员卡批次
        /// </summary>
        AddMembershipCardBatch,

        /// <summary>
        /// 修改会员卡批次
        /// </summary>
        EditMembershipCardBatch,

        /// <summary>
        /// 会员卡入库
        /// </summary>
        SetMembershipCardBatchToInStock,

        /// <summary>
        /// 导出会员卡批次
        /// </summary>
        ExportMembershipCardBatch,


        // TODO 各种规则、事件通知、原因维护的增删改操作

        #endregion

        #region MembershipCardBatchDetail
        /// <summary>
        /// 添加会员卡批次详情
        /// </summary>
        AddMembershipCardBatchDetail,
        #endregion

        #region RechargeRules

        /// <summary>
        /// 添加充值规则
        /// </summary>
        AddRechargeRules,

        /// <summary>
        /// 修改充值规则
        /// </summary>
        EditRechargeRules,

        /// <summary>
        /// 删除充值规则
        /// </summary>
        DeleteRechargeRules,

        #endregion

        #region PresetCardValue

        /// <summary>
        /// 添加预设面值
        /// </summary>
        AddPresetCardValue,

        /// <summary>
        /// 修改预设面值
        /// </summary>
        EditPresetCardValue,

        /// <summary>
        /// 删除预设面值
        /// </summary>
        DeletePresetCardValue,

        #endregion

        #region DiscountConsumptionItems

        /// <summary>
        /// 添加折扣项定义
        /// </summary>
        AddDiscountConsumptionItems,

        /// <summary>
        /// 修改折扣项定义
        /// </summary>
        EditDiscountConsumptionItems,

        /// <summary>
        /// 删除折扣项定义
        /// </summary>
        DeleteDiscountConsumptionItems,

        #endregion

        #region DiscountRules

        /// <summary>
        /// 添加折扣规则
        /// </summary>
        AddDiscountRules,

        /// <summary>
        /// 修改折扣规则
        /// </summary>
        EditDiscountRules,

        /// <summary>
        /// 删除折扣规则
        /// </summary>
        DeleteDiscountRules,

        #endregion PaymentRules

        #region PaymentRules

        /// <summary>
        /// 添加支付规则
        /// </summary>
        AddPaymentRules,

        /// <summary>
        /// 修改支付规则
        /// </summary>
        EditPaymentRules,

        /// <summary>
        /// 删除支付规则
        /// </summary>
        DeletePaymentRules,

        #endregion


        #region CustomEvent

        /// <summary>
        /// 添加事件通知
        /// </summary>
        AddCustomEvent,

        /// <summary>
        /// 修改事件通知
        /// </summary>
        EditCustomEvent,

        /// <summary>
        /// 删除事件通知
        /// </summary>
        DeleteCustomEvent,

        #endregion

        #region Reason

        /// <summary>
        /// 添加原因维护
        /// </summary>
        AddReason,

        /// <summary>
        /// 修改原因维护
        /// </summary>
        EditReason,

        /// <summary>
        /// 删除原因维护
        /// </summary>
        DeleteReason,

        #endregion

        #region MembershipCardNumberSegment

        /// <summary>
        /// 添加卡号段
        /// </summary>
        AddMembershipCardNumberSegment,

        /// <summary>
        /// 修改卡号段
        /// </summary>
        EditMembershipCardNumberSegment,

        /// <summary>
        /// 删除卡号段
        /// </summary>
        DeleteMembershipCardNumberSegment,

        #endregion

        #region InvoiceTitle

        /// <summary>
        /// 添加发票抬头
        /// </summary>
        AddInvoiceTitle,

        /// <summary>
        /// 修改发票抬头
        /// </summary>
        EditInvoiceTitle,

        /// <summary>
        /// 删除发票抬头
        /// </summary>
        DeleteInvoiceTitle,

        #endregion

        #region Invoice

        /// <summary>
        /// 开具发票
        /// </summary>
        WriteInvoice,

        /// <summary>
        /// 作废发票
        /// </summary>
        InvalidInvoice,

        #endregion


        #region AdjustPoints
        /// <summary>
        /// 调整积分
        /// </summary>
        AdjustPoints,
        #endregion AdjustPoints

        #region PointsTransfer
        /// <summary>
        /// 转移积分
        /// </summary>
        PointsTransfer,
        #endregion PointsTransfer

        #region ManagementFeesRatio
        /// <summary>
        /// 添加费率
        /// </summary>
        AddManagementFeesRatio,

        /// <summary>
        /// 修改费率
        /// </summary>
        EditManagementFeesRatio,

        /// <summary>
        /// 删除费率
        /// </summary>
        DeleteManagementFeesRatio,
        #endregion

        #region RoomPointsRule
        /// <summary>
        /// 添加客房消费积分规则
        /// </summary>
        AddRoomPointsRule,

        /// <summary>
        /// 编辑客房消费积分规则
        /// </summary>
        EditRoomPointsRule,

        /// <summary>
        /// 删除客房消费积分规则
        /// </summary>
        DeleteRoomPointsRule,

        /// <summary>
        /// 添加客房消费积分规则(Trcode模式)
        /// </summary>
        AddRoomPointsRuleForBucket,

        /// <summary>
        /// 编辑客房消费积分规则
        /// </summary>
        EditRoomPointsRuleForBucket,

        /// <summary>
        /// 删除客房消费积分规则
        /// </summary>
        DeleteRoomPointsRuleForBucket,
        #endregion RoomPointsRule

        #region RoomMileagesRule
        /// <summary>
        /// 添加客房消费里程规则
        /// </summary>
        AddRoomMileagesRule,

        /// <summary>
        /// 编辑客房消费里程规则
        /// </summary>
        EditRoomMileagesRule,

        /// <summary>
        /// 删除客房消费里程规则
        /// </summary>
        DeleteRoomMileagesRule,

        /// <summary>
        /// 添加客房消费里程规则(Trcode模式)
        /// </summary>
        AddRoomMileagesRuleForBucket,

        /// <summary>
        /// 编辑客房消费里程规则
        /// </summary>
        EditRoomMileagesRuleForBucket,

        /// <summary>
        /// 删除客房消费里程规则
        /// </summary>
        DeleteRoomMileagesRuleForBucket,
        #endregion RoomMileagesRule

        #region 客房消费促销积分规则
        /// <summary>
        /// 添加客房消费促销积分规则
        /// </summary>
        AddRoomPromotionPointsRule,

        /// <summary>
        /// 编辑客房消费促销积分规则
        /// </summary>
        EditRoomPromotionPointsRule,

        /// <summary>
        /// 删除客房消费促销积分规则
        /// </summary>
        DeletePromotionRoomPointsRule,

        /// <summary>
        /// 添加客房消费促销积分规则(Trcode模式)
        /// </summary>
        AddRoomPromotionPointsRuleForBucket,

        /// <summary>
        /// 编辑客房消费促销积分规则
        /// </summary>
        EditRoomPromotionPointsRuleForBucket,

        /// <summary>
        /// 删除客房消费促销积分规则
        /// </summary>
        DeleteRoomPomotionPointsRuleForBucket,
        #endregion

        #region 客房消费促里程规则
        /// <summary>
        /// 添加客房消费促销积分规则
        /// </summary>
        AddRoomPromotionMileagesRule,

        /// <summary>
        /// 编辑客房消费促销积分规则
        /// </summary>
        EditRoomPromotionMileagesRule,

        /// <summary>
        /// 删除客房消费促销积分规则
        /// </summary>
        DeletePromotionRoomMileagesRule,

        /// <summary>
        /// 添加客房消费促销积分规则(Trcode模式)
        /// </summary>
        AddRoomPromotionMileagesRuleForBucket,

        /// <summary>
        /// 编辑客房消费促销积分规则
        /// </summary>
        EditRoomPromotionMileagesRuleForBucket,

        /// <summary>
        /// 删除客房消费促销积分规则
        /// </summary>
        DeleteRoomPomotionMileagesRuleForBucket,
        #endregion

        #region FbPointsRule
        /// <summary>
        /// 添加餐饮消费积分规则
        /// </summary>
        AddFbPointsRule,

        /// <summary>
        /// 编辑餐饮消费积分规则
        /// </summary>
        EditFbPointsRule,

        /// <summary>
        /// 删除餐饮消费积分规则
        /// </summary>
        DeleteFbPointsRule,

        #endregion FbPointsRule

        #region FbPointsRuleGuestLedger
        /// <summary>
        /// 添加餐饮消费挂房账积分规则
        /// </summary>
        AddFbPointsRuleGuestLedger,
        /// <summary>
        /// 修改餐饮消费挂房账积分规则
        /// </summary>
        EditFbPointsRuleGuestLedger,
        /// <summary>
        /// 删除餐饮消费挂房账积分规则
        /// </summary>
        DeleteFbPointsRuleGuestLedger,
        #endregion FbPointsRuleGuestLedger

        #region MealPeriod
        /// <summary>
        /// 添加餐段设置
        /// </summary>
        AddMealPeriod,

        /// <summary>
        /// 编辑餐段设置
        /// </summary>
        EditMealPeriod,

        /// <summary>
        /// 删除餐段设置
        /// </summary>
        DeleteMealPeriod,
        #endregion

        #region FbGrowthRule
        /// <summary>
        /// 添加餐饮消费成长值规则
        /// </summary>
        AddFbGrowthRule,

        /// <summary>
        /// 编辑餐饮消费成长值规则
        /// </summary>
        EditFbGrowthRule,

        /// <summary>
        /// 删除餐饮消费成长值规则
        /// </summary>
        DeleteFbGrowthRule,
        #endregion

        #region FbGrowthRuleGuestLedger
        /// <summary>
        /// 添加餐饮消费挂房账成长值规则
        /// </summary>
        AddFbGrowthRuleGuestLedger,
        /// <summary>
        /// 修改餐饮消费挂房账成长值规则
        /// </summary>
        EditFbGrowthRuleGuestLedger,
        /// <summary>
        /// 删除餐饮消费挂房账成长值规则
        /// </summary>
        DeleteFbGrowthRuleGuestLedger,
        #endregion FbGrowthRuleGuestLedger
        #region RoomGrowthRule
        /// <summary>
        /// 添加客房消费成长值规则
        /// </summary>
        AddRoomGrowthRule,

        /// <summary>
        /// 编辑客房消费成长值规则
        /// </summary>
        EditRoomGrowthRule,

        /// <summary>
        /// 删除客房消费成长值规则
        /// </summary>
        DeleteRoomGrowthRule,
        #endregion

        #region RoomPromotionGrowthRule
        /// <summary>
        /// 添加客房消费成长值规则
        /// </summary>
        AddRoomPromotionGrowthRule,

        /// <summary>
        /// 编辑客房消费成长值规则
        /// </summary>
        EditRoomPromotionGrowthRule,

        /// <summary>
        /// 删除客房消费成长值规则
        /// </summary>
        DeleteRoomPromotionGrowthRule,
        #endregion

        #region RoomGrowthRuleForBucket
        /// <summary>
        /// 添加客房消费成长值规则（trcode模式）
        /// </summary>
        AddRoomGrowthRuleForBucket,

        /// <summary>
        /// 编辑客房消费成长值规则（trcode模式）
        /// </summary>
        EditRoomGrowthRuleForBucket,

        /// <summary>
        /// 删除客房消费成长值规则（trcode模式）
        /// </summary>
        DeleteRoomGrowthRuleForBucket,
        #endregion

        #region RoomPromotionGrowthRuleForBucket
        /// <summary>
        /// 添加客房消费促销成长值规则（trcode模式）
        /// </summary>
        AddRoomPromotionGrowthRuleForBucket,

        /// <summary>
        /// 编辑客房消费促销成长值规则（trcode模式）
        /// </summary>
        EditRoomPromotionGrowthRuleForBucket,

        /// <summary>
        /// 删除客房消费促销成长值规则（trcode模式）
        /// </summary>
        DeleteRoomPromotionGrowthRuleForBucket,
        #endregion

        #region UpgradeAndDowngradeRule
        /// <summary>
        /// 添加客房消费成长值规则
        /// </summary>
        AddUpgradeAndDowngradeRule,

        /// <summary>
        /// 编辑客房消费成长值规则
        /// </summary>
        EditUpgradeAndDowngradeRule,

        /// <summary>
        /// 删除客房消费成长值规则
        /// </summary>
        DeleteUpgradeAndDowngradeRule,
        #endregion

        #region IntroducerPointsRule
        /// <summary>
        /// 添加客房消费成长值规则
        /// </summary>
        AddIntroducerPointsRule,

        /// <summary>
        /// 编辑客房消费成长值规则
        /// </summary>
        EditIntroducerPointsRule,

        /// <summary>
        /// 删除客房消费成长值规则
        /// </summary>
        DeleteIntroducerPointsRule,

        #endregion

        #region BookerPointsRule
        /// <summary>
        /// 添加客房消费积分规则
        /// </summary>
        AddBookerPointsRule,

        /// <summary>
        /// 编辑客房消费积分规则
        /// </summary>
        EditBookerPointsRule,

        /// <summary>
        /// 删除客房消费积分规则
        /// </summary>
        DeleteBookerPointsRule,
        #endregion BookerPointsRule

        #region Coupons
        /// <summary>
        /// 添加兑换券定义
        /// </summary>
        AddCoupons,
        /// <summary>
        /// 修改兑换券定义
        /// </summary>
        EditCoupons,

        /// <summary>
        /// 删除兑换券定义
        /// </summary>
        DeleteCoupons,

        /// <summary>
        /// 添加兑换券分类
        /// </summary>
        AddCouponClass,
        /// <summary>
        /// 修改兑换券分类
        /// </summary>
        EditCouponClass,

        /// <summary>
        /// 删除兑换券分类
        /// </summary>
        DeleteCouponClass,

        #endregion

        #region Coupon
        /// <summary>
        /// 更新会员身上的券的信息
        /// </summary>
        UpdateCoupon,
        #endregion

        #region CouponChannel
        /// <summary>
        /// 添加券发放渠道定义
        /// </summary>
        AddCouponChannel,
        /// <summary>
        /// 修改券发放渠道定义
        /// </summary>
        EditCouponChannel,

        /// <summary>
        /// 删除券发放渠道定义
        /// </summary>
        DeleteCouponChannel,

        #endregion

        #region Preference
        /// <summary>
        /// 更新会员身上的券的信息
        /// </summary>
        UpdatePreference,
        #endregion

        #region AdvanceSendInfo
        /// <summary>
        /// 添加提前发送通知规则配置
        /// </summary>
        AddAdvanceSendInfo,

        /// <summary>
        /// 编辑提前发送通知规则配置
        /// </summary>
        EditAdvanceSendInfo,

        /// <summary>
        /// 删除提前发送通知规则配置
        /// </summary>
        DeleteAdvanceSendInfo,
        #endregion

        #region UseCoupon

        /// <summary>
        /// 核销券
        /// </summary>
        UseCoupon,
        /// <summary>
        /// 注销券
        /// </summary>
        CancelCoupon,

        #endregion

        /// <summary>
        /// 合并会员
        /// </summary>
        MergeProfile,

        /// <summary>
        /// 审核礼品发放
        /// </summary>
        VerifyGiftSend,

        #region PointsAccrueType
        /// <summary>
        /// 添加积分类型定义
        /// </summary>
        AddPointsAccrueType,
        /// <summary>
        /// 修改积分类型定义
        /// </summary>
        EditPointsAccrueType,

        /// <summary>
        /// 删除积分类型定义
        /// </summary>
        DeletePointsAccrueType,

        #endregion

        #region MerchantType
        /// <summary>
        /// 添加商户类型定义
        /// </summary>
        AddMerchantType,
        /// <summary>
        /// 修改商户类型定义
        /// </summary>
        EditMerchantType,

        /// <summary>
        /// 删除商户类型定义
        /// </summary>
        DeleteMerchantType,

        #endregion

        #region MultiPointsRule
        /// <summary>
        /// 新增多业态积分规则
        /// </summary>
        AddMultiPointsRule,
        EditMultiPointsRule,
        DeleteMultiPointsRule,
        #endregion

        #region MultiGrowthRule
        /// <summary>
        /// 新增多业态成长值规则
        /// </summary>
        AddMultiGrowthRule,
        EditMultiGrowthRule,
        DeleteMultiGrowthRule,
        #endregion

        /// <summary>
        /// 添加积分支付比例
        /// </summary>
        AddPointsPaymentRatio,

        /// <summary>
        /// 修改积分支付比例
        /// </summary>
        EditPointsPaymentRatio,

        /// <summary>
        /// 删除积分支付比例
        /// </summary>
        DeletePointsPaymentRatio,

        /// <summary>
        /// 调整里程
        /// </summary>
        AdjustMileages,

        AddDbCache,

        UpdateDbCache,

        DeleteDbCache,

        #region 外部会员
        UpdateProfileExternalMembership,
        DeleteProfileExternalMembership,
        #endregion

        #region CardAuthorization
        /// <summary>
        /// 添加预授权
        /// </summary>
        AddCardAuthorization,
        /// <summary>
        /// 追加预授权
        /// </summary>
        UpdateCardAuthorization,
        /// <summary>
        /// 支付时释放预授权
        /// </summary>
        PayByCardAuthorization,
        /// <summary>
        /// 取消预授权
        /// </summary>
        CancelCardAuthorization,
        /// <summary>
        /// 自动取消预授权
        /// </summary>
        AutoCancelCardAuthorization,
        #endregion

        #region 业态类型
        /// <summary>
        /// 添加业态类型
        /// </summary>
        AddConsumeType,
        /// <summary>
        /// 编辑业态类型
        /// </summary>
        EditConsumeType,
        #endregion 业态类型


        #region GiftCard
        /// <summary>
        /// 添加礼品卡类型
        /// </summary>
        AddGiftCardType,
        /// <summary>
        /// 修改礼品卡类型
        /// </summary>
        EditGiftCardType,
        /// <summary>
        /// 删除礼品卡类型
        /// </summary>
        DeleteGiftCardType,
        /// <summary>
        /// 添加礼品卡类
        /// </summary>
        AddGiftCardCategoryType,
        /// <summary>
        /// 修改礼品卡类
        /// </summary>
        EditGiftCardCategoryType,
        /// <summary>
        /// 删除礼品卡类
        /// </summary>
        DeleteGiftCardCategoryType,
        /// <summary>
        /// 添加礼品卡子类
        /// </summary>
        AddGiftCardSubCategoryType,
        /// <summary>
        /// 修改礼品卡子类
        /// </summary>
        EditGiftCardSubCategoryType,
        /// <summary>
        /// 删除礼品卡子类
        /// </summary>
        DeleteGiftCardSubCategoryType,


        /// <summary>
        /// 添加礼品卡投放渠道定义
        /// </summary>
        AddGiftCardChannel,
        /// <summary>
        /// 修改礼品卡投放渠道定义
        /// </summary>
        EditGiftCardChannel,

        /// <summary>
        /// 删除礼品卡投放渠道定义
        /// </summary>
        DeleteGiftCardChannel,

        /// <summary>
        /// 修改礼品卡
        /// </summary>
        UpdateGiftCard,

        /// <summary>
        /// 售卖礼品卡
        /// </summary>
        SalesGiftCards,

        /// <summary>
        /// 退礼品卡
        /// </summary>
        RefundGiftCards,
        #endregion

        #region ExchangeRate
        /// <summary>
        /// 添加汇率
        /// </summary>
        AddExchangeRate,
        /// <summary>
        /// 修改汇率
        /// </summary>
        UpdateExchangeRate,
        /// <summary>
        /// 删除汇率
        /// </summary>
        DeleteExchangeRate,
        #endregion

        #region RoomCouponsRule
        /// <summary>
        /// 添加客房消费赠送券规则
        /// </summary>
        AddRoomCouponsRule,

        /// <summary>
        /// 编辑客房消费赠送券规则
        /// </summary>
        EditRoomCouponsRule,

        /// <summary>
        /// 删除客房消费赠送券规则
        /// </summary>
        DeleteRoomCouponsRule,
        #endregion

        #region 自定义日期送券规则
        /// <summary>
        /// 添加自定义日期送券规则
        /// </summary>
        AddCustomDateCouponRules,

        /// <summary>
        /// 编辑自定义日期送券规则
        /// </summary>
        EditCustomDateCouponRules,

        /// <summary>
        /// 删除自定义日期送券规则
        /// </summary>
        DeleteCustomDateCouponRules,
        #endregion

        #region 自定义日期送积分规则
        /// <summary>
        /// 添加自定义日期送积分规则
        /// </summary>
        AddCustomDateIntegralRules,

        /// <summary>
        /// 编辑自定义日期送积分规则
        /// </summary>
        EditCustomDateIntegralRules,

        /// <summary>
        /// 删除自定义日期送积分规则
        /// </summary>
        DeleteCustomDateIntegralRules,
        #endregion

        #region ConsumeLimitRule
        /// <summary>
        /// 添加消费限制
        /// </summary>
        AddConsumeLimitRule,
        /// <summary>
        /// 编辑消费限制
        /// </summary>
        EditConsumeLimitRule,
        /// <summary>
        /// 删除消费限制
        /// </summary>
        DeleteConsumeLimitRule,
        #endregion

        #region PaymentLimitRule
        /// <summary>
        /// 添加支付限制
        /// </summary>
        AddPaymentLimitRule,
        /// <summary>
        /// 编辑支付限制
        /// </summary>
        EditPaymentLimitRule,
        /// <summary>
        /// 删除支付限制
        /// </summary>
        DeletePaymentLimitRule,
        #endregion

        #region RechargeUpgradeRule
        /// <summary>
        /// 添加储值升级规则
        /// </summary>
        AddRechargeUpgradeRule,
        /// <summary>
        /// 编辑储值升级规则
        /// </summary>
        EditRechargeUpgradeRule,
        /// <summary>
        /// 删除储值升级规则
        /// </summary>
        DeleteRechargeUpgradeRule,
        #endregion
        #region RegularValueStorageRule
        /// <summary>
        /// 添加定期储值清零规则
        /// </summary>
        AddRegularValueStorageRule,
        /// <summary>
        /// 修改定期储值清零规则
        /// </summary>
        EditRegularValueStorageRule,
        /// <summary>
        /// 删除定期储值清零规则
        /// </summary>
        DeleteRegularValueStorageRule,

        #endregion

        #region Privilege
        /// <summary>
        /// 添加权益定义
        /// </summary>
        AddPrivilegeType,
        /// <summary>
        /// 修改权益定义
        /// </summary>
        EditPrivilegeType,
        /// <summary>
        /// 删除权益定义
        /// </summary>
        DeletePrivilegeType,

        /// <summary>
        /// 购买权益
        /// </summary>
        SalesPrivilege,

        /// <summary>
        /// 使用权益
        /// </summary>
        UsePrivilege,
        #endregion

        #region CouponPackage 券包

        /// <summary>
        /// 券包定义添加
        /// </summary>
        AddCouponPackageType,

        /// <summary>
        /// 删除券包定义
        /// </summary>
        DeleteCouponPackageType,
        #endregion

        #region MetadataCategoryType 扩展字段大类

        /// <summary>
        /// 添加扩展字段大类
        /// </summary>
        AddMetadataCategoryType,

        /// <summary>
        /// 修改扩展字段大类
        /// </summary>
        EditMetadataCategoryType,

        /// <summary>
        /// 删除扩展字段大类
        /// </summary>
        DeleteMetadataCategoryType,

        #endregion

        #region MetadataCategoryType 扩展字段定义

        /// <summary>
        /// 添加扩展字段定义
        /// </summary>
        AddMetadataType,

        /// <summary>
        /// 修改扩展字段定义
        /// </summary>
        EditMetadataType,



        #endregion

        #region TagType 标签定义
        /// <summary>
        /// 新建标签
        /// </summary>
        AddTagType,
        /// <summary>
        /// 编辑标签
        /// </summary>
        EditTagType,
        /// <summary>
        /// 删除标签
        /// </summary>
        DeleteTagType,
        #endregion

        #region ProfileTag 档案标签
        /// <summary>
        /// 添加档案标签
        /// </summary>
        AddProfileTag,
        /// <summary>
        /// 删除档案标签
        /// </summary>
        DeleteProfileTag,
        #endregion

        #region MembershipCardValidPeriod 会员卡有效期
        /// <summary>
        /// 添加有效期定义
        /// </summary>
        AddMembershipCardValidPeriod,
        /// <summary>
        /// 修改有效期定义
        /// </summary>
        EditMembershipCardValidPeriod,
        /// <summary>
        /// 删除有效期定义
        /// </summary>
        DeleteMembershipCardValidPeriod,
        #endregion

        #region MembershipCardTransactionGrowthRule 累计卡值支付金额成长值规则
        /// <summary>
        /// 添加累计卡值支付金额成长值规则
        /// </summary>
        AddMembershipCardTransactionGrowthRule,
        /// <summary>
        /// 修改累计卡值支付金额成长值规则
        /// </summary>
        EditMembershipCardTransactionGrowthRule,
        /// <summary>
        /// 删除累计卡值支付金额成长值规则
        /// </summary>
        DeleteMembershipCardTransactionGrowthRule,
        #endregion

        #region MembershipCardBalanceGrowthRule 卡余额成长值规则
        /// <summary>
        /// 添加卡余额成长值规则
        /// </summary>
        AddMembershipCardBalanceGrowthRule,
        /// <summary>
        /// 修改卡余额成长值规则
        /// </summary>
        EditMembershipCardBalanceGrowthRule,
        /// <summary>
        /// 删除卡余额成长值规则
        /// </summary>
        DeleteMembershipCardBalanceGrowthRule,
        #endregion

        #region ConsumeGrowthRule 累计消费金额成长值规则
        /// <summary>
        /// 添加累计消费金额成长值规则
        /// </summary>
        AddConsumeGrowthRule,
        /// <summary>
        /// 修改累计消费金额成长值规则
        /// </summary>
        EditConsumeGrowthRule,
        /// <summary>
        /// 删除累计消费金额成长值规则
        /// </summary>
        DeleteConsumeGrowthRule,
        #endregion

        #region NightsGrowthRule 累计间夜成长值规则
        /// <summary>
        /// 添加累计间夜成长值规则
        /// </summary>
        AddNightsGrowthRule,
        /// <summary>
        /// 修改累计间夜成长值规则
        /// </summary>
        EditNightsGrowthRule,
        /// <summary>
        /// 删除累计间夜成长值规则
        /// </summary>
        DeleteNightsGrowthRule,
        #endregion

        #region RoomConsumeTimesGrowthRule 累计客房消费次数成长值规则
        /// <summary>
        /// 添加累计客房消费次数成长值规则
        /// </summary>
        AddRoomConsumeTimesGrowthRule,
        /// <summary>
        /// 修改累计客房消费次数成长值规则
        /// </summary>
        EditRoomConsumeTimesGrowthRule,
        /// <summary>
        /// 删除累计客房消费次数成长值规则
        /// </summary>
        DeleteRoomConsumeTimesGrowthRule,
        #endregion

        #region SalesItemizerInfo 消费项分组和折扣项
        /// <summary>
        /// 添加消费项分组
        /// </summary>
        AddSalesItemizerInfo,
        /// <summary>
        /// 编辑消费项分组
        /// </summary>
        EditSalesItemizerSendInfo,
        /// <summary>
        /// 删除消费项分组
        /// </summary>
        DeleteSalesItemizerSendInfo,
        #endregion

        #region CardReaderSettingInfo 卡号参数控制
        /// <summary>
        /// 添加卡号参数控制
        /// </summary>
        AddCardReaderSettingInfo,
        /// <summary>
        /// 编辑卡号参数控制
        /// </summary>
        EditCardReaderSettingInfo,
        /// <summary>
        /// 删除卡号参数控制
        /// </summary>
        DeleteCardReaderSettingInfo,
        #endregion

        #region 签到规则
        /// <summary>
        /// 添加签到规则
        /// </summary>
        AddDailyCheckInRule,
        /// <summary>
        /// 编辑签到规则
        /// </summary>
        EditDailyCheckInRule,
        /// <summary>
        /// 删除签到规则
        /// </summary>
        DeleteDailyCheckInRule,
        #endregion

        #region 卡值转移规则
        /// <summary>
        /// 添加卡值转移规则
        /// </summary>
        AddStoredValueTransferRule,
        /// <summary>
        /// 编辑卡值转移规则
        /// </summary>
        EditStoredValueTransferRule,
        /// <summary>
        /// 删除卡值转移规则
        /// </summary>
        DeleteStoredValueTransferRule,
        #endregion


        #region 活动规则
        /// <summary>
        /// 添加活动规则
        /// </summary>
        AddActivityRule,
        /// <summary>
        /// 编辑活动规则
        /// </summary>
        EditActivityRule,
        /// <summary>
        /// 删除活动规则
        /// </summary>
        DeleteActivityRule,
        #endregion

        #region 充值活动规则

        /// <summary>
        /// 添加充值活动规则
        /// </summary>
        AddRechargeActivityRule,

        /// <summary>
        /// 修改充值活动规则
        /// </summary>
        EditRechargeActivityRule,

        /// <summary>
        /// 删除充值活动规则
        /// </summary>
        DeleteRechargeActivityRule,

        #endregion

        #region 升级赠送积分规则
        /// <summary>
        /// 添加升级赠送积分规则
        /// </summary>
        AddUpgradeBonusPointsRule,
        /// <summary>
        /// 编辑升级赠送积分规则
        /// </summary>
        EditUpgradeBonusPointsRule,
        /// <summary>
        /// 删除升级赠送积分规则
        /// </summary>
        DeleteUpgradeBonusPointsRule,
        #endregion

        #region 积分上限规则
        /// <summary>
        /// 添加积分上限规则
        /// </summary>
        AddPointLimitRule,
        /// <summary>
        /// 编辑积分上限规则
        /// </summary>
        EditPointLimitRule,
        /// <summary>
        /// 删除积分上限规则
        /// </summary>
        DeletePointLimitRule,
        #endregion

        #region 积分使用日期规则
        /// <summary>
        /// 添加积分使用日期规则
        /// </summary>
        AddPointUseDateRule,
        /// <summary>
        /// 编辑积分使用日期规则
        /// </summary>
        EditPointUseDateRule,
        /// <summary>
        /// 删除积分使用日期规则
        /// </summary>
        DeletePointUseDateRule,
        #endregion

        #region 注册赠送券规则
        /// <summary>
        /// 添加注册赠送券规则
        /// </summary>
        AddRegisterCouponsRule,
        /// <summary>
        /// 编辑注册赠送券规则
        /// </summary>
        EditRegisterCouponsRule,
        /// <summary>
        /// 删除注册赠送券规则
        /// </summary>
        DeleteRegisterCouponsRule,
        #endregion
    }
}
