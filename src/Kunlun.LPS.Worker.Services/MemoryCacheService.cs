using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Services.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Kunlun.LPS.Worker.Services
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly IConfigurationService<Place> _placeService;
        private readonly IConfigurationService<Hotel> _hotelService;
        private readonly IConfigurationService<Sysparam> _sysparamService;
        private readonly IConfigurationService<MembershipCardType> _membershipCardTypeService;
        private readonly IConfigurationService<LocaleStringResource> _localeStringResourceService;
        private readonly IConfigurationService<MembershipCardAccount> _membershipCardAccountService;
        private readonly IConfigurationService<MealPeriod> _mealPeriodService;
        private readonly IConfigurationService<MembershipCardLevel> _membershipCardLevelService;
        private readonly IConfigurationService<FbItem> _fbItemService;
        private readonly IConfigurationService<PlaceM> _placeMService;
        private readonly IConfigurationService<RegisterCouponsRule> _registerCouponsRuleService;
        private readonly IConfigurationService<RegisterCouponsRuleMemberSourceMap> _registerCouponsRuleMemberSourceMapService;
        private readonly IConfigurationService<DicIdType> _dicIdTypeService;
        private readonly IConfigurationService<DicProvince> _dicProvinceService;
        private readonly IConfigurationService<DicSourceSvc> _dicSourceService;
        private readonly IConfigurationService<DicHistoryType> _dicHistoryTypeService;
        private readonly IConfigurationService<PointsValidPeriodRule> _pointsValidPeriodRuleService;
        private readonly IConfigurationService<PointsAccrueType> _pointsAccrueTypeService;
        private readonly IConfigurationService<PointsValidPeriodAdvanceSendInfo> _pointsValidPeriodAdvanceSendInfoService;

        public MemoryCacheService(
            ILogger<MemoryCacheService> logger,
            IConfigurationService<Place> placeService,
            IConfigurationService<Hotel> hotelService,
            IConfigurationService<Sysparam> sysparamService,
            IConfigurationService<MembershipCardType> membershipCardTypeService,
            IConfigurationService<LocaleStringResource> localeStringResourceService,
            IConfigurationService<MembershipCardAccount> membershipCardAccountService,
            IConfigurationService<MealPeriod> mealPeriodService,
            IConfigurationService<MembershipCardLevel> membershipCardLevelService,
            IConfigurationService<FbItem> fbItemService,
            IConfigurationService<PlaceM> placeMService,
            IConfigurationService<RegisterCouponsRule> registerCouponsRuleService,
            IConfigurationService<RegisterCouponsRuleMemberSourceMap> registerCouponsRuleMemberSourceMapService,
            IConfigurationService<DicIdType> dicIdTypeService,
            IConfigurationService<DicProvince> dicProvinceService,
            IConfigurationService<DicSourceSvc> dicSourceService,
            IConfigurationService<DicHistoryType> dicHistoryTypeService,
            IConfigurationService<PointsValidPeriodRule> pointsValidPeriodRuleService,
            IConfigurationService<PointsAccrueType> pointsAccrueTypeService,
            IConfigurationService<PointsValidPeriodAdvanceSendInfo> pointsValidPeriodAdvanceSendInfoService
            )
        {
            _logger = logger;
            _placeService = placeService;
            _hotelService = hotelService;
            _sysparamService = sysparamService;
            _membershipCardTypeService = membershipCardTypeService;
            _localeStringResourceService = localeStringResourceService;
            _membershipCardAccountService = membershipCardAccountService;
            _mealPeriodService = mealPeriodService;
            _membershipCardLevelService = membershipCardLevelService;
            _fbItemService = fbItemService;
            _placeMService = placeMService;
            _registerCouponsRuleService = registerCouponsRuleService;
            _registerCouponsRuleMemberSourceMapService = registerCouponsRuleMemberSourceMapService;
            _dicIdTypeService = dicIdTypeService;
            _dicProvinceService = dicProvinceService;
            _dicSourceService = dicSourceService;
            _dicHistoryTypeService = dicHistoryTypeService;
            _pointsValidPeriodRuleService = pointsValidPeriodRuleService;
            _pointsAccrueTypeService = pointsAccrueTypeService;
            _pointsValidPeriodAdvanceSendInfoService = pointsValidPeriodAdvanceSendInfoService;
        }

        public async Task LoadAllConfigurationCacheAsync()
        {
            await _placeService.SetCacheAsync();
            await _hotelService.SetCacheAsync();
            await _sysparamService.SetCacheAsync();
            await _membershipCardTypeService.SetCacheAsync();
            await _localeStringResourceService.SetCacheAsync();
            await _membershipCardAccountService.SetCacheAsync();
            await _mealPeriodService.SetCacheAsync();
            await _membershipCardLevelService.SetCacheAsync();
            await _fbItemService.SetCacheAsync();
            await _placeMService.SetCacheAsync();
            await _registerCouponsRuleService.SetCacheAsync();
            await _registerCouponsRuleMemberSourceMapService.SetCacheAsync();
            await _dicIdTypeService.SetCacheAsync();
            await _dicProvinceService.SetCacheAsync();
            await _dicSourceService.SetCacheAsync();
            await _dicHistoryTypeService.SetCacheAsync();
            await _pointsValidPeriodRuleService.SetCacheAsync();
            await _pointsAccrueTypeService.SetCacheAsync();
            await _pointsValidPeriodAdvanceSendInfoService.SetCacheAsync();
        }

        public async Task LoadConfigurationCacheAsync(string cacheDataTypeName, bool throwIfNotExists = false)
        {
            _logger.LogDebug($"LoadConfigurationCacheAsync(): cacheDataTypeName = {cacheDataTypeName}, throwIfNotExists = {throwIfNotExists}");

            switch (cacheDataTypeName?.ToLowerInvariant())
            {
                case "place":
                    await _placeService.SetCacheAsync();
                    break;
                case "hotel":
                    await _hotelService.SetCacheAsync();
                    break;
                case "sysparam":
                    await _sysparamService.SetCacheAsync();
                    break;
                case "membershipcardtype":
                    await _membershipCardTypeService.SetCacheAsync();
                    break;
                case "localestringresource":
                    await _localeStringResourceService.SetCacheAsync();
                    break;
                case "membershipcardaccount":
                    await _membershipCardAccountService.SetCacheAsync();
                    break;
                case "mealperiod":
                    await _mealPeriodService.SetCacheAsync();
                    break;
                case "fbitem":
                    await _fbItemService.SetCacheAsync();
                    break;
                case "membershipcardlevel":
                    await _membershipCardLevelService.SetCacheAsync();
                    break;
                case "placem":
                    await _placeMService.SetCacheAsync();
                    break;
                case "registercouponsrule":
                    await _registerCouponsRuleService.SetCacheAsync();
                    break;
                case "registercouponsrulemembersourcemap":
                    await _registerCouponsRuleMemberSourceMapService.SetCacheAsync();
                    break;
                case "dicidtype":
                    await _dicIdTypeService.SetCacheAsync();
                    break;
                case "dicprovince":
                    await _dicProvinceService.SetCacheAsync();
                    break;
                case "dicsourcesvc":
                    await _dicSourceService.SetCacheAsync();
                    break;
                case "dichistorytype":
                    await _dicHistoryTypeService.SetCacheAsync();
                    break;
                case "pointsvalidperiodrule":
                    await _pointsValidPeriodRuleService.SetCacheAsync();
                    break;
                case "pointsaccruetype":
                    await _pointsAccrueTypeService.SetCacheAsync();
                    break;
                case "pointsvalidperiodadvancesendinfo":
                    await _pointsValidPeriodAdvanceSendInfoService.SetCacheAsync();
                    break;
                default:
                    if (throwIfNotExists)
                    {
                        throw new ArgumentException($"没有找到名为 {cacheDataTypeName} 的缓存类型");
                    }

                    return;
            }
        }
    }
}
