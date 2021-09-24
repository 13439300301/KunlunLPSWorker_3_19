using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kunlun.LPS.Worker.Services.StoredValue
{
    public class UpdateDbStoredValueService : IUpdateDbStoredValueService
    {
        private readonly ILogger<UpdateDbStoredValueService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;

        public UpdateDbStoredValueService(
            ILogger<UpdateDbStoredValueService> logger,
            LPSWorkerContext context,
            IGetOrUpdateInfoFromRedisService getOrUpdateInfoFromRedisService
            )
        {
            _logger = logger;
            _context = context;
            _getOrUpdateInfoFromRedisService = getOrUpdateInfoFromRedisService;
        }

        public void TestDb()
        {
            var sw = Stopwatch.StartNew();

            var accounts = _context.Account.AsNoTracking();
            var accountList = accounts
                                .Where(a => a.AccountType == MembershipCardAccountType.StoredValue)
                                .Union(accounts.Where(a => a.AccountType == MembershipCardAccountType.Points)).ToList();

            _logger.LogInformation($"Query db {sw.ElapsedMilliseconds}ms, {accountList.Count} rows");
            sw.Restart();

            var needUpdateAccounts = new List<Account>();

            foreach (var item in accountList)
            {
                string accountType = "";
                if (item.AccountType == MembershipCardAccountType.StoredValue)
                {
                    accountType = RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE;
                }
                else if (item.AccountType == MembershipCardAccountType.Points)
                {
                    accountType = RedisLuaScript.ACCOUNT_TYPE_POINTS;
                }
                else if (item.AccountType == MembershipCardAccountType.Growth)
                {
                    accountType = RedisLuaScript.ACCOUNT_TYPE_GROWTH;
                }
                var accountValue = _getOrUpdateInfoFromRedisService.GetRedisAccountInfo(item.MembershipCardId, item.MembershipCardAccountId, accountType);
                var overdraftDefineAmount = _getOrUpdateInfoFromRedisService.GetRedisAccountInfo(item.MembershipCardId, item.MembershipCardAccountId, RedisLuaScript.OVERDRAFT_DEFINE_AMOUNT);
                if (accountValue.Length > 0 && accountValue[0] != null)
                {
                    decimal value = Convert.ToDecimal(accountValue[0]);
                   
                    if (item.Value != value)
                    {
                        item.Value = value;
                        item.UpdateDate = DateTime.Now;
                        item.UpdateUser = "LPS Worker";
                      
                    }
                 
                }
                if (overdraftDefineAmount.Length > 0 && overdraftDefineAmount[0] != null)
                {
                    decimal overdraftDefineAmountvalue = Convert.ToDecimal(overdraftDefineAmount[0]);
                    if (item.CreditLine != overdraftDefineAmountvalue)
                    {
                        item.CreditLine = overdraftDefineAmountvalue;
                    }
                }
                needUpdateAccounts.Add(item);
            }

            _logger.LogInformation($"Query redis {sw.ElapsedMilliseconds}ms, {needUpdateAccounts.Count} rows");
            sw.Restart();

            var updateAccount = new List<Account>();
            int t = 1000;
            int k = 1;
            int j = 1;

            foreach (var item in needUpdateAccounts)
            {
                updateAccount.Add(item);
                if (j == t * k)
                {
                    k++;
                    _context.UpdateRange(updateAccount);
                    int updateRows = _context.SaveChanges();
                    if (updateRows < 0)
                    {
                        _logger.LogError("卡值和积分更新Db报错");
                    }
                    else
                    {
                        _logger.LogInformation("卡值和积分更新Db成功:" + updateRows);
                    }
                    updateAccount.Clear();
                }
                if (j == needUpdateAccounts.Count && updateAccount.Any())
                {
                    _context.UpdateRange(updateAccount);
                    int updateRows = _context.SaveChanges();
                    if (updateRows < 0)
                    {
                        _logger.LogError("卡值和积分更新Db报错");
                    }
                    else
                    {
                        _logger.LogInformation("卡值和积分更新Db成功:" + updateRows);
                    }
                }
                j++;
            }

            _logger.LogInformation($"Update db {sw.ElapsedMilliseconds}ms");
        }


        public void TestDbTiming(int ti)
        {
            var sw = Stopwatch.StartNew();

            DateTime date = DateTime.Now.AddSeconds(-(ti + 20));
            DateTime endDate = DateTime.Now;
            List<long> accountIdList = new List<long>();
            var accountIdStoredValueList = (from s in _context.StoredValueAccountHistory.AsNoTracking()
                                            where s.TransactionDate > date && s.TransactionDate <= endDate
                                            select s).Select(p => p.AccountId).ToList();
            if (accountIdStoredValueList.Any())
            {
                accountIdList.AddRange(accountIdStoredValueList);
            }

            var accountIdPointsList = (from s in _context.PointsAccountHistory.AsNoTracking()
                                       where s.TransactionDate > date && s.TransactionDate <= endDate
                                       select s).Select(p => p.AccountId).ToList();
            if (accountIdPointsList.Any())
            {
                accountIdList.AddRange(accountIdPointsList);
            }

            var accounts = _context.Account.AsNoTracking();
            var accountList = accounts
                                .Where(a => a.AccountType == MembershipCardAccountType.StoredValue && accountIdList.Contains(a.Id))
                                .Union(accounts.Where(a => a.AccountType == MembershipCardAccountType.Points && accountIdList.Contains(a.Id))).ToList();

            _logger.LogInformation($"Query db {sw.ElapsedMilliseconds}ms, {accountList.Count} rows");
            sw.Restart();

            var needUpdateAccounts = new List<Account>();

            foreach (var item in accountList)
            {
                string accountType = "";
                if (item.AccountType == MembershipCardAccountType.StoredValue)
                {
                    accountType = RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE;
                }
                else if (item.AccountType == MembershipCardAccountType.Points)
                {
                    accountType = RedisLuaScript.ACCOUNT_TYPE_POINTS;
                }
                else if (item.AccountType == MembershipCardAccountType.Growth)
                {
                    accountType = RedisLuaScript.ACCOUNT_TYPE_GROWTH;
                }
                var accountValue = _getOrUpdateInfoFromRedisService.GetRedisAccountInfo(item.MembershipCardId, item.MembershipCardAccountId, accountType);
                var overdraftDefineAmount = _getOrUpdateInfoFromRedisService.GetRedisAccountInfo(item.MembershipCardId,item.MembershipCardAccountId, RedisLuaScript.OVERDRAFT_DEFINE_AMOUNT);
                if (accountValue.Length > 0 && accountValue[0] != null)
                {
                    decimal value = Convert.ToDecimal(accountValue[0]);
                    if (item.Value != value)
                    {
                        
                        item.Value = value;
                        item.UpdateDate = DateTime.Now;
                        item.UpdateUser = "LPS Worker";
                       
                    }

                }
                if (overdraftDefineAmount.Length > 0 && overdraftDefineAmount[0] != null)
                {
                    decimal overdraftDefineAmountvalue = Convert.ToDecimal(overdraftDefineAmount[0]);
                    if (item.CreditLine != overdraftDefineAmountvalue)
                    {
                        item.CreditLine = overdraftDefineAmountvalue;
                    }
                }
                needUpdateAccounts.Add(item);
            }

            _logger.LogInformation($"Query redis {sw.ElapsedMilliseconds}ms, {needUpdateAccounts.Count} rows");
            sw.Restart();

            var updateAccount = new List<Account>();
            int t = 1000;
            int k = 1;
            int j = 1;

            foreach (var item in needUpdateAccounts)
            {
                updateAccount.Add(item);
                if (j == t * k)
                {
                    k++;
                    _context.UpdateRange(updateAccount);
                    int updateRows = _context.SaveChanges();
                    if (updateRows < 0)
                    {
                        _logger.LogError("卡值和积分更新Db报错");
                    }
                    else
                    {
                        _logger.LogInformation("卡值和积分更新Db成功:" + updateRows);
                    }
                    updateAccount.Clear();
                }
                if (j == needUpdateAccounts.Count && updateAccount.Any())
                {
                    _context.UpdateRange(updateAccount);
                    int updateRows = _context.SaveChanges();
                    if (updateRows < 0)
                    {
                        _logger.LogError("卡值和积分更新Db报错");
                    }
                    else
                    {
                        _logger.LogInformation("卡值和积分更新Db成功:" + updateRows);
                    }
                }
                j++;
            }

            _logger.LogInformation($"Update db {sw.ElapsedMilliseconds}ms");

        }
    }
}
