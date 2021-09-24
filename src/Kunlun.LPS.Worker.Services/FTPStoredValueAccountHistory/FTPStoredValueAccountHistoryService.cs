using FluentFTP;
using Kunlun.LPS.Worker.Core.Consts;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Kunlun.LPS.Worker.Services.FTPStoredValueAccountHistory
{
    public class FTPStoredValueAccountHistoryService : IFTPStoredValueAccountHistoryService
    {
        private readonly ILogger<FTPStoredValueAccountHistoryService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IConfiguration _configuration;

        public FTPStoredValueAccountHistoryService(ILogger<FTPStoredValueAccountHistoryService> logger,
            LPSWorkerContext context,
            IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public void FTPStoredValueAccountHistory()
        {
            try
            {
                var host = _configuration.GetValue<string>(ConfigurationKey.FTPStoredValueAccountHistoryConfig + ":" + ConfigurationKey.HOST);
                var loginName = _configuration.GetValue<string>(ConfigurationKey.FTPStoredValueAccountHistoryConfig + ":" + ConfigurationKey.LOGINNAME);
                var pwd = _configuration.GetValue<string>(ConfigurationKey.FTPStoredValueAccountHistoryConfig + ":" + ConfigurationKey.PWD);
                var port = _configuration.GetValue<int>(ConfigurationKey.FTPStoredValueAccountHistoryConfig + ":" + "Port");

                var date = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                var endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

                var cardTypeCode = _configuration.GetValue<string>("FTPStoredValueAccountHistoryCardTypeCode");
                string[] cardTypes = null;
                if (!string.IsNullOrEmpty(cardTypeCode))
                {
                    cardTypes = cardTypeCode.Split(',');
                }

                //卡值操作类型枚举（储值0、退费1、预授权2、结账3、赠送4、转账5、冲账6、调整7、取消赠送8、过期9、合并会员卡10、还款11、初始化透支额度12、透支额度调整13，分裂卡14）
                var whereType = new int[6] { 0, 1, 3, 6, 5, 7 };
                var storedValueAccountHistoryList = (from s in _context.StoredValueAccountHistory.AsNoTracking()
                                                     join a in _context.MembershipCardAccount.AsNoTracking() on s.MembershipCardAccountId equals a.Id
                                                     join t in _context.MembershipCardType.AsNoTracking() on s.MembershipCardTypeId equals t.Id
                                                     where a.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.PrincipalAccount && s.TransactionDate > date && s.TransactionDate < endDate && whereType.Contains(s.OperationType)
                                                     select new FTPStoredValueAccountHistoryInfo
                                                     {
                                                         MembershipCardTypeCode = t.Code,
                                                         Date = s.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                                         HotelCode = s.HotelCode,
                                                         MemberNo = s.MembershipCardNumber,
                                                         Type = s.OperationType,
                                                         Amount = s.Amount
                                                     }).ToList();

                if (cardTypes != null)
                {
                    storedValueAccountHistoryList = storedValueAccountHistoryList.Where(p => cardTypes.Contains(p.MembershipCardTypeCode)).ToList();
                }

                StringBuilder storedValueAccountStr = new StringBuilder();
                foreach (var item in storedValueAccountHistoryList)
                {
                    switch (item.Type)
                    {
                        case 0:
                            item.StoredValueAmount = Math.Abs(item.Amount);
                            break;
                        case 1:
                            item.RefundAmount = Math.Abs(item.Amount);
                            break;
                        case 3:
                            item.ConsumptionAmount = Math.Abs(item.Amount);
                            break;
                        case 6:
                            item.DebitBalanceAmount = Math.Abs(item.Amount);
                            break;
                        case 5:
                        case 7:
                            if (item.Amount >= 0)
                            {
                                item.StoredValueAmount = Math.Abs(item.Amount);
                            }
                            else
                            {
                                item.RefundAmount = Math.Abs(item.Amount);
                            }
                            break;
                    }

                    storedValueAccountStr.AppendLine($"{item.Date},{item.HotelCode},{item.MemberNo},{item.StoredValueAmount:#0.00},{item.ConsumptionAmount:#0.00},{item.RefundAmount:#0.00},{item.DebitBalanceAmount:#0.00}");
                }

                Directory.CreateDirectory(".\\CSVSave");

                File.WriteAllText(".\\CSVSave\\ICCARD.csv", storedValueAccountStr.ToString());

                string serverFile = "/LPS_GB/ICCARD_" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + ".csv";

                using (var conn = new FtpClient(host, port, new NetworkCredential(loginName, pwd)))
                {
                    conn.Connect();
                    conn.UploadFile(".\\CSVSave\\ICCARD.csv", serverFile);

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
