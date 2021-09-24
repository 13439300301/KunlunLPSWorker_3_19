using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    public class SendInfoTempletService : ISendInfoTempletService
    {
        private readonly ILogger<SendInfoTempletService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IConfigurationService<Place> _placeService;
        private readonly IConfigurationService<Hotel> _hotelService;
        private readonly IConfigurationService<Core.Domain.Configurations.MembershipCardType> _membershipCardTypeService;
        private readonly IGetOrUpdateInfoFromRedisService _redisService;
        private readonly IConfigurationService<LocaleStringResource> _localeStringResourceService;
        private readonly IConfigurationService<MembershipCardLevel> _membershipCardLevelService;
        private readonly IConfigurationService<DicIdType> _dicIdTypeService;
        private readonly IConfigurationService<DicProvince> _dicProvinceService;
        private readonly IConfigurationService<DicSourceSvc> _dicSourceSvcService;

        public SendInfoTempletService(
            ILogger<SendInfoTempletService> logger,
            LPSWorkerContext context,
            IConfigurationService<Place> placeService,
            IConfigurationService<Hotel> hotelService,
            IConfigurationService<Core.Domain.Configurations.MembershipCardType> membershipCardTypeService,
            IGetOrUpdateInfoFromRedisService redisService,
            IConfigurationService<LocaleStringResource> localeStringResourceService,
            IConfigurationService<MembershipCardLevel> membershipCardLevelService,
            IConfigurationService<DicIdType> dicIdTypeService,
            IConfigurationService<DicProvince> dicProvinceService,
            IConfigurationService<DicSourceSvc> dicSourceSvcService
            )
        {
            _logger = logger;
            _context = context;
            _placeService = placeService;
            _hotelService = hotelService;
            _membershipCardTypeService = membershipCardTypeService;
            _redisService = redisService;
            _localeStringResourceService = localeStringResourceService;
            _membershipCardLevelService = membershipCardLevelService;
            _dicIdTypeService = dicIdTypeService;
            _dicProvinceService = dicProvinceService;
            _dicSourceSvcService = dicSourceSvcService;

            _logger.LogInformation(nameof(SendInfoTempletService));
        }
        public string GetSendInfoContent(string content, MembershipCard membershipCard)
        {
            if (content.Contains("[CARD_NO]"))
            {
                content = content.Replace("[CARD_NO]", membershipCard.MembershipCardNumber);
            }
            if (content.Contains("[CARD_FACE_NUMBER]"))
            {
                content = content.Replace("[CARD_FACE_NUMBER]", membershipCard.CardFaceNumber);
            }
            if (content.Contains("[FULL_CARD_NO]"))
            {
                content = content.Replace("[FULL_CARD_NO]", membershipCard.MembershipCardNumber);
            }
            if (content.Contains("[X_CARD_NO]"))
            {
                content = content.Replace("[X_CARD_NO]", "**" + membershipCard.MembershipCardNumber.Substring(2, membershipCard.MembershipCardNumber.Length - 2));
            }
            if (content.Contains("[CARD_NO_X]"))
            {
                content = content.Replace("[CARD_NO_X]", membershipCard.MembershipCardNumber.Substring(0, membershipCard.MembershipCardNumber.Length - 2) + "**");
            }
            if (content.Contains("[X_CARD_NO_X]"))
            {
                content = content.Replace("[X_CARD_NO_X]", "**" + membershipCard.MembershipCardNumber.Substring(2, membershipCard.MembershipCardNumber.Length - 4) + "**");
            }
            if (content.Contains("[MEMBERSHIP_TYPE]"))
            {
                var membershipCardType = _membershipCardTypeService.GetAllFromCache().Where(t => t.Id == membershipCard.MembershipCardTypeId).FirstOrDefault();
                content = content.Replace("[MEMBERSHIP_TYPE]", membershipCardType.Name);
            }
            if (content.Contains("[POINTS]") || content.Contains("[POINTS_DECIMAL]") || content.Contains("[MEMBER_POINTS]") || content.Contains("[MEMBER_POINTS_INT]"))
            {
                decimal points = 0;
                var pointsResult = _redisService.GetRedisCardTotalAmount(membershipCard.Id, RedisLuaScript.ACCOUNT_TYPE_POINTS);
                if (!(pointsResult == null || pointsResult.Length == 0 || pointsResult[0] == null))
                {
                    points = Convert.ToDecimal(pointsResult[0]);
                }
                content = content.Replace("[POINTS]", Convert.ToString(Math.Round(points, 0, MidpointRounding.AwayFromZero)));
                content = content.Replace("[POINTS_DECIMAL]", points.ToString("0.00"));
                content = content.Replace("[MEMBER_POINTS_INT]", Convert.ToString(Math.Round(points, 0, MidpointRounding.AwayFromZero)));
                content = content.Replace("[MEMBER_POINTS]", points.ToString("0.00"));
            }
            if (content.Contains("[BALANCE]"))
            {
                decimal storeValue = 0;
                var storeValueResult = _redisService.GetRedisCardTotalAmount(membershipCard.Id, RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE);
                if (!(storeValueResult == null || storeValueResult.Length == 0 || storeValueResult[0] == null))
                {
                    storeValue = Convert.ToDecimal(storeValueResult[0]);
                }
                content = content.Replace("[BALANCE]", Convert.ToString(Math.Round(storeValue, 2, MidpointRounding.AwayFromZero)));
            }
            if (content.Contains("[ENROLL_DATE]") && membershipCard.BindingDate.HasValue)
            {
                content = content.Replace("[ENROLL_DATE]", membershipCard.BindingDate.Value.ToString("yyyy-MM-dd"));
            }
            if (content.Contains("[EXPIRY_DATE]") && membershipCard.ExpirationDate.HasValue)
            {
                content = content.Replace("[EXPIRY_DATE]", membershipCard.ExpirationDate.Value.ToString("yyyy-MM-dd"));
            }
            if (content.Contains("[RETENTIONPERIOD_DATE]") && membershipCard.ExpirationDate.HasValue)
            {
                content = content.Replace("[RETENTIONPERIOD_DATE]", membershipCard.ExpirationDate.Value.ToString("yyyy-MM-dd"));
            }
            if (content.Contains("[CARD_LEVEL]"))
            {
                //var levelName=_context.MembershipCardLevel.AsNoTracking().FirstOrDefault(l=>l.Id==membershipCard.MembershipCardLevelId).Name;
                var levelName = _membershipCardLevelService.GetAllFromCache().Where(l => l.Id == membershipCard.MembershipCardLevelId).FirstOrDefault().Name;
                content = content.Replace("[CARD_LEVEL]", levelName);
            }
            if (content.Contains("[MEMBER_SOURCE]"))
            {
                var membercode = _dicSourceSvcService.GetAllFromCache().Where(s => s.Code == membershipCard.MemberSourceCode).FirstOrDefault();
                if (membercode != null)
                {
                    content = content.Replace("[MEMBER_SOURCE]", membercode.Name);
                }

            }

            return content;
        }
        public string GetSendInfoContent(string content, Profile profile, bool custId = false)
        {
            if (content.Contains("[FULL_NAME]"))
            {
                content = content.Replace("[FULL_NAME]", profile.FullName);
            }
            if (!custId)
            {
                if (content.Contains("[CH_NAME]"))
                {
                    content = content.Replace("[CH_NAME]", profile.LastName + profile.FirstName);
                }
                if (content.Contains("[ENG_NAME]"))
                {
                    content = content.Replace("[ENG_NAME]", profile.AltLastName + profile.AltFirstName);
                }

                if (content.Contains("[LAST_NAME]"))
                {
                    content = content.Replace("[LAST_NAME]", profile.LastName);
                }
            }
            else
            {
                if (content.Contains("[CH_NAME]"))
                {
                    content = content.Replace("[CH_NAME]", "");
                }
                if (content.Contains("[ENG_NAME]"))
                {
                    content = content.Replace("[ENG_NAME]", "");
                }

                if (content.Contains("[LAST_NAME]"))
                {
                    content = content.Replace("[LAST_NAME]", "");
                }
            }
            if (content.Contains("[FIRST_NAME]"))
            {
                content = content.Replace("[FIRST_NAME]", profile.FirstName);
            }

            if (content.Contains("[PROFILE_PASSWORD]"))
            {
                content = content.Replace("[PROFILE_PASSWORD]", "[CARD_PASSWORD]");
            }
            if (profile.Title != null && profile.Title.Name.Contains("|"))
            {
                string ChTitle, EnTitle;
                ChTitle = profile.Title.Name.Trim().Substring(0, profile.Title.Name.IndexOf("|"));
                EnTitle = profile.Title.Name.Trim().Substring(profile.Title.Name.IndexOf("|") + 1, profile.Title.Name.Count() - (profile.Title.Name.IndexOf("|") + 1));
                if (content.Contains("[CH_TITLE]"))
                {
                    content = content.Replace("[CH_TITLE]", ChTitle);
                }
                if (content.Contains("[EN_TITLE]"))
                {
                    content = content.Replace("[EN_TITLE]", EnTitle);
                }
            }
            else
            {
                if (content.Contains("[TITLE]"))
                {
                    content = content.Replace("[TITLE]", profile.Title?.Name);
                }
            }
            if (content.Contains("LINKMAN_TEL"))
            {
                content = content.Replace("[LINKMAN_TEL]", profile.MobilePhoneNumber);
            }
            if (content.Contains("[BIRTHDAY]"))
            {
                content = content.Replace("[BIRTHDAY]", profile.Birthday == null ? null : profile.Birthday.Value.ToString("yyyy-MM-dd"));
            }
            if (content.Contains("[NATIONALITY]"))
            {
                content = content.Replace("[NATIONALITY]", profile.NationalityCode);
            }
            if (content.Contains("[EMAIL]"))
            {
                content = content.Replace("[EMAIL]", profile.Email);
            }
            if (content.Contains("[MOBILE]"))
            {
                content = content.Replace("[MOBILE]", profile.MobilePhoneNumber);
            }
            if (content.Contains("[ZIP]"))
            {
                content = content.Replace("[ZIP]", profile.AddressPostcode);
            }
            if (content.Contains("[ID_TYPE]"))
            {
                content = content.Replace("[ID_TYPE]", _dicIdTypeService.GetAllFromCache().Where(i => i.Code == profile.IdTypeCode).FirstOrDefault().Name);
            }
            if (content.Contains("[ID_NO]"))
            {
                content = content.Replace("[ID_NO]", profile.IdNumber);
            }
            if (content.Contains("[PROVINCE]"))
            {
                if (!String.IsNullOrEmpty(profile.AddressProvinceCode))
                {
                    content = content.Replace("[PROVINCE]", _dicProvinceService.GetAllFromCache().Where(p => p.Code == profile.AddressProvinceCode).FirstOrDefault().Name);
                }
            }
            if (content.Contains("[CITY]"))
            {
                content = content.Replace("[CITY]", profile.AddressCityName);
            }

            return content;
        }

        public string GetSendInfoContent(string content, PointsAccountHistory pointsAccountHistory, string languageCode)
        {
            var hotel = _hotelService.GetAllFromCache().FirstOrDefault(c => c.Code == pointsAccountHistory.HotelCode);
            var place = _placeService.GetAllFromCache().FirstOrDefault(c => c.Code == pointsAccountHistory.PlaceCode);

            if (content.Contains("[OPER_TYPE]"))
            {
                //var key = "Enums.Kunlun.LPS.Core.Enum.TransactionType." + _context.Transaction.AsNoTracking()
                //    .FirstOrDefault(c => c.Id == pointsAccountHistory.TransactionId)?.TransactionType.ToString();
                //var resourceValue = _localeStringResourceService.GetAllFromCache()
                //    .FirstOrDefault(c => c.Code == key && c.LanguageId == languageCode)?.Name;
                //这里取得是调整类型
                var resourceValue = _context.PointsAccrueType.AsNoTracking().FirstOrDefault(t => t.Code == pointsAccountHistory.AccrueType)?.Name;
                content = content.Replace("[OPER_TYPE]", resourceValue);
            }
            if (content.Contains("[OPER_VALUE]") || content.Contains("[OPER_VAlUE]"))
            {
                content = content.Replace("[OPER_VALUE]", Convert.ToString(Math.Round(Math.Abs(pointsAccountHistory.Points), 2, MidpointRounding.AwayFromZero)));
                content = content.Replace("[OPER_VAlUE]", Convert.ToString(Math.Round(Math.Abs(pointsAccountHistory.Points), 2, MidpointRounding.AwayFromZero)));
            }
            if (content.Contains("[DATE]"))
            {
                content = content.Replace("[DATE]", Convert.ToString(pointsAccountHistory.TransactionDate));
            }
            if (content.Contains("[PLACE]"))
            {
                content = content.Replace("[PLACE]", place?.Name);
            }
            if (content.Contains("[CH_HOTEL]"))
            {
                content = content.Replace("[CH_HOTEL]", hotel?.Name);
            }
            if (content.Contains("[ENG_HOTEL]"))
            {
                content = content.Replace("[ENG_HOTEL]", hotel?.EngName);
            }
            if (content.Contains("[DEPOSIT]"))
            {
                content = content.Replace("[DEPOSIT]", "0.00");
            }
            if (content.Contains("[OPERATOR]"))
            {
                content = content.Replace("[OPERATOR]", pointsAccountHistory.InsertUser);
            }
            return content;
        }

        public string GetSendInfoContent(string content, Transaction transaction, string languageCode)
        {
            var hotel = _hotelService.GetAllFromCache().FirstOrDefault(c => c.Code == transaction.HotelCode);
            var place = _placeService.GetAllFromCache().FirstOrDefault(c => c.Code == transaction.PlaceCode);

            if (content.Contains("[OPER_TYPE]"))
            {
                var key = "Enums.Kunlun.LPS.Core.Enum.TransactionType." + _context.Transaction.AsNoTracking()
                    .FirstOrDefault(c => c.Id == transaction.Id)?.TransactionType.ToString();
                var resourceValue = _localeStringResourceService.GetAllFromCache()
                    .FirstOrDefault(c => c.Code == key && c.LanguageId == languageCode)?.Name;
                content = content.Replace("[OPER_TYPE]", resourceValue);
            }
            if (content.Contains("[OPER_VALUE]") || content.Contains("[OPER_VAlUE]"))
            {
                content = content.Replace("[OPER_VAlUE]", Convert.ToString(Math.Round(Math.Abs(transaction.Amount), 2, MidpointRounding.AwayFromZero)));
                content = content.Replace("[OPER_VALUE]", Convert.ToString(Math.Round(Math.Abs(transaction.Amount), 2, MidpointRounding.AwayFromZero)));

            }
            if (content.Contains("[REAL_OPER_VAlUE]") || content.Contains("[REAL_OPER_VALUE]"))
            {
                if (content.Contains("[DEPOSIT_POINTS]"))
                {
                    content = content.Replace("[REAL_OPER_VAlUE]", transaction.Amount.ToString("0"));
                    content = content.Replace("[REAL_OPER_VALUE]", transaction.Amount.ToString("0"));
                }
                else if (content.Contains("[DEPOSIT_POINTS_DECIMAL]"))
                {
                    content = content.Replace("[REAL_OPER_VAlUE]", transaction.Amount.ToString("0.00"));
                    content = content.Replace("[REAL_OPER_VALUE]", transaction.Amount.ToString("0.00"));
                }
                else
                {
                    content = content.Replace("[REAL_OPER_VAlUE]", transaction.Amount.ToString("0.00"));
                    content = content.Replace("[REAL_OPER_VALUE]", transaction.Amount.ToString("0.00"));
                }
            }
            if (content.Contains("[DATE]"))
            {
                content = content.Replace("[DATE]", Convert.ToString(transaction.TransactionDate));
            }
            if (content.Contains("[PLACE]"))
            {
                content = content.Replace("[PLACE]", place?.Name);
            }
            if (content.Contains("[CH_HOTEL]"))
            {
                content = content.Replace("[CH_HOTEL]", hotel?.Name);
            }
            if (content.Contains("[ENG_HOTEL]"))
            {
                content = content.Replace("[ENG_HOTEL]", hotel?.EngName);
            }
            if (content.Contains("[DEPOSIT]"))
            {
                content = content.Replace("[DEPOSIT]", "0.00");
            }
            if (content.Contains("[OPERATOR]"))
            {
                content = content.Replace("[OPERATOR]", transaction.InsertUser);
            }
            return content;
        }

        public string GetSendInfoContent(string content, MembershipCardBalanceNotification membershipCardBalanceNotification)
        {
            if (membershipCardBalanceNotification.Balance != 0 && content.Contains("[ALERT_BALANCE]"))
            {
                content = content.Replace("[ALERT_BALANCE]", membershipCardBalanceNotification.Balance.ToString("0.00"));
            }
            return content;
        }

        public string GetSendInfoContent(string content, MembershipCard membershipCard, decimal balance)
        {
            if (content.Contains("[CARD_NO]"))
            {
                content = content.Replace("[CARD_NO]", membershipCard.MembershipCardNumber);
            }
            if (content.Contains("[CARD_FACE_NUMBER]"))
            {
                content = content.Replace("[CARD_FACE_NUMBER]", membershipCard.CardFaceNumber);
            }
            if (content.Contains("[FULL_CARD_NO]"))
            {
                content = content.Replace("[FULL_CARD_NO]", membershipCard.MembershipCardNumber);
            }
            if (content.Contains("[X_CARD_NO]"))
            {
                content = content.Replace("[X_CARD_NO]", "**" + membershipCard.MembershipCardNumber.Substring(2, membershipCard.MembershipCardNumber.Length - 2));
            }
            if (content.Contains("[CARD_NO_X]"))
            {
                content = content.Replace("[CARD_NO_X]", membershipCard.MembershipCardNumber.Substring(0, membershipCard.MembershipCardNumber.Length - 2) + "**");
            }
            if (content.Contains("[X_CARD_NO_X]"))
            {
                content = content.Replace("[X_CARD_NO_X]", "**" + membershipCard.MembershipCardNumber.Substring(2, membershipCard.MembershipCardNumber.Length - 4) + "**");
            }
            if (content.Contains("[MEMBERSHIP_TYPE]"))
            {
                var membershipCardType = _membershipCardTypeService.GetAllFromCache().Where(t => t.Id == membershipCard.MembershipCardTypeId).FirstOrDefault();
                content = content.Replace("[MEMBERSHIP_TYPE]", membershipCardType.Name);
            }
            if (content.Contains("[POINTS]") || content.Contains("[POINTS_DECIMAL]"))
            {
                decimal points = 0;
                var pointsResult = _redisService.GetRedisCardTotalAmount(membershipCard.Id, RedisLuaScript.ACCOUNT_TYPE_POINTS);
                if (!(pointsResult == null || pointsResult.Length == 0 || pointsResult[0] == null))
                {
                    points = Convert.ToDecimal(pointsResult[0]);
                }
                content = content.Replace("[POINTS]", Convert.ToString(Math.Round(points, 0, MidpointRounding.AwayFromZero)));
                content = content.Replace("[POINTS_DECIMAL]", points.ToString("0.00"));
            }
            if (content.Contains("[BALANCE]"))
            {
                //decimal storeValue = 0;
                //var storeValueResult = _redisService.GetRedisCardTotalAmount(membershipCard.Id, RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE);
                //if (!(storeValueResult == null || storeValueResult.Length == 0 || storeValueResult[0] == null))
                //{
                //    storeValue = Convert.ToDecimal(storeValueResult[0]);
                //}
                content = content.Replace("[BALANCE]", Convert.ToString(Math.Round(balance, 2, MidpointRounding.AwayFromZero)));
            }
            if (content.Contains("[ENROLL_DATE]") && membershipCard.BindingDate.HasValue)
            {
                content = content.Replace("[ENROLL_DATE]", membershipCard.BindingDate.Value.ToString("yyyy-MM-dd"));
            }
            if (content.Contains("[EXPIRY_DATE]") && membershipCard.ExpirationDate.HasValue)
            {
                content = content.Replace("[EXPIRY_DATE]", membershipCard.ExpirationDate.Value.ToString("yyyy-MM-dd"));
            }
            if (content.Contains("[CARD_LEVEL]"))
            {
                //var levelName=_context.MembershipCardLevel.AsNoTracking().FirstOrDefault(l=>l.Id==membershipCard.MembershipCardLevelId).Name;
                var levelName = _membershipCardLevelService.GetAllFromCache().Where(l => l.Id == membershipCard.MembershipCardLevelId).FirstOrDefault().Name;
                content = content.Replace("[CARD_LEVEL]", levelName);
            }
            return content;
        }

        public string GetSendInfoContent(string content, Transaction transaction, string languageCode, bool isPoints)
        {
            var hotel = _hotelService.GetAllFromCache().FirstOrDefault(c => c.Code == transaction.HotelCode);
            var place = _placeService.GetAllFromCache().FirstOrDefault(c => c.Code == transaction.PlaceCode);

            if (content.Contains("[OPER_TYPE]"))
            {
                var key = "Enums.Kunlun.LPS.Core.Enum.TransactionType." + _context.Transaction.AsNoTracking()
                    .FirstOrDefault(c => c.Id == transaction.Id)?.TransactionType.ToString();
                var resourceValue = _localeStringResourceService.GetAllFromCache()
                    .FirstOrDefault(c => c.Code == key && c.LanguageId == languageCode)?.Name;
                content = content.Replace("[OPER_TYPE]", resourceValue);
            }

            if (content.Contains("[OPER_VALUE]") || content.Contains("[OPER_VAlUE]"))
            {
                content = content.Replace("[OPER_VAlUE]",
                    Convert.ToString(Math.Round(Math.Abs(isPoints ? transaction.Points : transaction.Amount), 2, MidpointRounding.AwayFromZero)));
                content = content.Replace("[OPER_VALUE]",
                    Convert.ToString(Math.Round(Math.Abs(isPoints ? transaction.Points : transaction.Amount), 2, MidpointRounding.AwayFromZero)));

            }
            if (content.Contains("[REAL_OPER_VAlUE]") || content.Contains("[REAL_OPER_VALUE]"))
            {
                if (content.Contains("[DEPOSIT_POINTS]"))
                {
                    content = content.Replace("[REAL_OPER_VAlUE]", isPoints ? transaction.Points.ToString("0") : transaction.Amount.ToString("0"));
                    content = content.Replace("[REAL_OPER_VALUE]", isPoints ? transaction.Points.ToString("0") : transaction.Amount.ToString("0"));
                }
                else if (content.Contains("[DEPOSIT_POINTS_DECIMAL]"))
                {
                    content = content.Replace("[REAL_OPER_VAlUE]", isPoints ? transaction.Points.ToString("0") : transaction.Amount.ToString("0.00"));
                    content = content.Replace("[REAL_OPER_VALUE]", isPoints ? transaction.Points.ToString("0") : transaction.Amount.ToString("0.00"));
                }
                else
                {
                    content = content.Replace("[REAL_OPER_VAlUE]", isPoints ? transaction.Points.ToString("0") : transaction.Amount.ToString("0.00"));
                    content = content.Replace("[REAL_OPER_VALUE]", isPoints ? transaction.Points.ToString("0") : transaction.Amount.ToString("0.00"));
                }
            }
            if (content.Contains("[DATE]"))
            {
                content = content.Replace("[DATE]", Convert.ToString(transaction.TransactionDate));
            }
            if (content.Contains("[PLACE]"))
            {
                content = content.Replace("[PLACE]", place?.Name);
            }
            if (content.Contains("[CH_HOTEL]"))
            {
                content = content.Replace("[CH_HOTEL]", hotel?.Name);
            }
            if (content.Contains("[ENG_HOTEL]"))
            {
                content = content.Replace("[ENG_HOTEL]", hotel?.EngName);
            }
            if (content.Contains("[DEPOSIT]"))
            {
                content = content.Replace("[DEPOSIT]", "0.00");
            }
            if (content.Contains("[OPERATOR]"))
            {
                content = content.Replace("[OPERATOR]", transaction.InsertUser);
            }
            return content;
        }

        public string GetSendInfoContent(string content, List<CouponTransactionHistory> list, DateTime date, string languageCode)
        {
            var couponTransactionHistory = list.FirstOrDefault();
            var coupon = _context.Coupon.AsNoTracking().Where(c => c.OwnerId == couponTransactionHistory.ProfileId).ToList();

            //获得兑换券后的状态  LPS_Coupon.Status
            if (content.Contains("[EXCHANGE_COUPON_STAYUS]"))
            {
                content = content.Replace("[EXCHANGE_COUPON_STAYUS]", GetLocaleStringResource("LPS.CouponShownModel.IsNotUsed", languageCode));
            }
            //获得兑换券类型(EXCHANGE/TOPUP_BONUS/PROMOTION_BONUS/OTHER) 现在只有积分兑换券
            if (content.Contains("[EXCHANGE_ACCRUE_TYPE]"))
            {
                var key = "Enums.Kunlun.LPS.Core.Enum.TransactionType." + _context.Transaction.AsNoTracking()
                       .FirstOrDefault(c => c.Id == couponTransactionHistory.TransactionId)?.TransactionType.ToString();
                content = content.Replace("[EXCHANGE_ACCRUE_TYPE]", GetLocaleStringResource(key, languageCode));
            }
            if (content.Contains("[EXCHANGE_COUPONS_ISNOTUSED]") || content.Contains("[EXCHANGE_COUPONS_ISUSED]"))
            {
                //兑换券没有使用的多少
                var isNotUsedAmount = coupon.Where(c => !c.IsUsed).Count();
                content = content.Replace("[EXCHANGE_COUPONS_ISNOTUSED]", isNotUsedAmount.ToString());

                //兑换券使用了多少
                var isUsedAmount = coupon.Where(c => c.IsUsed).Count();
                content = content.Replace("[EXCHANGE_COUPONS_ISUSED]", isUsedAmount.ToString());
            }
            string detail = "";

            if (content.IndexOf("[EXCHANGE_BEGIN]") != -1 && content.IndexOf("[EXCHANGE_END]") != -1)
            {
                var couponTypeIdList = list.GroupBy(c => c.CouponTypeId).Select(s => new { s.Key, Count = s.Count() });
                var couponTypeList = _context.CouponType.AsNoTracking().Where(c => couponTypeIdList.Select(c => c.Key).Contains(c.Id)).ToList();

                foreach (var couponType in couponTypeList)
                {
                    var couponDetail = content.Substring(content.IndexOf("[EXCHANGE_BEGIN]") + 16, content.IndexOf("[EXCHANGE_END]") - (content.IndexOf("[EXCHANGE_BEGIN]") + 16));
                    //兑换券交易类型名称
                    if (couponDetail.Contains("[ACCRUE_TYPE_NAME]"))
                    {
                        var mode = "";
                        switch (couponType.ExchangeMode)
                        {
                            case CouponExchangeMode.None:
                                mode = GetLocaleStringResource("Enums.Kunlun.LPS.Core.Enum.CouponExchangeMode.None", languageCode);
                                break;
                            case CouponExchangeMode.Points:
                                mode = GetLocaleStringResource("Enums.Kunlun.LPS.Core.Enum.CouponExchangeMode.Points", languageCode);
                                break;
                            case CouponExchangeMode.CouponPackage:
                                mode = GetLocaleStringResource("Enums.Kunlun.LPS.Core.Enum.CouponExchangeMode.CouponPackage", languageCode);
                                break;
                            case CouponExchangeMode.Privilege:
                                mode = GetLocaleStringResource("Enums.Kunlun.LPS.Core.Enum.CouponExchangeMode.Privilege", languageCode);
                                break;
                            case CouponExchangeMode.Gift:
                                mode = GetLocaleStringResource("Enums.Kunlun.LPS.Core.Enum.CouponExchangeMode.Gift", languageCode);
                                break;
                            case CouponExchangeMode.Sell:
                                mode = GetLocaleStringResource("Enums.Kunlun.LPS.Core.Enum.CouponExchangeMode.Sell", languageCode);
                                break;
                            default:
                                break;
                        }
                        couponDetail = couponDetail.Replace("[ACCRUE_TYPE_NAME]", mode);
                    }
                    //兑换券代码
                    if (couponDetail.Contains("[EXCHANGE_COUPONS_CODE]"))
                    {
                        couponDetail = couponDetail.Replace("[EXCHANGE_COUPONS_CODE]", couponType.Code);
                    }
                    //兑换券名称
                    if (!String.IsNullOrEmpty(couponType.Name) && couponType.Name.Contains("|"))
                    {
                        string ChCouponName, EnCouponName;
                        ChCouponName = couponType.Name.Trim().Substring(0, couponType.Name.IndexOf("|"));
                        EnCouponName = couponType.Name.Trim().Substring(couponType.Name.IndexOf("|") + 1, couponType.Name.Count() - (couponType.Name.IndexOf("|") + 1));
                        if (couponDetail.Contains("[CH_EXCHANGE_COUPONS_NAME]"))
                        {
                            couponDetail = couponDetail.Replace("[CH_EXCHANGE_COUPONS_NAME]", ChCouponName);
                        }
                        if (couponDetail.Contains("[EN_EXCHANGE_COUPONS_NAME]"))
                        {
                            couponDetail = couponDetail.Replace("[EN_EXCHANGE_COUPONS_NAME]", EnCouponName);
                        }
                    }
                    else
                    {
                        if (couponDetail.Contains("[EXCHANGE_COUPONS_NAME]"))
                        {
                            couponDetail = couponDetail.Replace("[EXCHANGE_COUPONS_NAME]", couponType.Name);
                        }
                    }
                    //兑换积分
                    if (couponDetail.Contains("[EXCHANGE_POINTS]") || couponDetail.Contains("[EXCHANGE_POINTS_DECIMAL]"))
                    {
                        couponDetail = couponDetail.Replace("[EXCHANGE_POINTS]", couponType.ExchangeNeedPoints.ToString("0"));
                        couponDetail = couponDetail.Replace("[EXCHANGE_POINTS_DECIMAL]", couponType.ExchangeNeedPoints.ToString("0.00"));
                    }
                    //兑换数量
                    if (couponDetail.Contains("[EXCHANGE_NUM]"))
                    {
                        var exchangeCount = couponTypeIdList.FirstOrDefault(c => c.Key == couponType.Id).Count;
                        couponDetail = couponDetail.Replace("[EXCHANGE_NUM]", exchangeCount.ToString());
                    }
                    //兑换时间
                    if (couponDetail.Contains("[EXCHANGE_DT]"))
                    {
                        couponDetail = couponDetail.Replace("[EXCHANGE_DT]", date.ToString());
                    }
                    //兑换地点
                    if (couponDetail.Contains("[EXCHANGE_PLACE]"))
                    {
                        var place = _placeService.GetAllFromCache().FirstOrDefault(c => c.Code == couponTransactionHistory.PlaceCode);
                        couponDetail = couponDetail.Replace("[EXCHANGE_PLACE]", place?.Name);
                    }
                    //兑换酒店代码 兑换酒店名称 
                    if (couponDetail.Contains("[EXCHANGE_HOTEL_CODE]") || couponDetail.Contains("[EXCHANGE_HOTEL_NAME]"))
                    {
                        var hotel = _hotelService.GetAllFromCache().FirstOrDefault(c => c.Code == couponTransactionHistory.HotelCode);
                        couponDetail = couponDetail.Replace("[EXCHANGE_HOTEL_CODE]", hotel?.Code);
                        couponDetail = couponDetail.Replace("[EXCHANGE_HOTEL_NAME]", hotel?.Name);
                    }
                    //兑换券编号
                    if (couponDetail.Contains("[EXCHANGE_SERIAL_NUMBER]"))
                    {
                        var couponIdList = list.Where(c => c.CouponTypeId == couponType.Id).Select(c => c.CouponId);
                        var number = String.Join(",", coupon.Where(c => couponIdList.Contains(c.Id)).Select(c => c.Number));
                        couponDetail = couponDetail.Replace("[EXCHANGE_SERIAL_NUMBER]", number);
                    }
                    detail = detail + couponDetail;
                }

                string replaceContent = content.Substring(content.IndexOf("[EXCHANGE_BEGIN]"), content.IndexOf("[EXCHANGE_END]") - content.IndexOf("[EXCHANGE_BEGIN]") + 14);
                if (content.IndexOf(replaceContent) > -1)
                {
                    content = content.Replace(replaceContent, detail);
                }
            }
            return content;
        }

        public string GetLocaleStringResource(string code, string languageCode)
        {
            return _localeStringResourceService.GetAllFromCache().FirstOrDefault(c => c.Code == code && c.LanguageId == languageCode)?.Name;
        }

        public string GetSendInfoContent(string content, decimal? balance, decimal? points, DateTime? enrollDate, DateTime? expireDate, DateTime? updateDate)
        {
            if (content.Contains("[POINTS]"))
            {
                content = content.Replace("[POINTS]", points.Value.ToString());
            }
            if (content.Contains("[BALANCE]"))
            {
                content = content.Replace("[BALANCE]", balance.Value.ToString());
            }
            if (content.Contains("[ENROLL_DATE]"))
            {
                content = content.Replace("[ENROLL_DATE]", enrollDate.Value.ToString("yyyy-MM-dd"));
            }
            if (content.Contains("[EXPIRY_DATE]"))
            {
                content = content.Replace("[EXPIRY_DATE]", expireDate.Value.ToString("yyyy-MM-dd"));
            }
            if (content.Contains("[UPDATE_DATE]"))
            {
                content = content.Replace("[UPDATE_DATE]", updateDate.Value.ToString("yyyy-MM-dd"));
            }
            if (content.Contains("[CHANGE_DT]"))
            {
                content = content.Replace("[CHANGE_DT]", updateDate.Value.ToString("yyyy-MM-dd"));
            }

            return content;
        }

        public string GetSendInfoContent(string content, string membershipCardNumber, string cardFaceNumber, decimal? ExpiredPoints = null, int? NextMonth = null, int? NextMonths = null, int? ExpiredYear = null)
        {
            if (content.Contains("[CARD_NO]"))
            {
                content = content.Replace("[CARD_NO]", membershipCardNumber);
            }
            if (content.Contains("[CARD_FACE_NUMBER]"))
            {
                content = content.Replace("[CARD_FACE_NUMBER]", cardFaceNumber);
            }
            if (content.Contains("[EXPIRED_POINTS]"))
            {
                if (ExpiredPoints != null)
                {
                    content = content.Replace("[EXPIRED_POINTS]", Convert.ToString(Math.Round(Math.Abs(ExpiredPoints.Value), 2, MidpointRounding.AwayFromZero)));
                }
            }
            if (content.Contains("[NEXT_MONTH]"))
            {
                if (NextMonth != null)
                {
                    content = content.Replace("[NEXT_MONTH]", NextMonth.Value.ToString());
                }
            }
            if (content.Contains("[NEXT_MONTHS]"))
            {
                if (NextMonths != null)
                {
                    content = content.Replace("[NEXT_MONTHS]", NextMonths.Value.ToString());
                }
            }
            if (content.Contains("[ORDER_EXPIRE_YEAR]"))
            {
                if (ExpiredYear != null)
                {
                    content = content.Replace("[ORDER_EXPIRE_YEAR]", ExpiredYear.Value.ToString());
                }
            }

            return content;
        }
    }
}
