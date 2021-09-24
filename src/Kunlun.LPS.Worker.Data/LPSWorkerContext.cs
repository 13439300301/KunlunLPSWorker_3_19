using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.Domain.ConsumeGiftCoupons;
using Kunlun.LPS.Worker.Core.Domain.CustomDateCouponRule;
using Kunlun.LPS.Worker.Core.Domain.CustomDateIntegralRule;
using Kunlun.LPS.Worker.Core.Domain.DivisionCards;
using Kunlun.LPS.Worker.Core.Domain.RoomCouponRules;
using Kunlun.LPS.Worker.Core.Domain.RoomPointsRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MembershipCardType = Kunlun.LPS.Worker.Core.Domain.Configurations.MembershipCardType;

namespace Kunlun.LPS.Worker.Data
{
    public class LPSWorkerContext : DbContext
    {
        public LPSWorkerContext(DbContextOptions<LPSWorkerContext> options) : base(options) { }

        #region Configurations 字典数据
        public DbSet<FbItem> FbItem { get; set; }

        public DbSet<Place> Place { get; set; }

        public DbSet<Title> Title { get; set; }

        public DbSet<Gender> Gender { get; set; }

        public DbSet<Hotel> Hotel { get; set; }

        public DbSet<Sysparam> Sysparam { get; set; }
        
        public DbSet<MembershipCardType> MembershipCardType { get; set; }

        public DbSet<MembershipCardAccount> MembershipCardAccount { get; set; }

        public DbSet<LocaleStringResource> LocaleStringResource { get; set; }

        public DbSet<MealPeriod> MealPeriod { get; set; }

        public DbSet<PlaceM> PlaceM { get; set; }

        public DbSet<RegisterCouponsRule> RegisterCouponsRule { get; set; }

        public DbSet<RegisterCouponsRuleMemberSourceMap> RegisterCouponsRuleMemberSourceMap { get; set; }

        public DbSet<DicIdType> DicIdType { get; set; }

        public DbSet<DicProvince> DicProvince { get; set; }

        public DbSet<DicSourceSvc> DicSourceSvc { get; set; }

        public DbSet<DicHistoryType> DicHistoryType { get; set; }

        public DbSet<PointsValidPeriodAdvanceSendInfo> PointsValidPeriodAdvanceSendInfo { get; set; }

        #endregion

        public DbSet<Profile> Profile { get; set; }

        public DbSet<MembershipCard> MembershipCard { get; set; }

        public DbSet<SendInfo> SendInfo { get; set; }

        public DbSet<SendInfoTemplet> SendInfoTemplet { get; set; }

        public DbSet<CustomEvent> CustomEvent { get; set; }

        public DbSet<CustomEventDetail> CustomEventDetail { get; set; }

        public DbSet<ConsumeHistory> ConsumeHistory { get; set; }

        public DbSet<ConsumeHistoryMetadata> ConsumeHistoryMetadata { get; set; }

        public DbSet<Account> Account { get; set; }

        public DbSet<GrowthAccountHistory> GrowthAccountHistory { get; set; }

        public DbSet<MembershipCardTransaction> MembershipCardTransaction { get; set; }

        public DbSet<PointsAccountHistory> PointsAccountHistory { get; set; }

        public DbSet<StoredValueAccountHistory> StoredValueAccountHistory { get; set; }

        public DbSet<Transaction> Transaction { get; set; }

        public DbSet<RechargeActivityRule> RechargeActivityRule { get; set; }

        public DbSet<UpgradeBonusPointsRule> UpgradeBonusPointsRule { get; set; }

        public DbSet<DivisionCard> DivisionCard { get; set; }

        public DbSet<DivisionCardFeeHistory> DivisionCardFeeHistory { get; set; }

        public DbSet<FailCardTransaction> FailCardTransaction { get; set; }

        public DbSet<ConsumeHistoryDetail> ConsumeHistoryDetail { get; set; }

        public DbSet<UserNotification> UserNotification { get; set; }

        public DbSet<ProfileExternalMembership> ProfileExternalMembership { get; set; }

        public DbSet<ExternalMembershipProvider> ExternalMembershipProvider { get; set; }      

        public DbSet<MembershipCardBalanceNotification> MembershipCardBalanceNotification { get; set; }

        public DbSet<MembershipCardBalanceNotificationDetail> MembershipCardBalanceNotificationDetail { get; set; }

        public DbSet<MembershipCardLevel> MembershipCardLevel { get; set; }

        public DbSet<RechargeNoActivityPlaceDetail> RechargeNoActivityPlaceDetail { get; set; }

        public DbSet<Coupon> Coupon { get; set; }

        public DbSet<CouponType> CouponType { get; set; }

        public DbSet<CouponInventory> CouponInventory { get; set; }

        public DbSet<CouponTypePaymentWay> CouponTypePaymentWay { get; set; }

        public DbSet<CouponTypePaymentWay_Map> CouponTypePaymentWay_Map { get; set; }
        public DbSet<CouponPackageTypeDetail> CouponPackageTypeDetail { get; set; }

        public DbSet<CouponTransactionHistory> CouponTransactionHistory { get; set; }

        public DbSet<RegisterPointsRule> RegisterPointsRule { get; set; }

        public DbSet<RegisterPointsRuleMemberSourceMap> RegisterPointsRuleMemberSourceMap { get; set; }

        public DbSet<RegisterPointsRuleMembershipCardLevelDetail> RegisterPointsRuleMembershipCardLevelDetail { get; set; }

        public DbSet<ProfileIdDetail> ProfileIdDetail { get; set; }

        public DbSet<StoredValuePaymentPointsRule> StoredValuePaymentPointsRule { get; set; }


        public DbSet<StoredValuePaymentPointsRulePlaceMap> StoredValuePaymentPointsRulePlaceMap { get; set; }

