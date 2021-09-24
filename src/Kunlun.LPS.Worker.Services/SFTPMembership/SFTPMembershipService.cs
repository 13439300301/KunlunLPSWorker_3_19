using CsvHelper;
using FluentFTP;
using Kunlun.LPS.Worker.Core.Consts;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.SFTPMembership;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Kunlun.LPS.Worker.Services.SFTPMembership
{
    public class SFTPMembershipService : ISFTPMembershipService
    {
        private readonly ILogger<SFTPMembershipService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IConfiguration _configuration;

        public SFTPMembershipService(ILogger<SFTPMembershipService> logger,
            LPSWorkerContext context,
            IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public void FTPSMembership()
        {
            try
            {
                var host = _configuration.GetValue<string>(ConfigurationKey.SFTP_CONF + ":" + ConfigurationKey.HOST);
                var loginName = _configuration.GetValue<string>(ConfigurationKey.SFTP_CONF + ":" + ConfigurationKey.LOGINNAME);
                var pwd = _configuration.GetValue<string>(ConfigurationKey.SFTP_CONF + ":" + ConfigurationKey.PWD);
                var cardTypeCode = _configuration.GetValue<string>("FTPCardTypeCode");
                var port = _configuration.GetValue<int>(ConfigurationKey.SFTP_CONF + ":" + "Port");

                string[] cardTypes = null;
                if (!string.IsNullOrEmpty(cardTypeCode))
                {
                    cardTypes = cardTypeCode.Split(',');
                }

                var date = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                var endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                _logger.LogInformation("date:" + date);
                _logger.LogInformation("endDate:" + endDate);
                _logger.LogInformation("type:" + cardTypes);
                List<SFTPMembershipInfo> membershipInfo = null;

                if (cardTypes == null)
                {
                    membershipInfo = (from p in _context.Profile.AsNoTracking()
                                      join m in _context.MembershipCard.AsNoTracking() on p.Id equals m.ProfileId
                                      join t in _context.MembershipCardType.AsNoTracking() on m.MembershipCardTypeId equals t.Id
                                      join l in _context.MembershipCardLevel.AsNoTracking() on m.MembershipCardLevelId equals l.Id
                                      where m.BindingDate.Value > date && m.BindingDate.Value < endDate
                                       && (m.Status == Core.Enum.MembershipCardStatus.Normal || m.Status == Core.Enum.MembershipCardStatus.PendingSales || m.Status == Core.Enum.MembershipCardStatus.NotOpenCard)
                                      //where p.CreateDate > date && p.CreateDate < endDate
                                      select new SFTPMembershipInfo
                                      {
                                          MemberNo = m.MembershipCardNumber,
                                          Name = p.LastName,
                                          FirstName = p.FirstName,
                                          MemberType = t.Code,
                                          CardStatus = l.Code,
                                          MemValidThru = m.ExpirationDate.HasValue ? m.ExpirationDate.Value.ToString("yyyy-MM-dd") : ""
                                      }).ToList();
                }
                else
                {
                    membershipInfo = (from p in _context.Profile.AsNoTracking()
                                      join m in _context.MembershipCard.AsNoTracking() on p.Id equals m.ProfileId
                                      join t in _context.MembershipCardType.AsNoTracking() on m.MembershipCardTypeId equals t.Id
                                      join l in _context.MembershipCardLevel.AsNoTracking() on m.MembershipCardLevelId equals l.Id
                                      where m.BindingDate.Value > date && m.BindingDate.Value < endDate
                                       && (m.Status == Core.Enum.MembershipCardStatus.Normal || m.Status == Core.Enum.MembershipCardStatus.PendingSales || m.Status == Core.Enum.MembershipCardStatus.NotOpenCard)
                                      //where p.CreateDate > date && p.CreateDate < endDate
                                      && cardTypes.Contains(t.Code)
                                      select new SFTPMembershipInfo
                                      {
                                          MemberNo = m.MembershipCardNumber,
                                          Name = p.LastName,
                                          FirstName = p.FirstName,
                                          MemberType = t.Code,
                                          CardStatus = l.Code,
                                          MemValidThru = m.ExpirationDate.HasValue ? m.ExpirationDate.Value.ToString("yyyy-MM-dd") : ""
                                      }).ToList();
                }
                _logger.LogInformation("infoCount:" + membershipInfo.Count);
                Directory.CreateDirectory(".\\CSVSave");

                using (var writer = new StreamWriter(".\\CSVSave\\SFTPMembershipInfo.csv",false, Encoding.GetEncoding("BIG5")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    foreach (var item in membershipInfo)
                    {
                        item.MemberType = item.MemberType.Length > 1 ? item.MemberType.Substring(item.MemberType.Length - 1, 1) : item.MemberType;
                        item.CardStatus = item.CardStatus.Length > 2 ? item.CardStatus.Substring(item.CardStatus.Length - 2, 2) : item.CardStatus;
                    }
                    csv.WriteRecords(membershipInfo);
                    _logger.LogInformation("csvWriter memberInfo:" + membershipInfo.Count);
                }
                _logger.LogInformation("infoCount2:" + membershipInfo.Count);

                string serverFile = "/LPS_HS/MEM" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + ".csv";

                using (var conn = new FtpClient(host, port, new NetworkCredential(loginName, pwd)))
                {
                    conn.Connect();
                    conn.UploadFile(".\\CSVSave\\SFTPMembershipInfo.csv", serverFile);

                    conn.Disconnect();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }
    }
}
