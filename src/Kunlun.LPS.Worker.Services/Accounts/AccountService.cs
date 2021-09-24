using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Common;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunlun.LPS.Worker.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IMessageQueueProducer _messageQueueProducer;
        private readonly ICommonService _commonService;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;
        private readonly Configurations.IConfigurationService<Core.Domain.Configurations.Sysparam> _sysparamService;

        public AccountService(
           ILogger<AccountService> logger,
           LPSWorkerContext context,
           IUniqueIdGeneratorService uniqueIdGeneratorService,
           IMessageQueueProducer messageQueueProducer,
           ICommonService commonService,
           IGetOrUpdateInfoFromRedisService getOrUpdateInfoFromRedisService,
           Configurations.IConfigurationService<Core.Domain.Configurations.Sysparam> sysparamService
            )
        {
            _logger = logger;
            _context = context;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _messageQueueProducer = messageQueueProducer;
            _commonService = commonService;
            _getOrUpdateInfoFromRedisService = getOrUpdateInfoFromRedisService;
            _sysparamService = sysparamService;
        }

        /// <summary>
        /// 生成账户到redis和数据库\删除冗余 InitAccount方法
        /// </summary>
        /// <param name="membershipCard">卡信息</param>
        /// <param name="membershipCardAccountType">账户类型 不传即生成全部</param>
        /// <param name="membershipCardAccountIds">要保存的账户类型id</param>
        /// <returns></returns>
        public List<Account> InitAccount(long membershipCardId, MembershipCardAccountType? membershipCardAccountType = null, List<long> membershipCardAccountIds = null, decimal? initCardFee = null, MembershipCard membershipCard = null)
        {
            List<Account> accountList = new List<Account>();
            List<StoredValueAccountHistory> storedValueAccountHistoryList = new List<StoredValueAccountHistory>();
            try
            {
                var cardAccountList = _context.MembershipCardAccount.Where(p => p.MembershipCardTypeId == membershipCard.MembershipCardTypeId).ToList();
                if (membershipCardAccountType != null)
                {
                    cardAccountList = cardAccountList.Where(p => p.Type == membershipCardAccountType).ToList();
                }
                if (membershipCardAccountIds != null)
                {
                    cardAccountList = cardAccountList.Where(p => membershipCardAccountIds.Any(t => t == p.Id)).ToList();
                }
                foreach (var a in cardAccountList)
                {
                    if (a.Type == MembershipCardAccountType.Points)
                    {
                        string value = "0";
                        string creditLine = "0";
                        var isExist = _getOrUpdateInfoFromRedisService.CreateAccount(membershipCard.Id.ToString(), a.Id.ToString(), RedisLuaScript.ACCOUNT_TYPE_POINTS, "", value, creditLine);
                        if (isExist == "0")
                        {
                            var account = new Account
                            {
                                Id = _uniqueIdGeneratorService.Next(),
                                MembershipCardAccountId = a.Id,
                                AccountType = a.Type,
                                MembershipCardId = membershipCard.Id,
                                Value = Convert.ToDecimal(0),
                                CreditLine = Convert.ToDecimal(0),
                                Version = new byte[] { 1, 0, 0, 0 },
                                InsertDate = DateTime.Now,
                                InsertUser = "Worker",
                                UpdateDate = DateTime.Now,
                                UpdateUser = "Worker"
                            };
                            //_context.Account.Add(account);
                            //int isSuccess = _context.SaveChanges();
                            accountList.Add(account);
                        }
                        else
                        {
                            var acc = _context.Account.FirstOrDefault(p => p.MembershipCardId == membershipCard.Id && p.MembershipCardAccountId == a.Id);
                            if (acc != null) accountList.Add(acc);
                            return accountList;
                        }
                    }
                    else if (a.Type == MembershipCardAccountType.Growth)
                    {
                        #region redis判断

                        #endregion

                        #region 数据库判断是否存在账户

                        var acc = _context.Account.FirstOrDefault(p => p.MembershipCardId == membershipCard.Id && p.MembershipCardAccountId == a.Id);
                        if (acc == null)
                        {
                            var account = new Account
                            {
                                Id = _uniqueIdGeneratorService.Next(),
                                MembershipCardAccountId = a.Id,
                                AccountType = a.Type,
                                MembershipCardId = membershipCard.Id,
                                Value = Convert.ToDecimal(0),
                                CreditLine = Convert.ToDecimal(0),
                                Version = new byte[] { 1, 0, 0, 0 },
                                InsertDate = DateTime.Now,
                                InsertUser = "Worker",
                                UpdateDate = DateTime.Now,
                                UpdateUser = "Worker"
                            };
                            accountList.Add(account);
                        }
                        else
                        {
                            accountList.Add(acc);
                            return accountList;
                        }

                        #endregion
                    }
                    else if (a.Type == MembershipCardAccountType.StoredValue)
                    {
                        if (a.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.PrincipalAccount)
                        {
                            string value = !initCardFee.HasValue ? "0" : initCardFee.Value.ToString();
                            string creditLine = "0";
                            var isExist = _getOrUpdateInfoFromRedisService.CreateAccount(membershipCard.Id.ToString(), a.Id.ToString(), RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE, "PrincipalAccount", value, creditLine);

                            if (isExist == "0")
                            {
                                var account = new Account
                                {
                                    Id = _uniqueIdGeneratorService.Next(),
                                    MembershipCardAccountId = a.Id,
                                    AccountType = a.Type,
                                    MembershipCardId = membershipCard.Id,
                                    Value = membershipCard.MainMembershipCardId.HasValue ? Convert.ToDecimal(0) : a.CreditLine,
                                    CreditLine = membershipCard.MainMembershipCardId.HasValue ? Convert.ToDecimal(0) : a.CreditLine,
                                    Version = new byte[] { 1, 0, 0, 0 }
                                };
                                accountList.Add(account);
                                if (initCardFee.HasValue)
                                {
                                    if (!membershipCard.MainMembershipCardId.HasValue)
                                    {
                                        storedValueAccountHistoryList.Add(new StoredValueAccountHistory
                                        {
                                            Id = _uniqueIdGeneratorService.Next(),
                                            ProfileId = membershipCard.ProfileId.Value,
                                            MembershipCardId = membershipCard.Id,
                                            MembershipCardNumber = membershipCard.MembershipCardNumber,
                                            AccountId = account.Id,
                                            MembershipCardTypeId = membershipCard.MembershipCardTypeId,
                                            MembershipCardAccountId = account.MembershipCardAccountId,
                                            OperationType = (int)StoredValueAccountHistoryOperationType.InitCardFee,
                                            RevenueType = (int)StoredValueAccountHistoryRevenueType.Other,
                                            Amount = initCardFee.Value,
                                            LastBalance = 0,
                                            ThisBalance = account.Value,
                                            TransactionDate = DateTime.Now,
                                            ExpireDate = null,
                                            IsVoild = false,
                                            BatchId = Guid.NewGuid(),
                                            IsManual = false,
                                            HotelCode = "",
                                            PlaceCode = "",
                                            Description = "Init Card Fee"
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var acc = _context.Account.FirstOrDefault(p => p.MembershipCardId == membershipCard.Id && p.MembershipCardAccountId == a.Id);
                                if (acc != null) accountList.Add(acc);
                                return accountList;
                            }
                        }
                        else if (a.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount)
                        {
                            string value = a.CreditLine.ToString();
                            string creditLine = a.CreditLine.ToString();
                            var isExist = _getOrUpdateInfoFromRedisService.CreateAccount(membershipCardId.ToString(), a.Id.ToString(), RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE, "OverdraftAccount", value, creditLine);
                            if (isExist == "0")
                            {
                                var account = new Account
                                {
                                    Id = _uniqueIdGeneratorService.Next(),
                                    MembershipCardAccountId = a.Id,
                                    AccountType = a.Type,
                                    MembershipCardId = membershipCard.Id,
                                    Value = membershipCard.MainMembershipCardId.HasValue ? Convert.ToDecimal(0) : a.CreditLine,
                                    CreditLine = membershipCard.MainMembershipCardId.HasValue ? Convert.ToDecimal(0) : a.CreditLine,
                                    Version = new byte[] { 1, 0, 0, 0 }
                                };
                                accountList.Add(account);
                                if (!membershipCard.MainMembershipCardId.HasValue)
                                {
                                    storedValueAccountHistoryList.Add(new StoredValueAccountHistory
                                    {
                                        Id = _uniqueIdGeneratorService.Next(),
                                        ProfileId = membershipCard.ProfileId.Value,
                                        MembershipCardId = membershipCard.Id,
                                        MembershipCardNumber = membershipCard.MembershipCardNumber,
                                        AccountId = account.Id,
                                        MembershipCardTypeId = membershipCard.MembershipCardTypeId,
                                        MembershipCardAccountId = account.MembershipCardAccountId,
                                        OperationType = (int)StoredValueAccountHistoryOperationType.InitOverdraft,
                                        RevenueType = (int)StoredValueAccountHistoryRevenueType.Other,
                                        Amount = a.CreditLine,
                                        LastBalance = 0,
                                        ThisBalance = account.Value,
                                        TransactionDate = DateTime.Now,
                                        ExpireDate = null,
                                        IsVoild = false,
                                        BatchId = Guid.NewGuid(),
                                        IsManual = false,
                                        HotelCode = "",
                                        PlaceCode = "",
                                        Description = "Init Overdraft"
                                    });
                                }
                            }
                            else
                            {
                                var acc = _context.Account.FirstOrDefault(p => p.MembershipCardId == membershipCard.Id && p.MembershipCardAccountId == a.Id);
                                if (acc != null) accountList.Add(acc);
                                return accountList;
                            }
                        }
                        else
                        {
                            string value = "0";
                            string creditLine = "0";
                            var isExist = _getOrUpdateInfoFromRedisService.CreateAccount(membershipCardId.ToString(), a.Id.ToString(), RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE, "", value, creditLine);
                            if (isExist == "0")
                            {
                                var account = new Account
                                {
                                    Id = _uniqueIdGeneratorService.Next(),
                                    MembershipCardAccountId = a.Id,
                                    AccountType = a.Type,
                                    MembershipCardId = membershipCard.Id,
                                    Value = Convert.ToDecimal(0),
                                    CreditLine = Convert.ToDecimal(0),
                                    Version = new byte[] { 1, 0, 0, 0 }
                                };
                                accountList.Add(account);
                            }
                            else
                            {
                                var acc = _context.Account.FirstOrDefault(p => p.MembershipCardId == membershipCard.Id && p.MembershipCardAccountId == a.Id);
                                if (acc != null) accountList.Add(acc);
                                return accountList;
                            }
                        }

                    }

                    else
                    {
                       
                            var acc = _context.Account.FirstOrDefault(p => p.MembershipCardId == membershipCard.Id && p.MembershipCardAccountId == a.Id);
                            if (acc != null) accountList.Add(acc);
                            return accountList;
                    }
                }

                try
                {
                    if (accountList.Any())
                    {
                        _context.Account.AddRange(accountList);                   
                    }
                    if (storedValueAccountHistoryList.Any())
                    {
                        _context.StoredValueAccountHistory.AddRange(storedValueAccountHistoryList);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogInformation("初始化时，插入报错，添加事务时，会报错");
                    _logger.LogInformation("连接已经在事务中，不能参与另一个事务。 EntityClient 不支持并行事务。");
                    foreach (var a in accountList)
                    {
                        if (a.AccountType == MembershipCardAccountType.StoredValue)
                        {
                            _getOrUpdateInfoFromRedisService.DeleteAccount(a.MembershipCardId.ToString(), a.MembershipCardAccountId.ToString(), RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE);
                        }
                        else if (a.AccountType == MembershipCardAccountType.Points)
                        {
                            _getOrUpdateInfoFromRedisService.DeleteAccount(a.MembershipCardId.ToString(), a.MembershipCardAccountId.ToString(), RedisLuaScript.ACCOUNT_TYPE_POINTS);
                        }
                    }
                    _logger.LogInformation("初始化时，插入报错");
                    _logger.LogInformation(ex.Message);
                    _logger.LogError(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("InitAccount报错");
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);
            }
            return accountList;
        }

        /// <summary>
        /// 生成账户到redis和数据库
        /// </summary>
        /// <param name="membershipCardId">卡id</param>
        /// <param name="membershipCardAccountType">账户类型 不传即判断全部</param>
        /// <param name="membershipCardAccountIds">要查询的账户类型id</param>
        /// <returns></returns>
        public List<RequestAccount> RequestAccounts(long membershipCardId, MembershipCardAccountType? membershipCardAccountType = null, List<long> membershipCardAccountIds = null, decimal? initCardFee = null)
        {
            List<RequestAccount> requestAccounts = new List<RequestAccount>();
            try
            {
                MembershipCard membershipCard = new MembershipCard();
                //查会员
                membershipCard = _context.MembershipCard.FirstOrDefault(t => t.Id == membershipCardId);
                //账户类型
                var cardAccountTypeList = _context.MembershipCardAccount.Where(t => t.Id == membershipCard.MembershipCardTypeId).ToList();

                if (membershipCardAccountType != null)
                {
                    cardAccountTypeList = cardAccountTypeList.Where(p => p.Type == membershipCardAccountType).ToList();
                }

                if (membershipCardAccountIds != null)
                {
                    _logger.LogInformation("传入了membershipCardAccountIds");
                    cardAccountTypeList = cardAccountTypeList.Where(p => membershipCardAccountIds.Any(t => t == p.Id)).ToList();
                }

                #region New
                RequestAccount requestAccountList = new RequestAccount();
                foreach (var membershipCardAccount in cardAccountTypeList)
                {
                    string redisType = String.Empty;
                    if (membershipCardAccount.Type == MembershipCardAccountType.StoredValue)
                    {
                        redisType = RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE;
                    }
                    else if (membershipCardAccount.Type == MembershipCardAccountType.Points)
                    {
                        redisType = RedisLuaScript.ACCOUNT_TYPE_POINTS;
                    }
                    else if (membershipCardAccount.Type == MembershipCardAccountType.Growth)
                    {
                        redisType = RedisLuaScript.ACCOUNT_TYPE_GROWTH;
                    }
                    else
                    {
                        requestAccountList.Code = "ERROR";
                        requestAccountList.Name = "账户类型不存在。";
                        requestAccountList.Accounts = null;
                        requestAccounts.Add(requestAccountList);
                        return requestAccounts;
                    }

                    //判断redis 是否存在
                    bool isExistRedis = _getOrUpdateInfoFromRedisService.IsExistRedisAccountInfo(membershipCardId, membershipCardAccount.Id, redisType);

                    var cardAccountLists = _context.Account.Where(t => t.MembershipCardId == membershipCard.Id && t.MembershipCardAccountId == membershipCardAccount.Id).ToList();

                    if (!((isExistRedis && cardAccountLists.Count == 1) || (!isExistRedis && cardAccountLists.Count == 0)))
                    {
                        requestAccountList.Code = "ERROR";
                        requestAccountList.Name = "Redis和数据库账户不一致。";
                        requestAccountList.Accounts = null;
                        _logger.LogInformation("Redis与数据库账户不一致的,账户类型:" + redisType + ";MemberShipCardId:" + membershipCardId.ToString() + ";MemberShipCardNumber:"+ membershipCard.MembershipCardNumber.ToString() + "MembershipCardAccountId:" + membershipCardAccount.Id.ToString());
                        requestAccounts.Add(requestAccountList);
                        return requestAccounts;
                    }
                }


                requestAccountList.Code = "SUCCESS";
                requestAccountList.Name = "数据库与redis一致";
                //返回 accountlist
                requestAccountList.Accounts = InitAccount(membershipCardId, membershipCardAccountType, membershipCardAccountIds, initCardFee, membershipCard);
                requestAccounts.Add(requestAccountList);
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogInformation("调用Redis初始化账户验证 RequestAccounts 报错:" + ex);
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.ToString());
                RequestAccount requestAccountList = new RequestAccount();
                requestAccountList.Code = "ERROR";
                requestAccountList.Name = "Redis初始化账户验证报错";
                requestAccountList.Accounts = null;
                requestAccounts.Add(requestAccountList);

                return requestAccounts;
            }

            return requestAccounts;
        }
    }
}