        public DbSet<PointsHistoryDetail> PointsHistoryDetail { get; set; }

        public DbSet<PointsAccrueType> PointsAccrueType { get; set; }

        public DbSet<CustInfo> CustInfo { get; set; }

        public DbSet<PointsValidPeriodRule> PointsValidPeriodRule { get; set; }

        public DbSet<UserMetadata> UserMetadata { get; set; }

        public DbSet<RegularValueStorageRule> RegularValueStorageRule { get; set; }

        public DbSet<RoomFirstStayGiftPoint> RoomFirstStayGiftPoint { get; set; }

        public DbSet<RoomFirstStayGiftPointForBucket> RoomFirstStayGiftPointForBucket { get; set; }

        public DbSet<RoomPointsRuleBucketMap> RoomPointsRuleBucketMap { get; set; }

        public DbSet<RoomPointsRuleChannel> RoomPointsRuleChannel { get; set; }

        public DbSet<RoomPointsRuleForBucketMembershipCardLevel> RoomPointsRuleForBucketMembershipCardLevel { get; set; }

        public DbSet<RoomPointsRuleHotel> RoomPointsRuleHotel { get; set; }

        public DbSet<RoomPointsRuleHotelRoomType> RoomPointsRuleHotelRoomType { get; set; }

        public DbSet<RoomPointsRuleMarket> RoomPointsRuleMarket { get; set; }

        public DbSet<RoomPointsRuleMembershipCardLevel> RoomPointsRuleMembershipCardLevel { get; set; }

        public DbSet<RoomPointsRulePayment> RoomPointsRulePayment { get; set; }

        public DbSet<RoomPointsRuleRateTemplate> RoomPointsRuleRateTemplate { get; set; }

        public DbSet<RoomPointsRules> RoomPointsRules { get; set; }
        
        public DbSet<RechargeAmountUpgradeRule> RechargeAmountUpgradeRule { get; set; } 
        public DbSet<RechargeAmountUpgradeRuleDetail> RechargeAmountUpgradeRuleDetail { get; set; }
        public DbSet<MembershipCardLevelChangeHistory> MembershipCardLevelChangeHistory { get; set; }
        public DbSet<UpgradeAndDowngradeRule> UpgradeAndDowngradeRule { get; set; }
        public DbSet<LPSLog> LPSLog { get; set; }

        public DbSet<RechargeActivityRuleCouponsDetail> RechargeActivityRuleCouponsDetail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LPSWorkerContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<FbConsumeGiftCouponsRule> FbConsumeGiftCouponsRule { get; set; }

        public DbSet<FbConsumeGiftCouponsRuleMemberSourceMap> FbConsumeGiftCouponsRuleMemberSourceMap { get; set; }

        public DbSet<FbConsumeGiftCouponsRulePaymentMap> FbConsumeGiftCouponsRulePaymentMap { get; set; }
        public DbSet<ProfileMobilePhoneNumberDetail> ProfileMobilePhoneNumberDetail { get; set; }
        public DbSet<ExternalMembershipLevel> ExternalMembershipLevel { get; set; }
        public DbSet<ExternalMembershipLevelMembershipCardLevelMap> ExternalMembershipLevelMembershipCardLevelMap { get; set; }

        public DbSet<FbConsumeGiftCouponsRulePlaceMap> FbConsumeGiftCouponsRulePlaceMap { get; set; }

        public DbSet<FbConsumeGiftCouponsRuleCouponsDetail> FbConsumeGiftCouponsRuleCouponsDetail { get; set; }

        public DbSet<MultiConsumeGiftCouponsRule> MultiConsumeGiftCouponsRule { get; set; }

        public DbSet<MultiConsumeGiftCouponsRuleMemberSourceMap> MultiConsumeGiftCouponsRuleMemberSourceMap { get; set; }

        public DbSet<MultiConsumeGiftCouponsRulePaymentMap> MultiConsumeGiftCouponsRulePaymentMap { get; set; }

        public DbSet<MultiConsumeGiftCouponsRulePlaceMap> MultiConsumeGiftCouponsRulePlaceMap { get; set; }

        public DbSet<MultiConsumeGiftCouponsRuleCouponsDetail> MultiConsumeGiftCouponsRuleCouponsDetail { get; set; }

        public DbSet<MultiConsumeGiftCouponsRuleConsumeTypeMap> MultiConsumeGiftCouponsRuleConsumeTypeMap { get; set; }

        public DbSet<RoomCouponsRule> RoomCouponsRule { get; set; }
        public DbSet<RoomCouponsRulePayment> RoomCouponsRulePayment { get; set; }

        public DbSet<RoomCouponsRuleChannel> RoomCouponsRuleChannel { get; set; }
        public DbSet<RoomCouponsRuleCouponsDetail> RoomCouponsRuleCouponsDetail { get; set; }
        public DbSet<RoomCouponsRuleMarket> RoomCouponsRuleMarket { get; set; }

        public DbSet<RoomCouponsRuleMemberSource> RoomCouponsRuleMemberSource { get; set; }

        public DbSet<RoomCouponsRuleRate> RoomCouponsRuleRate { get; set; }

        public DbSet<CustomDateIntegralRules> CustomDateIntegralRules { get; set; }
        public DbSet<CustomDateIntegralRuleMemberSource> CustomDateIntegralRuleMemberSource { get; set; }

        public DbSet<CustomDateCouponRules> CustomDateCouponRules { get; set; }

        public DbSet<CustomDateCouponRulesCouponsDetail> CustomDateCouponRulesCouponsDetail { get; set; }

        public DbSet<CustomDateCouponRulesMemberSource> CustomDateCouponRulesMemberSource { get; set; }


    }
}
