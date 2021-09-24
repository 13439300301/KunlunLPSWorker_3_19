using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Points;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Kunlun.LPS.Worker.Services.RegisterPoints
{
    public class RegisterPointsService : IRegisterPointsService
    {
        private readonly ILogger<RegisterPointsService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IPointsService _pointsService;

        public RegisterPointsService(
            ILogger<RegisterPointsService> logger,
            LPSWorkerContext context,
            IPointsService pointsService
             )
        {
            _logger = logger;
            _context = context;
            _pointsService = pointsService;
        }

        public void GiftPoints(RegisterPointsMessage message, string routingKeys)
        {
            try
            {
                var date = DateTime.Now;

                var membershipCard = _context.MembershipCard.AsNoTracking().FirstOrDefault(o => o.Id == message.MembershipCardId);

                var profile = _context.Profile.AsNoTracking().FirstOrDefault(o => o.Id == message.ProfileId);
                if (profile.IsRegisterRewardPointsCalculated != null && profile.IsRegisterRewardPointsCalculated.Value == true)
                {
                    _logger.LogInformation("该档案已经算过积分或者不支持赠送积分");
                    return;
                }
                var rule = (from r in _context.RegisterPointsRule.AsNoTracking()
                            join s in _context.RegisterPointsRuleMemberSourceMap.AsNoTracking() on r.Id equals s.RegisterPointsRuleId
                            where s.MemberSource == membershipCard.MemberSourceCode
                            && r.MembershipCardTypeId == membershipCard.MembershipCardTypeId
                            && r.BeginDate <= date.Date
                            && r.EndDate >= date.Date
                            && r.WeekControl.Contains(date.DayOfWeek.ToString() == DayOfWeek.Sunday.ToString() ? "7" : date.DayOfWeek.GetHashCode().ToString())
                            select r).ToList();
                if (!rule.Any())
                {
                    _logger.LogInformation("不符合注册赠送积分规则");
                }
                else if (rule.Count > 1)
                {
                    _logger.LogInformation("匹配多条注册赠送积分规则");
                }
                else
                {
                    var registerPointsRule = rule.FirstOrDefault();
                    var detail = _context.RegisterPointsRuleMembershipCardLevelDetail.AsNoTracking().FirstOrDefault(o => o.RegisterPointsRuleId == registerPointsRule.Id
                        && o.MembershipCardLevelId == membershipCard.MembershipCardLevelId);
                    var points = registerPointsRule.Limit < detail.Points ? registerPointsRule.Limit : detail.Points;

                    if (routingKeys == RoutingKeys.Profile_MembershipCardBind && !registerPointsRule.ShouldRewardNewCard)
                    {
                        _logger.LogDebug("规则不支持新发卡赠送积分");
                        return;
                    }
                    _pointsService.GiftPoints(message, membershipCard, date, points);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
