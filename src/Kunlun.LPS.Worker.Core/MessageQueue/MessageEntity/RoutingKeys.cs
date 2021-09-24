namespace Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity
{
    public static class RoutingKeys
    {
        /// <summary>
        /// 测试
        /// </summary>
        public const string Test = nameof(Test);

        /// <summary>
        /// 新发卡
        /// </summary>
        public const string Profile_MembershipCardBind = "Profile.MembershipCardBind";

        /// <summary>
        /// 储值
        /// </summary>
        public const string StoredValue_Topup = "StoredValue.Topup";

        /// <summary>
        /// 储值退费
        /// </summary>
        public const string StoredValue_TopupRefund = "StoredValue.TopupRefund";

        /// <summary>
        /// 卡值支付
        /// </summary>
        public const string StoredValue_Payment = "StoredValue.Payment";

        /// <summary>
        /// 取消卡值支付
        /// </summary>
        public const string StoredValue_PaymentRefund = "StoredValue.PaymentRefund";

        /// <summary>
        /// 调整卡值
        /// </summary>
        public const string StoredValue_Adjust = "StoredValue.Adjust";

        /// <summary>
        /// 销卡
        /// </summary>
        public const string MembershipCard_Cancelled = "MembershipCard.Cancelled";

        /// <summary>
        /// 注册
        /// </summary>
        public const string ProfileRegister = "Profile.Register";

        /// <summary>
        /// 新卡绑定
        /// </summary>
        public const string NewCard_Bind = "NewCard.Bind";

        /// <summary>
        /// 修改会员密码
        /// </summary>
        public const string Profile_PwdUpdate = "Profile.PwdUpdate";

        /// <summary>
        /// 修改会员信息
        /// </summary>
        public const string Profile_Update = "Profile.Update";

        /// <summary>
        /// 礼品卡充值
        /// </summary>
        public const string GiftCard_Recharge = "GiftCard.Recharge";

        /// <summary>
        /// 赠送卡值充值
        /// </summary>
        public const string StoredValue_CardValueAwardTopup = "StoredValue.CardValueAwardTopup";

        /// <summary>
        /// 赠送退费
        /// </summary>
        public const string StoredValue_CardValueAwardCancel = "StoredValue.CardValueAwardCancel";

        /// <summary>
        /// 卡值转移源
        /// </summary>
        public const string StoredValue_SourceTransfer = "StoredValue.SourceTransfer";

        /// <summary>
        /// 卡值转移目标卡
        /// </summary>
        public const string StoredValue_ToTransfer = "StoredValue.ToTransfer";

        /// <summary>
        /// 初始化卡值
        /// </summary>
        public const string StoredValue_InitCardFee = "StoredValue.InitCardFee";

        /// <summary>
        /// 合并档案
        /// </summary>
        public const string StoredValue_MergeMembershipCard = "StoredValue.MergeMembershipCard";

        public const string StoredValue_Change = "StoredValue.Change";

        public const string PointsValue_Change = "PointsValue.Change";

        /// <summary>
        /// 卡级别变更
        /// </summary>
        public const string MembershipcardLevel_Update = "MembershipcardLevel.Update";

        /// <summary>
        /// 积分支付
        /// </summary>
        public const string Points_Payment = "Points.Payment";

        /// <summary>
        /// 积分取消支付
        /// </summary>
        public const string Points_PaymentCancel = "Points.PaymentCancel";

        /// <summary>
        /// 调整积分
        /// </summary>
        public const string Points_Adjust = "Points.Adjust";

        /// <summary>
        /// 批量调整积分
        /// </summary>
        public const string Points_BatchAdjust = "Points.BatchAdjust";

        /// <summary>
        /// 添加积分
        /// </summary>
        public const string Points_AddPoints = "Points.AddPoints";

        /// <summary>
        /// 积分转移源
        /// </summary>
        public const string Points_SourceTransfer = "Points.SourceTransfer";

        /// <summary>
        /// 积分转移目标卡
        /// </summary>
        public const string Points_ToTransfer = "Points.ToTransfer";

        /// <summary>
        /// 修改会员手机号
        /// </summary>
        public const string Profile_ProfilePhoneNumberUpdate = "Profile.ProfilePhoneNumberUpdate";




        /// <summary>
        /// 修改定义（包含：添加、修改、删除各种字典数据、卡类型、卡级别、各种规则等定义）
        /// </summary>
        public const string Definition_Modify = "Definition.Modify";

        /// <summary>
        /// 刷新所有缓存
        /// </summary>
        public const string Cache_RefreshALL = "Cache.RefreshAll";

        /// <summary>
        /// 数据迁移
        /// </summary>
        public const string Data_Migration = "Data.Migration";

        /// <summary>
        /// 消费
        /// </summary>
        public const string Consume_New = "Consume.New";

        /// <summary>
        /// 储值送积分成长值
        /// </summary>
        //public const string StoredValue_TopupPointsGrowth = "StoredValue.TopupPointsGrowth";

        /// <summary>
        /// 活动奖励积分
        /// </summary>
        public const string Points_ActivityBonus = "Points.ActivityBonus";

        /// <summary>
        ///升级送积分
        /// </summary>
        public const string MembershipCardLevelChange_Upgrade = "MembershipCardLevelChange.Upgrade";

        /// <summary>
        /// 签到积分
        /// </summary>
        public const string Points_DailyCheckIn = "Points.DailyCheckIn";


        /// <summary>
        /// 签到奖励积分
        /// </summary>
        public const string Points_DailyCheckInBonus = "Points.DailyCheckInBonus";

        /// <summary>
        /// 导入消费
        /// </summary>
        public const string Import_Consume = "Import.Consume";

        /// <summary>
        /// 重算积分
        /// </summary>
        public const string Points_Reacl = "Points.Reacl";

        /// <summary>
        /// 储值赠送券
        /// </summary>
        public const string Coupon_TopupGfit = "Coupon.TopupGfit";

        #region 券
        /// <summary>
        /// 兑换券
        /// </summary>
        public const string Coupon_Exchange = "Coupon.Exchange";

        /// <summary>
        /// 取消兑换券
        /// </summary>
        public const string Coupon_CancelExchange = "Coupon.CancelExchange";

        /// <summary>
        /// 使用券
        /// </summary>
        public const string Coupon_Use = "Coupon.Use";

        /// <summary>
        /// 取消使用券
        /// </summary>
        public const string Coupon_CancelUse = "Coupon.CancelUse";

        /// <summary>
        /// 赠送券
        /// </summary>
        public const string Coupon_Gift = "Coupon.Gift";

        /// <summary>
        /// 会员间赠送券，赠送源
        /// </summary>
        public const string Coupon_GiftSource = "Coupon.GiftSource";

        /// <summary>
        /// 会员间赠送全，赠送目标
        /// </summary>
        public const string Coupon_GiftTarget = "Coupon.GiftTarget";

        /// <summary>
        /// 注册赠送券
        /// </summary>
        public const string Coupon_RegisterGift = "Coupon.RegisterGift";
        
        /// <summary>
        /// 修改券库存
        /// </summary>
        public const string Coupon_UpdateInventory = "Coupon.UpdateInventory";

        /// <summary>
        /// 批量赠送券
        /// </summary>
        public const string Coupon_BatchGift = "Coupon.BatchGift";
        #endregion

        #region 注册

        /// <summary>
        /// 注册
        /// </summary>
        public const string Profile_Register = "Profile.Register";

        /// <summary>
        /// 注册(指定密码)
        /// </summary>
        public const string Profile_RegisterEncryptedPassword = "Profile.RegisterEncryptedPassword";

        public const string Profile_ResetPassword = "Profile.ResetPassword";

        #endregion

        /// <summary>
        /// 会员卡级别变更
        /// </summary>
        //public const string MembershipCard_LPSChangeLevel = "MembershipCard.LPSChangeLevel";

        /// <summary>
        /// 会员卡级别变更(守护程序/LPS)
        /// </summary>
        public const string MembershipCard_LevelChange = "MembershipCard.LevelChange";

        /// <summary>
        /// 卡级别变化 外发微信
        /// </summary>
        public const string MembershipCard_ChangeLevel = "MembershipCard.ChangeLevel";

        /// 合并卡
        /// </summary>
        public const string Profile_Merge = "Profile.Merge";

        /// <summary>
        /// 券变更外发通知
        /// </summary>
        public const string Coupon_Change = "Coupon.Change";
        #region 守护程序 积分

        public const string Points_Room = "Points.Room";

        public const string Points_Fb = "Points.Fb";

        public const string Points_B = "Points.B";

        public const string Points_I = "Points.I";

        public const string Points_Registered = "Points.Registered";

        public const string Points_Expired = "Points.Expired";

        public const string Points_GuestLedger = "Points.GuestLedger";

        public const string Points_RoomPromotion = "Points.RoomPromotion";

        public const string Points_FbPromotion = "Points.FbPromotion";

        public const string Points_Banquet = "Points.Banquet";

        public const string Points_BanquetPromotion = "Points.BanquetPromotion";

        public const string Points_BanquetGuestLedger = "Points.BanquetGuestLedger";

        public const string Points_Bonus = "Points.Bonus";

        public const string Points_BindingExternal = "Points.BindingExternal";

        public const string Points_Multi = "Points.Multi";

        public const string Points_MultiPromotion = "Points.MultiPromotion";

        #endregion

        public const string Fb_ConsumeGiftCoupons = "Fb.ConsumeGiftCoupons";

        public const string Multi_ConsumeGiftCoupons = "Multi.ConsumeGiftCoupons";
        /// <summary>
 	    /// Worker批量赠送券
 	    /// </summary>
 	    public const string Coupon_WorkerBatchGift = "Coupon.WorkerBatchGift";
    }
}
