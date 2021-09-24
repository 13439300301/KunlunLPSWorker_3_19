using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Points;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Linq;
using System.Collections.Generic;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.Enum;

namespace Kunlun.LPS.Worker.Services.HRTProfile
{
    public class HRTProfileService : IHRTProfileService
    {

        private readonly ILogger<MembershipCardLevelChangeService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly ISendInfoService _sendInfoService;
        private readonly ISendInfoTempletService _sendInfoTempletService;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IPointsService _pointsService;
        private readonly Configurations.IConfigurationService<Core.Domain.Configurations.Sysparam> _sysparamService;

        public HRTProfileService(
            ILogger<MembershipCardLevelChangeService> logger,
            LPSWorkerContext context,
            ISendInfoService sendInfoService,
            ISendInfoTempletService sendInfoTempletService,
            IGetOrUpdateInfoFromRedisService getOrUpdateInfoFromRedisService,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            IPointsService pointsService,
            Configurations.IConfigurationService<Core.Domain.Configurations.Sysparam> sysparamService
            )
        {
            _logger = logger;
            _context = context;
            _sendInfoService = sendInfoService;
            _sendInfoTempletService = sendInfoTempletService;
            _logger.LogInformation(nameof(MembershipCardLevelChangeService));
            _getOrUpdateInfoFromRedisService = getOrUpdateInfoFromRedisService;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _pointsService = pointsService;
            _sysparamService = sysparamService;
        }
        public void UpdateLpsProfile(GetMemberResponse response)
        {
            var logger = LogManager.GetLogger("HrtProfileConsumer");
            var ProviderCode = "HRT";
            try
            {

                Profile profileExternal = new Profile();
                profileExternal = (from p in _context.Profile
                                   from px in _context.ProfileExternalMembership 
                                   where p.Id == px.ProfileId && px.ProviderKey == response.memberId
                                   && px.IsValid
                                   select p).FirstOrDefault();

                var lpsProfile = _context.Profile.FirstOrDefault(m => m.MobilePhoneNumber == response.mobile);
                var phoneNumberDetail = new ProfileMobilePhoneNumberDetail();
                var idNumberDetail = new ProfileIdDetail();
                bool idNumberFlag = false;
                //外部会员存在，和华润通存在绑定关系 更新会员
                if (profileExternal != null)
                {
                    var profileInfo = profileExternal;
                    profileInfo.Remark = response.memo;
                    if (profileInfo.MobilePhoneNumber != response.mobile)
                    {
                        phoneNumberDetail = _context.ProfileMobilePhoneNumberDetail.FirstOrDefault(m => m.ProfileId == profileInfo.Id && m.IsDefault == true);
                        phoneNumberDetail.MobilePhoneNumber = response.mobile;
                    }
                    profileInfo.MobilePhoneNumber = response.mobile;
                    profileInfo.FirstName = response.name.Substring(1, response.name.Length - 1);
                    profileInfo.LastName = response.name.Substring(0, 1);
                    profileInfo.FullName = response.name;
                    DateTime? birthday = null;
                    if (!string.IsNullOrEmpty(response.birthday))
                    {
                        birthday = Convert.ToDateTime(response.birthday);
                    }
                    else
                    {
                        birthday = null;
                    }
                    profileInfo.Birthday = birthday;
                    profileInfo.Email = response.email;
                    var IdTypeCodeObj = _sysparamService.GetAllFromCache().FirstOrDefault(c => c.Code == "DOMESTIC_ID_CARD_CODE");
                    if (IdTypeCodeObj==null)
                    {
                        IdTypeCodeObj=_context.Sysparam.FirstOrDefault(c => c.Code == "DOMESTIC_ID_CARD_CODE");
                    }
                    //身份证号 
                    if (profileInfo.IdTypeCode == IdTypeCodeObj?.ParValue)
                    {
                        if (response.certificateType == "1" && response.certificateNo != null)
                        {
                            idNumberDetail = _context.ProfileIdDetail.FirstOrDefault(m => m.ProfileId == profileInfo.Id && m.IdNumber == profileInfo.IdNumber);
                            if (idNumberDetail != null)
                            {
                                idNumberDetail.IdNumber = response.certificateNo;
                            }
                            else
                            {
                                idNumberDetail = new ProfileIdDetail();
                                idNumberDetail.Id = _uniqueIdGeneratorService.Next();
                                idNumberDetail.IdNumber = response.certificateNo;
                                idNumberDetail.IdTypeCode = IdTypeCodeObj?.ParValue;
                                idNumberDetail.IsDefault = true;
                                idNumberDetail.ProfileId = profileInfo.Id;
                                idNumberFlag = true;
                            }

                        }
                        profileInfo.IdNumber = response.certificateNo;
                    }


                    //修改外部会员级别同使修改卡级别
                    var external = _context.ProfileExternalMembership.FirstOrDefault(m => m.ProfileId == profileInfo.Id);
                    var membershipCard = _context.MembershipCard.FirstOrDefault(m => m.ProfileId == profileExternal.Id);
                    var membershipCardLevelChangeHistory = new MembershipCardLevelChangeHistory();
                    var newMembershipCardLevel = new MembershipCardLevel();
                    var logs = new List<LPSLog>();
                    if (external != null)
                    { 
                        var levelCode = _context.ExternalMembershipLevel.FirstOrDefault(m => m.Id == external.LevelId)?.Code;
                        if (levelCode != response.memberLevel)
                        {
                            var ProviderId = _context.ExternalMembershipProvider.FirstOrDefault(m => m.Code == ProviderCode)?.Id;
                            var ExternalMemberLevelId = _context.ExternalMembershipLevel.Where(m => m.Code == response.memberLevel && m.ExternalMembershipProviderId == ProviderId);
                            external.LevelId = ExternalMemberLevelId.FirstOrDefault().Id;
                            logger.Info("会员外部会员提供商级别变成完成：OldLevel=" + levelCode + ",NewLevel=" + response.memberLevel);
                            //var cardLevelMapItem = _externalMembershipLevelMembershipCardLevelMapService.GetCardLevelMapByTypeIdAndExternalLevelId(membershipCard.MembershipCardTypeId, ExternalMemberLevelId.FirstOrDefault().Id);
                            var cardLevelMapItem = _context.ExternalMembershipLevelMembershipCardLevelMap.FirstOrDefault(m => m.MembershipCardTypeId == membershipCard.MembershipCardTypeId && m.ExternalMembershipLevelId == ExternalMemberLevelId.FirstOrDefault().Id);
                            if (cardLevelMapItem != null)
                            {
                                newMembershipCardLevel = _context.MembershipCardLevel.FirstOrDefault(m => m.Id == cardLevelMapItem.MembershipCardLevelId);

                                membershipCard.MembershipCardLevelId = newMembershipCardLevel.Id;



                                membershipCardLevelChangeHistory.Id = _uniqueIdGeneratorService.Next();
                                membershipCardLevelChangeHistory.ProfileId = profileInfo.Id;
                                membershipCardLevelChangeHistory.MembershipCardId = membershipCard.Id;
                                membershipCardLevelChangeHistory.MembershipCardTypeId = membershipCard.MembershipCardTypeId;
                                membershipCardLevelChangeHistory.SourceLevelId = membershipCard.MembershipCardLevelId;
                                membershipCardLevelChangeHistory.DestinationLevelId = newMembershipCardLevel.Id;
                                membershipCardLevelChangeHistory.Direction = MembershipCardLevelChangeDirection.Upgrade;
                                membershipCardLevelChangeHistory.ChangeTime = DateTime.Now;
                                membershipCardLevelChangeHistory.Description = "修改外部会员级别时，修改的外部身份的级别比当前会员卡级别高，级别互通升级";


                                logs.Add(new LPSLog()
                                {
                                    Id = _uniqueIdGeneratorService.Next(),
                                    RequestId = Guid.NewGuid(),
                                    OperationType = LogOperationType.ChangeMembershipCardLevel.ToString(),
                                    OperationMainDataId = membershipCard.Id.ToString(),
                                    OperationDataType = LogDataType.MembershipCard.ToString(),
                                    Description = "修改外部会员" + external.Id + "级别时触发级别同步，卡级别由" + membershipCard.MembershipCardLevelId + "升级为" + newMembershipCardLevel.Id
                                });



                            }
                        }
                    }

                    if (response.memberStatus == "0" || response.memberStatus == "3" || response.memberStatus == "5")
                    {
                        MembershipCardStatus status = MembershipCardStatus.Normal;
                        var description = "";
                        switch (response.memberStatus)
                        {
                            case "0":
                                status = MembershipCardStatus.Normal;
                                break;
                            case "3":
                                status = MembershipCardStatus.Freeze;
                                description = "冻结卡;原因：消费者自动跟随华润通更新会员信息";
                                break;
                            case "5":
                                status = MembershipCardStatus.Cancelled;
                                description = "注销卡;原因：消费者自动跟随华润通更新会员信息";
                                break;
                        }

                        if (membershipCard.Status != status)
                        {
                            membershipCard.Status = status;
                            logs.Add(new LPSLog()
                            {
                                Id = _uniqueIdGeneratorService.Next(),
                                OperationDataType = LogOperationType.ChangeMembershipCardStatus.ToString(),
                                OperationMainDataId = membershipCard.Id.ToString(),
                                OperationType = LogDataType.MembershipCard.ToString(),
                                Description = description
                            });
                        }

                    }

                    _context.Profile.Update(profileInfo);
                    if (phoneNumberDetail.Id != 0)
                    {
                        _context.ProfileMobilePhoneNumberDetail.Update(phoneNumberDetail);
                    }
                    if (external != null)
                    {
                        _context.ProfileExternalMembership.Update(external);
                    }
                    if (idNumberFlag)
                    {
                        _context.ProfileIdDetail.Add(idNumberDetail);
                    }
                    else
                    {
                        if (idNumberDetail != null)
                        {
                            if (idNumberDetail.Id != 0)
                            {
                                _context.ProfileIdDetail.Update(idNumberDetail);
                            }
                        }
                    }
                    _context.MembershipCard.Update(membershipCard);
                    if (membershipCardLevelChangeHistory.Id != 0)
                    {
                        _context.MembershipCardLevelChangeHistory.Add(membershipCardLevelChangeHistory);
                    }
                    if (logs.Any())
                    {
                        _context.LPSLog.AddRange(logs);
                    } 
                    _context.SaveChanges();

                    logger.Info("会员更新完成：memberId=" + response.memberId + ",name=" + response.name + ",MobilePhoneNumber=" + response.mobile + ",Birthday=" + response.birthday + ",Email=" + response.email);
                    if (logs.Any())
                    {
                        logger.Info("修改外部会员" + external.Id + "级别时触发级别同步，卡级别由" + membershipCard.MembershipCardLevelId +  "升级为" + newMembershipCardLevel.Code + "-" + newMembershipCardLevel.Name);
                        logger.Info("更新会员卡状态为：" + response.memberStatus);
                    }


                }//外部会员不存在，不存在绑定关系，但是LPS存在会员
                else if (profileExternal == null && lpsProfile != null)
                { 
                    var providerId = _context.ExternalMembershipProvider.FirstOrDefault(m => m.Code == ProviderCode); 
                    var providerLevel = _context.ExternalMembershipLevel.FirstOrDefault(m => m.Code == response.memberLevel && m.ExternalMembershipProviderId == providerId.Id);


                    ProfileExternalMembership profileExternalMembership = new ProfileExternalMembership()
                    {
                        Id = _uniqueIdGeneratorService.Next(),
                        ProfileId = lpsProfile.Id,
                        ProviderId = providerId.Id,
                        ProviderKey = response.memberId,
                        ProviderType = providerId.Type,
                        IsValid = true,
                        LevelId = providerLevel.Id
                    }; 
                    _context.ProfileExternalMembership.Add(profileExternalMembership);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Info("修改外部会员:" + response.memberId + ";修改失败！");
                throw;
            }

        }
    }
}
