using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Kunlun.LPS.Worker.Services.StoredValue;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Services.Accounts;

namespace Kunlun.LPS.Worker.Services.RegularValueStorageRule
{
    public class RegularValueStorageRuleService : IRegularValueStorageRuleService
    {
        private readonly ILogger<RegularValueStorageRuleService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;
        private readonly IAccountService _accountService;
        public RegularValueStorageRuleService(ILogger<RegularValueStorageRuleService> logger,
            LPSWorkerContext context,
            IGetOrUpdateInfoFromRedisService getOrUpdateInfoFromRedisService,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            IAccountService accountService
           )
        {
            _logger = logger;
            _context = context;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _getOrUpdateInfoFromRedisService = getOrUpdateInfoFromRedisService;
            _accountService = accountService;
        }

        /// <summary>
        /// 获取季度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool GetFirstQuarter(int value)
        {
            string[] montharr = new string[] { "01", "04", "07", "10" };

            string result = string.Empty;
            bool b = false;
            for (int i = 0; i < montharr.Length; i++)
            {
                if (value < 10)
                    result = montharr[i] + "-0" + value;
                else
                    result = montharr[i] + "-" + value;
                if (result == DateTime.Now.ToString("MM-dd"))
                {
                    b = true;
                    return b;
                }
            }

            return b;

        }
        public static bool GetLastQuarter(int value)
        {
            string[] quareterarr = new string[] { "03", "06", "09", "12" };
            string quarterresult = string.Empty;
            bool b = false;
            for (int i = 0; i < quareterarr.Length; i++)
            {
                quarterresult = quareterarr[i] + "-" + value;

                if (quarterresult == DateTime.Now.ToString("MM-dd"))
                {
                    b = true;
                    return b;
                }
            }
            return b;
        }

        //public List<Account> InitAccount(MembershipCard membershipCard, MembershipCardAccountType? membershipCardAccountType = null, List<long> membershipCardAccountIds = null)
        //{
        //    List<Account> accountList = new List<Account>();
        //    var cardAccountList = _context.MembershipCardAccount.Where(p => p.MembershipCardTypeId == membershipCard.MembershipCardTypeId).ToList();
        //    if (membershipCardAccountType != null)
        //    {
        //        cardAccountList = cardAccountList.Where(p => p.Type == membershipCardAccountType).ToList();
        //    }
        //    if (membershipCardAccountIds != null)
        //    {
        //        cardAccountList = cardAccountList.Where(p => membershipCardAccountIds.Any(t => t == p.Id)).ToList();
        //    }
        //    foreach (var a in cardAccountList)
        //    {
        //        if (a.Type == MembershipCardAccountType.StoredValue)
        //        {
        //            string value = "0";
        //            string creditLine = "0";
        //            var isExist = _getOrUpdateInfoFromRedisService.CreateAccount(membershipCard.Id.ToString(), a.Id.ToString(), RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE, "", value, creditLine);
        //            if (isExist == "0")
        //            {
        //                var account = new Account
        //                {
        //                    Id = _uniqueIdGeneratorService.Next(),
        //                    MembershipCardAccountId = a.Id,
        //                    AccountType = a.Type,
        //                    MembershipCardId = membershipCard.Id,
        //                    Value = 0,
        //                    CreditLine = 0,
        //                    Version = new byte[] { 1, 0, 0, 0 }
        //                };
        //                accountList.Add(account);
        //                if (accountList.Any())
        //                {
        //                    _context.Account.AddRange(accountList);
        //                    _context.SaveChanges();
        //                }
        //            }
        //            else
        //            {
        //                var acc = _context.Account.FirstOrDefault(p => p.MembershipCardId == membershipCard.Id && p.MembershipCardAccountId == a.Id);
        //                if (acc != null)
        //                    accountList.Add(acc);
        //            }
        //        }


        //    }

        //    return accountList;
        //}
        public void RegularValueJob()
        {
            try
            {
                List<Account> accounts = new List<Account>();
                List<Transaction> transactions = new List<Transaction>();
                List<StoredValueAccountHistory> storedValueAccountHistories = new List<StoredValueAccountHistory>();
                List<MembershipCardTransaction> membershipCardTransactions = new List<MembershipCardTransaction>();
                // 查询会员相关信息
                //查询会员相关信息
                //var membershipcardList = (from r in _context.RegularValueStorageRule.AsNoTracking()
                //                          join t in _context.MembershipCard.AsNoTracking()
                //                          on new { typeId = r.MembershipCardTypeId, levelId = r.MembershipCardLevelId }
                //                          equals new { typeId = t.MembershipCardTypeId, levelId = t.MembershipCardLevelId }

                //                          join a in _context.Account.DefaultIfEmpty()
                //                          on new { cardId = t.Id, cardAccountId = r.MembershipCardAccountId }
                //                          equals new { cardId = a.MembershipCardId, cardAccountId = a.MembershipCardAccountId }
                //                          select new
                //                          {

                //                              a.Id,
                //                              a.MembershipCardId,
                //                              t.ProfileId,
                //                              t.HotelCode,
                //                              t.MembershipCardNumber,
                //                              r.Amount,
                //                              r.Config,
                //                              r.MembershipCardAccountId,
                //                              r.MembershipCardLevelId,
                //                              r.MembershipCardTypeId
                //                          }).ToList();


                var membershipcardList = (from r in _context.RegularValueStorageRule
                                          join t in _context.MembershipCard
                                          on new { typeId = r.MembershipCardTypeId, levelId = r.MembershipCardLevelId } equals new { typeId = t.MembershipCardTypeId, levelId = t.MembershipCardLevelId }
                                          join a in _context.Account on new { cardId = t.Id, cardAccountId = r.MembershipCardAccountId } equals new { cardId = a.MembershipCardId, cardAccountId = a.MembershipCardAccountId } into tb
                                          from a in tb.DefaultIfEmpty()
                                          select new
                                          {
                                              a.Id,
                                              //a.MembershipCardId,
                                              MembershipCardId = t.Id,
                                              t.ProfileId,
                                              t.HotelCode,
                                              t.MembershipCardNumber,
                                              r.Amount,
                                              r.Config,
                                              r.MembershipCardAccountId,
                                              r.MembershipCardLevelId,
                                              r.MembershipCardTypeId,

                                          }).ToList();
                //membershipcardList = membershipcardList.Where(m => m.MembershipCardNumber == "HT0220000311").ToList();
                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var aa = _context.MembershipCardAccount.Where(t => t.Type == MembershipCardAccountType.StoredValue && t.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount).Select(t => t.MembershipCardTypeId).ToList();
                membershipcardList = membershipcardList.Where(t => !aa.Contains(t.MembershipCardTypeId)).ToList();
                foreach (var item in membershipcardList)
                {
                    var regularValueStorageRulesConfig = System.Text.Json.JsonSerializer.Deserialize<RegularValueStorageRulesConfig>(item.Config, jsonSerializerOptions);
                    //有储值规则
                    if (regularValueStorageRulesConfig.storedfrequency != null)
                    {
                        if (membershipcardList.Any())
                        {
                            if (regularValueStorageRulesConfig.storedfrequency.type == "Day")
                            {
                                var membershipCard = _context.MembershipCard.FirstOrDefault(p => p.Id == item.MembershipCardId);
                                var transactionId = _uniqueIdGeneratorService.Next();
                                var membershipCardTransactionId = _uniqueIdGeneratorService.Next();                                

                                //创建账户
                                var accountList = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.StoredValue, new List<long> { item.MembershipCardAccountId }).FirstOrDefault();
                                if(accountList.Code == "ERROR")
                                {
                                    _logger.LogError(accountList.Name);
                                    continue;
                                }
                                var creditLimitAccount = (from a in _context.Account
                                                          join m in _context.MembershipCardAccount
                                                           on a.MembershipCardAccountId equals m.Id into temp
                                                          from c in temp.DefaultIfEmpty()
                                                          where a.MembershipCardId == membershipCard.Id &&
                                                          a.AccountType == MembershipCardAccountType.StoredValue &&
                                                          c.Type == MembershipCardAccountType.StoredValue &&
                                                          c.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount
                                                          select a).FirstOrDefault();   //获取 当前会员卡下的透支账户
                                var tempAccountList = accountList.Accounts;
                                string[] luaResult;
                                if (tempAccountList.Any())
                                {
                                    string jsonStr = String.Empty;
                                    if (creditLimitAccount == null)
                                    {
                                        
                                        jsonStr += "[";
                                        
                                        jsonStr += "{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + item.Amount.ToString() + "\"}";
                                        jsonStr += "]";
                                        luaResult = _getOrUpdateInfoFromRedisService.ChangeStoredValueAccountBalance(item.MembershipCardId, jsonStr);
                                        if (luaResult[0] == "Err")
                                        {
                                            _logger.LogError("redis exist overdraft.");
                                        }
                                    }
                                    else
                                    {
                                        jsonStr = "[{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + item.Amount.ToString() + "\"}";                                        
                                        jsonStr += "]";
                                        luaResult = _getOrUpdateInfoFromRedisService.AddStoredValueAccountBalanceExistOverdraft(item.MembershipCardId, jsonStr);
                                        if (luaResult[0] == "Err")
                                        {
                                            _logger.LogError("redis not exist overdraft.");
                                        }
                                    }
                                    

                                    var resultArr = (JArray)JsonConvert.DeserializeObject(luaResult[1]);

                                    decimal totalLastBalance = Convert.ToDecimal(luaResult[3]);
                                    decimal totalThisBalance = Convert.ToDecimal(luaResult[2]);
                                    //var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                    //更新账户
                                    for (int i = 0; i < resultArr.Count; i++)
                                    {
                                        var accountTransactionAmount = Convert.ToDecimal(resultArr[i]["transactionAmount"]);
                                        var thisBalance = Convert.ToDecimal(resultArr[i]["balance"]);
                                        var lastBalance = thisBalance - accountTransactionAmount;
                                        //更新账户在Redis里面处理了
                                        //decimal currentCardValue = 0;
                                        //var account = _context.Account.Where(a => a.Id == item.Id).First();
                                        var membershipCardAccountId = Convert.ToInt64(resultArr[i]["membershipCardAccountId"]);
                                        var account = tempAccountList.Where(m => m.MembershipCardAccountId == membershipCardAccountId).FirstOrDefault();
                                        //currentCardValue = account.Value;
                                        //account.Value = account.Value + item.Amount;
                                        //account.UpdateDate = DateTime.Now;
                                        //account.UpdateUser = "LPSWorker";
                                        //accounts.Add(account);

                                        var storedValueAccountHistory = new StoredValueAccountHistory()
                                        {
                                            Id = _uniqueIdGeneratorService.Next(),
                                            AccountId = account.Id,
                                            MembershipCardTypeId = item.MembershipCardTypeId,
                                            MembershipCardAccountId = membershipCardAccountId,
                                            MembershipCardId = item.MembershipCardId,
                                            ProfileId = item.ProfileId.Value,
                                            MembershipCardNumber = item.MembershipCardNumber,
                                            TransactionDate = DateTime.Now,
                                            Amount = item.Amount,
                                            OperationType = 0,
                                            LastBalance = lastBalance,
                                            ThisBalance = thisBalance,
                                            ExpireDate = null,
                                            RevenueType = 3,
                                            PlaceCode = "",
                                            Description = "定期调整卡值",
                                            IsVoild = false,
                                            BatchId = Guid.NewGuid(),
                                            IsManual = true,
                                            TransactionId = transactionId,
                                            MembershipCardTransactionId = membershipCardTransactionId,
                                            HotelCode = item.HotelCode,
                                            InsertDate = DateTime.Now,
                                            InsertUser = "LPSWorker",
                                            UpdateDate = DateTime.Now,
                                            UpdateUser = "LPSWorker"
                                        };

                                        storedValueAccountHistories.Add(storedValueAccountHistory);

                                        Transaction transaction = new Transaction()
                                        {
                                            Id = transactionId,
                                            Amount = item.Amount,
                                            TransactionDate = DateTime.Now,
                                            TransactionType = Core.Enum.TransactionType.Topup,
                                            ProfileId = item.ProfileId.Value,
                                            CurrencyCode = "CNY",
                                            Description = "定期调整卡值",
                                            HotelCode = item.HotelCode,
                                            RealAmount = item.Amount,
                                            PlaceCode = "",
                                            TransactionNumber = Guid.NewGuid().ToString(),
                                            MainId = null,
                                            InsertDate = DateTime.Now,
                                            InsertUser = "LPSWorker",
                                            UpdateDate = DateTime.Now,
                                            UpdateUser = "LPSWorker"
                                        };
                                        transactions.Add(transaction);

                                        var membershipCardTransaction = new MembershipCardTransaction()
                                        {
                                            Id = membershipCardTransactionId,
                                            TransactionId = transactionId,
                                            MembershipCardId = item.MembershipCardId,
                                            Amount = item.Amount,
                                            RealAmount = 0,
                                            LastBalance = totalLastBalance,
                                            ThisBalance = totalThisBalance,
                                            Points = 0,
                                            MainMembershipCardId = item.MembershipCardId,
                                            InsertDate = DateTime.Now,
                                            InsertUser = "LPSWorker",
                                            UpdateDate = DateTime.Now,
                                            UpdateUser = "LPSWorker"
                                        };

                                        membershipCardTransactions.Add(membershipCardTransaction);
                                    }
                                }
                                else
                                {
                                    _logger.LogError("tempAccountList为空，没有进行调整卡值的卡号："+membershipCard.MembershipCardNumber);
                                }
                            }
                            if (regularValueStorageRulesConfig.storedfrequency.type == "Week")
                            {
                                if (regularValueStorageRulesConfig.storedfrequency.value == (int)DateTime.Now.DayOfWeek)
                                {
                                    var membershipCard = _context.MembershipCard.FirstOrDefault(p => p.Id == item.MembershipCardId);
                                    var transactionId = _uniqueIdGeneratorService.Next();
                                    var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                    //创建账户
                                    var accountList = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.StoredValue, new List<long> { item.MembershipCardAccountId }).FirstOrDefault();
                                    if(accountList.Code == "ERROR")
                                    {
                                        _logger.LogError(accountList.Name);
                                        continue;
                                    }
                                    var creditLimitAccount = (from a in _context.Account
                                                              join m in _context.MembershipCardAccount
                                                               on a.MembershipCardAccountId equals m.Id into temp
                                                              from c in temp.DefaultIfEmpty()
                                                              where a.MembershipCardId == membershipCard.Id &&
                                                              a.AccountType == MembershipCardAccountType.StoredValue &&
                                                              c.Type == MembershipCardAccountType.StoredValue &&
                                                              c.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount
                                                              select a).FirstOrDefault();   //获取 当前会员卡下的透支账户
                                    var tempAccountList = accountList.Accounts;
                                    string[] luaResult;
                                    if (tempAccountList.Any())
                                    {
                                        string jsonStr = String.Empty;
                                        
                                        if (creditLimitAccount == null)
                                        {
                                            jsonStr += "[";
                                            
                                            jsonStr += "{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + item.Amount.ToString() + "\"}";
                                            jsonStr += "]";
                                            luaResult = _getOrUpdateInfoFromRedisService.ChangeStoredValueAccountBalance(item.MembershipCardId, jsonStr);
                                            if (luaResult[0] == "Err")
                                            {
                                                _logger.LogError("redis exist overdraft.");
                                            }
                                        }
                                        else
                                        {
                                            jsonStr = "[{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + item.Amount.ToString() + "\"}";
                                            jsonStr += "]";
                                            luaResult = _getOrUpdateInfoFromRedisService.AddStoredValueAccountBalanceExistOverdraft(item.MembershipCardId, jsonStr);
                                            if (luaResult[0] == "Err")
                                            {
                                                _logger.LogError("redis not exist overdraft.");
                                            }
                                        }
                                        var resultArr = (JArray)JsonConvert.DeserializeObject(luaResult[1]);

                                        decimal totalLastBalance = Convert.ToDecimal(luaResult[3]);
                                        decimal totalThisBalance = Convert.ToDecimal(luaResult[2]);
                                        //var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                        //更新账户
                                        for (int i = 0; i < resultArr.Count; i++)
                                        {
                                            var accountTransactionAmount = Convert.ToDecimal(resultArr[i]["transactionAmount"]);
                                            var thisBalance = Convert.ToDecimal(resultArr[i]["balance"]);
                                            var lastBalance = thisBalance - accountTransactionAmount;
                                            //更新账户在Redis里面处理了
                                            //decimal currentCardValue = 0;
                                            //var account = _context.Account.Where(a => a.Id == item.Id).First();
                                            //currentCardValue = account.Value;
                                            //account.Value = account.Value + item.Amount;
                                            //account.UpdateDate = DateTime.Now;
                                            //account.UpdateUser = "LPSWorker";
                                            //accounts.Add(account);
                                            var membershipCardAccountId = Convert.ToInt64(resultArr[i]["membershipCardAccountId"]);
                                            var account = tempAccountList.Where(m => m.MembershipCardAccountId == membershipCardAccountId).FirstOrDefault();

                                            var storedValueAccountHistory = new StoredValueAccountHistory()
                                            {
                                                Id = _uniqueIdGeneratorService.Next(),
                                                AccountId = account.Id,
                                                MembershipCardTypeId = item.MembershipCardTypeId,
                                                MembershipCardAccountId = membershipCardAccountId,
                                                MembershipCardId = item.MembershipCardId,
                                                ProfileId = item.ProfileId.Value,
                                                MembershipCardNumber = item.MembershipCardNumber,
                                                TransactionDate = DateTime.Now,
                                                Amount = item.Amount,
                                                OperationType = 0,
                                                LastBalance = lastBalance,
                                                ThisBalance = thisBalance,
                                                ExpireDate = null,
                                                RevenueType = 3,
                                                PlaceCode = "",
                                                Description = "定期调整卡值",
                                                IsVoild = false,
                                                BatchId = Guid.NewGuid(),
                                                IsManual = true,
                                                TransactionId = transactionId,
                                                MembershipCardTransactionId = membershipCardTransactionId,
                                                HotelCode = item.HotelCode,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };

                                            storedValueAccountHistories.Add(storedValueAccountHistory);

                                            Transaction transaction = new Transaction()
                                            {
                                                Id = transactionId,
                                                Amount = item.Amount,
                                                TransactionDate = DateTime.Now,
                                                TransactionType = Core.Enum.TransactionType.Topup,
                                                ProfileId = item.ProfileId.Value,
                                                CurrencyCode = "CNY",
                                                Description = "定期调整卡值",
                                                HotelCode = item.HotelCode,
                                                RealAmount = item.Amount,
                                                PlaceCode = "",
                                                TransactionNumber = Guid.NewGuid().ToString(),
                                                MainId = null,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };
                                            transactions.Add(transaction);

                                            var membershipCardTransaction = new MembershipCardTransaction()
                                            {
                                                Id = membershipCardTransactionId,
                                                TransactionId = transactionId,
                                                MembershipCardId = item.MembershipCardId,
                                                Amount = item.Amount,
                                                RealAmount = 0,
                                                LastBalance = totalLastBalance,
                                                ThisBalance = totalThisBalance,
                                                Points = 0,
                                                MainMembershipCardId = item.MembershipCardId,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };

                                            membershipCardTransactions.Add(membershipCardTransaction);
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogError("tempAccountList为空，没有进行调整卡值的卡号：" + membershipCard.MembershipCardNumber);
                                    }
                                }
                               
                            }
                           

                            if (regularValueStorageRulesConfig.storedfrequency.type == "Month")
                            {
                                var datetime = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                               int monthday = 0;
                                if (regularValueStorageRulesConfig.storedfrequency.value.ToString().StartsWith("-")) //如果是倒数逻辑
                                {
                                    monthday = (datetime + 1) - Math.Abs(regularValueStorageRulesConfig.storedfrequency.value);
                                }
                                if (regularValueStorageRulesConfig.storedfrequency.value == DateTime.Now.Day || DateTime.Now.Day == monthday)
                                {

                                    var membershipCard = _context.MembershipCard.FirstOrDefault(p => p.Id == item.MembershipCardId);
                                    var transactionId = _uniqueIdGeneratorService.Next();
                                    var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                    //创建账户
                                    var accountList = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.StoredValue, new List<long> { item.MembershipCardAccountId }).FirstOrDefault();

                                    if (accountList.Code == "ERROR")
                                    {
                                        _logger.LogError(accountList.Name);
                                        continue;
                                    }
                                    var creditLimitAccount = (from a in _context.Account
                                                              join m in _context.MembershipCardAccount
                                                               on a.MembershipCardAccountId equals m.Id into temp
                                                              from c in temp.DefaultIfEmpty()
                                                              where a.MembershipCardId == membershipCard.Id &&
                                                              a.AccountType == MembershipCardAccountType.StoredValue &&
                                                              c.Type == MembershipCardAccountType.StoredValue &&
                                                              c.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount
                                                              select a).FirstOrDefault();   //获取 当前会员卡下的透支账户
                                    var tempAccountList = accountList.Accounts;
                                    string[] luaResult;
                                    if (tempAccountList.Any())
                                    {
                                        string jsonStr = String.Empty;

                                        if (creditLimitAccount == null)
                                        {
                                            jsonStr += "[";

                                            jsonStr += "{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + item.Amount.ToString() + "\"}";
                                            jsonStr += "]";
                                            luaResult = _getOrUpdateInfoFromRedisService.ChangeStoredValueAccountBalance(item.MembershipCardId, jsonStr);
                                            if (luaResult[0] == "Err")
                                            {
                                                _logger.LogError("redis exist overdraft.");
                                            }
                                        }
                                        else
                                        {
                                            jsonStr = "[{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + item.Amount.ToString() + "\"}";
                                            jsonStr += "]";
                                            luaResult = _getOrUpdateInfoFromRedisService.AddStoredValueAccountBalanceExistOverdraft(item.MembershipCardId, jsonStr);
                                            if (luaResult[0] == "Err")
                                            {
                                                _logger.LogError("redis not exist overdraft.");
                                            }
                                        }
                                        var resultArr = (JArray)JsonConvert.DeserializeObject(luaResult[1]);

                                        decimal totalLastBalance = Convert.ToDecimal(luaResult[3]);
                                        decimal totalThisBalance = Convert.ToDecimal(luaResult[2]);
                                        //var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                        //更新账户
                                        for (int i = 0; i < resultArr.Count; i++)
                                        {
                                            var accountTransactionAmount = Convert.ToDecimal(resultArr[i]["transactionAmount"]);
                                            var thisBalance = Convert.ToDecimal(resultArr[i]["balance"]);
                                            var lastBalance = thisBalance - accountTransactionAmount;
                                            //更新账户在Redis里面处理了
                                            //decimal currentCardValue = 0;
                                            //var account = _context.Account.Where(a => a.Id == item.Id).First();
                                            //currentCardValue = account.Value;
                                            //account.Value = account.Value + item.Amount;
                                            //account.UpdateDate = DateTime.Now;
                                            //account.UpdateUser = "LPSWorker";
                                            //accounts.Add(account);
                                            var membershipCardAccountId = Convert.ToInt64(resultArr[i]["membershipCardAccountId"]);
                                            var account = tempAccountList.Where(m => m.MembershipCardAccountId == membershipCardAccountId).FirstOrDefault();
                                            var storedValueAccountHistory = new StoredValueAccountHistory()
                                            {
                                                Id = _uniqueIdGeneratorService.Next(),
                                                AccountId = account.Id,
                                                MembershipCardTypeId = item.MembershipCardTypeId,
                                                MembershipCardAccountId = membershipCardAccountId,
                                                MembershipCardId = item.MembershipCardId,
                                                ProfileId = item.ProfileId.Value,
                                                MembershipCardNumber = item.MembershipCardNumber,
                                                TransactionDate = DateTime.Now,
                                                Amount = item.Amount,
                                                OperationType = 0,
                                                LastBalance = lastBalance,
                                                ThisBalance = thisBalance,
                                                ExpireDate = null,
                                                RevenueType = 3,
                                                PlaceCode = "",
                                                Description = "定期调整卡值",
                                                IsVoild = false,
                                                BatchId = Guid.NewGuid(),
                                                IsManual = true,
                                                TransactionId = transactionId,
                                                MembershipCardTransactionId = membershipCardTransactionId,
                                                HotelCode = item.HotelCode,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };

                                            storedValueAccountHistories.Add(storedValueAccountHistory);

                                            Transaction transaction = new Transaction()
                                            {
                                                Id = transactionId,
                                                Amount = item.Amount,
                                                TransactionDate = DateTime.Now,
                                                TransactionType = Core.Enum.TransactionType.Topup,
                                                ProfileId = item.ProfileId.Value,
                                                CurrencyCode = "CNY",
                                                Description = "定期调整卡值",
                                                HotelCode = item.HotelCode,
                                                RealAmount = item.Amount,
                                                PlaceCode = "",
                                                TransactionNumber = Guid.NewGuid().ToString(),
                                                MainId = null,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };
                                            transactions.Add(transaction);

                                            var membershipCardTransaction = new MembershipCardTransaction()
                                            {
                                                Id = membershipCardTransactionId,
                                                TransactionId = transactionId,
                                                MembershipCardId = item.MembershipCardId,
                                                Amount = item.Amount,
                                                RealAmount = 0,
                                                LastBalance = totalLastBalance,
                                                ThisBalance = totalThisBalance,
                                                Points = 0,
                                                MainMembershipCardId = item.MembershipCardId,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };

                                            membershipCardTransactions.Add(membershipCardTransaction);
                                        }
                                    }
                                    else
                                    {
                                    _logger.LogError("tempAccountList为空，没有进行调整卡值的卡号："+membershipCard.MembershipCardNumber);
                                    }

                                }
                            }

                            if (regularValueStorageRulesConfig.storedfrequency.type == "Quarter")
                            {
                                bool result = false;
                                var datetime = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                                var monthday = (datetime + 1) - Math.Abs(regularValueStorageRulesConfig.storedfrequency.value);
                                if (regularValueStorageRulesConfig.storedfrequency.value > 0)
                                    result = GetFirstQuarter(regularValueStorageRulesConfig.storedfrequency.value);
                                else
                                    result = GetLastQuarter(monthday);
                                if (result)
                                {

                                    var membershipCard = _context.MembershipCard.FirstOrDefault(p => p.Id == item.MembershipCardId);
                                    var transactionId = _uniqueIdGeneratorService.Next();
                                    var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                    //创建账户
                                    var accountList = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.StoredValue, new List<long> { item.MembershipCardAccountId }).FirstOrDefault();

                                    if (accountList.Code == "ERROR")
                                    {
                                        _logger.LogError(accountList.Name);
                                        continue;
                                    }
                                    var creditLimitAccount = (from a in _context.Account
                                                              join m in _context.MembershipCardAccount
                                                               on a.MembershipCardAccountId equals m.Id into temp
                                                              from c in temp.DefaultIfEmpty()
                                                              where a.MembershipCardId == membershipCard.Id &&
                                                              a.AccountType == MembershipCardAccountType.StoredValue &&
                                                              c.Type == MembershipCardAccountType.StoredValue &&
                                                              c.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount
                                                              select a).FirstOrDefault();   //获取 当前会员卡下的透支账户
                                    var tempAccountList = accountList.Accounts;
                                    string[] luaResult;
                                    if (tempAccountList.Any())
                                    {
                                        string jsonStr = String.Empty;
                                        if (creditLimitAccount == null)
                                        {

                                            jsonStr += "[";

                                            jsonStr += "{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + item.Amount.ToString() + "\"}";
                                            jsonStr += "]";
                                            luaResult = _getOrUpdateInfoFromRedisService.ChangeStoredValueAccountBalance(item.MembershipCardId, jsonStr);
                                            if (luaResult[0] == "Err")
                                            {
                                                _logger.LogError("redis exist overdraft.");
                                            }
                                        }
                                        else
                                        {
                                            jsonStr = "[{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + item.Amount.ToString() + "\"}";
                                            jsonStr += "]";
                                            luaResult = _getOrUpdateInfoFromRedisService.AddStoredValueAccountBalanceExistOverdraft(item.MembershipCardId, jsonStr);
                                            if (luaResult[0] == "Err")
                                            {
                                                _logger.LogError("redis not exist overdraft.");
                                            }
                                        }
                                        var resultArr = (JArray)JsonConvert.DeserializeObject(luaResult[1]);

                                        decimal totalLastBalance = Convert.ToDecimal(luaResult[3]);
                                        decimal totalThisBalance = Convert.ToDecimal(luaResult[2]);
                                        //var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                        //更新账户
                                        for (int i = 0; i < resultArr.Count; i++)
                                        {
                                            var accountTransactionAmount = Convert.ToDecimal(resultArr[i]["transactionAmount"]);
                                            var thisBalance = Convert.ToDecimal(resultArr[i]["balance"]);
                                            var lastBalance = thisBalance - accountTransactionAmount;
                                            //更新账户在Redis里面处理了
                                            //decimal currentCardValue = 0;
                                            //var account = _context.Account.Where(a => a.Id == item.Id).First();
                                            //currentCardValue = account.Value;
                                            //account.Value = account.Value + item.Amount;
                                            //account.UpdateDate = DateTime.Now;
                                            //account.UpdateUser = "LPSWorker";
                                            //accounts.Add(account);
                                            var membershipCardAccountId = Convert.ToInt64(resultArr[i]["membershipCardAccountId"]);
                                            var account = tempAccountList.Where(m => m.MembershipCardAccountId == membershipCardAccountId).FirstOrDefault();
                                            var storedValueAccountHistory = new StoredValueAccountHistory()
                                            {
                                                Id = _uniqueIdGeneratorService.Next(),
                                                AccountId = account.Id,
                                                MembershipCardTypeId = item.MembershipCardTypeId,
                                                MembershipCardAccountId = membershipCardAccountId,
                                                MembershipCardId = item.MembershipCardId,
                                                ProfileId = item.ProfileId.Value,
                                                MembershipCardNumber = item.MembershipCardNumber,
                                                TransactionDate = DateTime.Now,
                                                Amount = item.Amount,
                                                OperationType = 0,
                                                LastBalance = lastBalance,
                                                ThisBalance = thisBalance,
                                                ExpireDate = null,
                                                RevenueType = 3,
                                                PlaceCode = "",
                                                Description = "定期调整卡值",
                                                IsVoild = false,
                                                BatchId = Guid.NewGuid(),
                                                IsManual = true,
                                                TransactionId = transactionId,
                                                MembershipCardTransactionId = membershipCardTransactionId,
                                                HotelCode = item.HotelCode,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };

                                            storedValueAccountHistories.Add(storedValueAccountHistory);

                                            Transaction transaction = new Transaction()
                                            {
                                                Id = transactionId,
                                                Amount = item.Amount,
                                                TransactionDate = DateTime.Now,
                                                TransactionType = Core.Enum.TransactionType.Topup,
                                                ProfileId = item.ProfileId.Value,
                                                CurrencyCode = "CNY",
                                                Description = "定期调整卡值",
                                                HotelCode = item.HotelCode,
                                                RealAmount = item.Amount,
                                                PlaceCode = "",
                                                TransactionNumber = Guid.NewGuid().ToString(),
                                                MainId = null,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };
                                            transactions.Add(transaction);

                                            var membershipCardTransaction = new MembershipCardTransaction()
                                            {
                                                Id = membershipCardTransactionId,
                                                TransactionId = transactionId,
                                                MembershipCardId = item.MembershipCardId,
                                                Amount = item.Amount,
                                                RealAmount = 0,
                                                LastBalance = totalLastBalance,
                                                ThisBalance = totalThisBalance,
                                                Points = 0,
                                                MainMembershipCardId = item.MembershipCardId,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };

                                            membershipCardTransactions.Add(membershipCardTransaction);
                                        }
                                    }
                                    else
                                    {
                                    _logger.LogError("tempAccountList为空，没有进行调整卡值的卡号："+membershipCard.MembershipCardNumber);
                                    }

                                }
                            }

                        }
                    }


                }

                //_context.Account.UpdateRange(accounts);
                _context.StoredValueAccountHistory.AddRange(storedValueAccountHistories);
                _context.Transaction.AddRange(transactions);
                _context.MembershipCardTransaction.AddRange(membershipCardTransactions);
                _context.SaveChanges();





            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public void RestValueJob()
        {
            try
            {
                List<Account> accounts = new List<Account>();
                List<Transaction> transactions = new List<Transaction>();
                List<StoredValueAccountHistory> storedValueAccountHistories = new List<StoredValueAccountHistory>();
                List<MembershipCardTransaction> membershipCardTransactions = new List<MembershipCardTransaction>();
                //查询会员相关信息
                //var membershipcardList = (from r in _context.RegularValueStorageRule.AsNoTracking()
                //                          join t in _context.MembershipCard.AsNoTracking()
                //                          on new { typeId = r.MembershipCardTypeId, levelId = r.MembershipCardLevelId }
                //                          equals new { typeId = t.MembershipCardTypeId, levelId = t.MembershipCardLevelId }
                //                          join a in _context.Account.AsNoTracking()
                //                          on new { cardId = t.Id, cardAccountId = r.MembershipCardAccountId }
                //                          equals new { cardId = a.MembershipCardId, cardAccountId = a.MembershipCardAccountId }
                //                          select new { a.Id, a.MembershipCardId, t.ProfileId, t.HotelCode, t.MembershipCardNumber, r.Amount, r.Config, r.MembershipCardAccountId, r.MembershipCardLevelId, r.MembershipCardTypeId }).ToList();
                var membershipcardList = (from r in _context.RegularValueStorageRule
                                          join t in _context.MembershipCard
                                          on new { typeId = r.MembershipCardTypeId, levelId = r.MembershipCardLevelId } equals new { typeId = t.MembershipCardTypeId, levelId = t.MembershipCardLevelId }
                                          join a in _context.Account on new { cardId = t.Id, cardAccountId = r.MembershipCardAccountId } equals new { cardId = a.MembershipCardId, cardAccountId = a.MembershipCardAccountId } into tb
                                          from a in tb.DefaultIfEmpty()
                                          select new
                                          {
                                              a.Id,
                                              //a.MembershipCardId,
                                              MembershipCardId = t.Id,
                                              t.ProfileId,
                                              t.HotelCode,
                                              t.MembershipCardNumber,
                                              r.Amount,
                                              r.Config,
                                              r.MembershipCardAccountId,
                                              r.MembershipCardLevelId,
                                              r.MembershipCardTypeId,

                                          }).ToList();
                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                //判断会员以及账户信息是否有，如果有就去实现
                foreach (var item in membershipcardList)
                {
                    var RestValueStorageRulesConfig = System.Text.Json.JsonSerializer.Deserialize<RegularValueStorageRulesConfig>(item.Config, jsonSerializerOptions);
                    //有清零规则
                    if (RestValueStorageRulesConfig.eliminatefrequency != null)
                    {
                        if (membershipcardList.Any())
                        {


                            if (RestValueStorageRulesConfig.eliminatefrequency.type == "Day")
                            {
                                var membershipCard = _context.MembershipCard.FirstOrDefault(p => p.Id == item.MembershipCardId);
                                var transactionId = _uniqueIdGeneratorService.Next();
                                var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                //创建账户
                                var accountList = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.StoredValue, new List<long> { item.MembershipCardAccountId }).FirstOrDefault();

                                if (accountList.Code == "ERROR")
                                {
                                    _logger.LogError(accountList.Name);
                                    continue;
                                }
                                var creditLimitAccount = (from a in _context.Account
                                                          join m in _context.MembershipCardAccount
                                                           on a.MembershipCardAccountId equals m.Id into temp
                                                          from c in temp.DefaultIfEmpty()
                                                          where a.MembershipCardId == membershipCard.Id &&
                                                          a.AccountType == MembershipCardAccountType.StoredValue &&
                                                          c.Type == MembershipCardAccountType.StoredValue &&
                                                          c.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount
                                                          select a).FirstOrDefault();   //获取 当前会员卡下的透支账户
                                var tempAccountList = accountList.Accounts;
                                string[] luaResult;

                                if (tempAccountList.Any())
                                {
                                    string jsonStr = String.Empty;
                                    var accountValue = _getOrUpdateInfoFromRedisService.GetRedisAccountInfo(item.MembershipCardId, item.MembershipCardAccountId, RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE);
                                    decimal value = Convert.ToDecimal(accountValue[0]);
                                    if (creditLimitAccount == null)
                                    {
                                        jsonStr += "[";
                                        jsonStr += "{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + -value + "\"}";
                                        jsonStr += "]";
                                        luaResult = _getOrUpdateInfoFromRedisService.ChangeStoredValueAccountBalance(item.MembershipCardId, jsonStr);
                                        if (luaResult[0] == "Err")
                                        {
                                            _logger.LogError("redis exist overdraft.");
                                        }
                                    }
                                    else
                                    {
                                        jsonStr = "[{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + item.Amount.ToString() + "\"}";
                                        jsonStr += "]";
                                        luaResult = _getOrUpdateInfoFromRedisService.AddStoredValueAccountBalanceExistOverdraft(item.MembershipCardId, jsonStr);
                                        if (luaResult[0] == "Err")
                                        {
                                            _logger.LogError("redis not exist overdraft.");
                                        }
                                    }
                                    var resultArr = (JArray)JsonConvert.DeserializeObject(luaResult[1]);

                                    decimal totalLastBalance = Convert.ToDecimal(luaResult[3]);
                                    decimal totalThisBalance = Convert.ToDecimal(luaResult[2]);
                                    //var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                    //更新账户
                                    for (int i = 0; i < resultArr.Count; i++)
                                    {
                                        var accountTransactionAmount = Convert.ToDecimal(resultArr[i]["transactionAmount"]);
                                        var thisBalance = Convert.ToDecimal(resultArr[i]["balance"]);
                                        var lastBalance = thisBalance - accountTransactionAmount;
                                        //更新账户在Redis里面处理了
                                        //decimal currentCardValue = 0;
                                        //var account = _context.Account.Where(a => a.Id == item.Id).First();
                                        //currentCardValue = account.Value;
                                        //account.Value = account.Value + item.Amount;
                                        //account.UpdateDate = DateTime.Now;
                                        //account.UpdateUser = "LPSWorker";
                                        //accounts.Add(account);
                                        var membershipCardAccountId = Convert.ToInt64(resultArr[i]["membershipCardAccountId"]);
                                        var account = tempAccountList.Where(m => m.MembershipCardAccountId == membershipCardAccountId).FirstOrDefault();
                                        var storedValueAccountHistory = new StoredValueAccountHistory()
                                        {
                                            Id = _uniqueIdGeneratorService.Next(),
                                            AccountId = account.Id,
                                            MembershipCardTypeId = item.MembershipCardTypeId,
                                            MembershipCardAccountId = membershipCardAccountId,
                                            MembershipCardId = item.MembershipCardId,
                                            ProfileId = item.ProfileId.Value,
                                            MembershipCardNumber = item.MembershipCardNumber,
                                            TransactionDate = DateTime.Now,
                                            Amount = -value,
                                            OperationType = 9,
                                            LastBalance = lastBalance,
                                            ThisBalance = thisBalance,
                                            ExpireDate = null,
                                            RevenueType = 3,
                                            PlaceCode = "",
                                            Description = "定期调整卡值",
                                            IsVoild = false,
                                            BatchId = Guid.NewGuid(),
                                            IsManual = true,
                                            TransactionId = transactionId,
                                            MembershipCardTransactionId = membershipCardTransactionId,
                                            HotelCode = item.HotelCode,
                                            InsertDate = DateTime.Now,
                                            InsertUser = "LPSWorker",
                                            UpdateDate = DateTime.Now,
                                            UpdateUser = "LPSWorker"
                                        };

                                        storedValueAccountHistories.Add(storedValueAccountHistory);

                                        Transaction transaction = new Transaction()
                                        {
                                            Id = transactionId,
                                            Amount = -value,
                                            TransactionDate = DateTime.Now,
                                            TransactionType = Core.Enum.TransactionType.Expired,
                                            ProfileId = item.ProfileId.Value,
                                            CurrencyCode = "CNY",
                                            Description = "定期调整卡值",
                                            HotelCode = item.HotelCode,
                                            RealAmount = -value,
                                            PlaceCode = "",
                                            TransactionNumber = Guid.NewGuid().ToString(),
                                            MainId = null,
                                            InsertDate = DateTime.Now,
                                            InsertUser = "LPSWorker",
                                            UpdateDate = DateTime.Now,
                                            UpdateUser = "LPSWorker"
                                        };
                                        transactions.Add(transaction);

                                        var membershipCardTransaction = new MembershipCardTransaction()
                                        {
                                            Id = membershipCardTransactionId,
                                            TransactionId = transactionId,
                                            MembershipCardId = item.MembershipCardId,
                                            Amount = -value,
                                            RealAmount = 0,
                                            LastBalance = totalLastBalance,
                                            ThisBalance = totalThisBalance,
                                            Points = 0,
                                            MainMembershipCardId = item.MembershipCardId,
                                            InsertDate = DateTime.Now,
                                            InsertUser = "LPSWorker",
                                            UpdateDate = DateTime.Now,
                                            UpdateUser = "LPSWorker"
                                        };

                                        membershipCardTransactions.Add(membershipCardTransaction);
                                    }
                                }
                            }
                            if (RestValueStorageRulesConfig.eliminatefrequency.type == "Week")
                            {
                                if (RestValueStorageRulesConfig.eliminatefrequency.value == (int)DateTime.Now.DayOfWeek)
                                {
                                    var membershipCard = _context.MembershipCard.FirstOrDefault(p => p.Id == item.MembershipCardId);
                                    var transactionId = _uniqueIdGeneratorService.Next();
                                    var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                    //创建账户
                                    var accountList = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.StoredValue, new List<long> { item.MembershipCardAccountId }).FirstOrDefault();

                                    if (accountList.Code == "ERROR")
                                    {
                                        _logger.LogError(accountList.Name);
                                        continue;
                                    }
                                    var creditLimitAccount = (from a in _context.Account
                                                              join m in _context.MembershipCardAccount
                                                               on a.MembershipCardAccountId equals m.Id into temp
                                                              from c in temp.DefaultIfEmpty()
                                                              where a.MembershipCardId == membershipCard.Id &&
                                                              a.AccountType == MembershipCardAccountType.StoredValue &&
                                                              c.Type == MembershipCardAccountType.StoredValue &&
                                                              c.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount
                                                              select a).FirstOrDefault();   //获取 当前会员卡下的透支账户
                                    var tempAccountList = accountList.Accounts;
                                    string[] luaResult;

                                    if (tempAccountList.Any())
                                    {
                                        var accountValue = _getOrUpdateInfoFromRedisService.GetRedisAccountInfo(item.MembershipCardId, item.MembershipCardAccountId, RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE);
                                        decimal value = Convert.ToDecimal(accountValue[0]);
                                        string jsonStr = String.Empty;
                                        if (creditLimitAccount == null)
                                        {
                                            jsonStr += "[";
                                            jsonStr += "{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + -value + "\"}";
                                            jsonStr += "]";
                                            luaResult = _getOrUpdateInfoFromRedisService.ChangeStoredValueAccountBalance(item.MembershipCardId, jsonStr);
                                            if (luaResult[0] == "Err")
                                            {
                                                _logger.LogError("redis exist overdraft.");
                                            }
                                        }
                                        else
                                        {
                                            jsonStr = "[{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + item.Amount.ToString() + "\"}";
                                            jsonStr += "]";
                                            luaResult = _getOrUpdateInfoFromRedisService.AddStoredValueAccountBalanceExistOverdraft(item.MembershipCardId, jsonStr);
                                            if (luaResult[0] == "Err")
                                            {
                                                _logger.LogError("redis not exist overdraft.");
                                            }
                                        }

                                        var resultArr = (JArray)JsonConvert.DeserializeObject(luaResult[1]);

                                        decimal totalLastBalance = Convert.ToDecimal(luaResult[3]);
                                        decimal totalThisBalance = Convert.ToDecimal(luaResult[2]);
                                        //var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                        //更新账户
                                        for (int i = 0; i < resultArr.Count; i++)
                                        {
                                            var accountTransactionAmount = Convert.ToDecimal(resultArr[i]["transactionAmount"]);
                                            var thisBalance = Convert.ToDecimal(resultArr[i]["balance"]);
                                            var lastBalance = thisBalance - accountTransactionAmount;
                                            //更新账户在Redis里面处理了
                                            //decimal currentCardValue = 0;
                                            //var account = _context.Account.Where(a => a.Id == item.Id).First();
                                            //currentCardValue = account.Value;
                                            //account.Value = account.Value + item.Amount;
                                            //account.UpdateDate = DateTime.Now;
                                            //account.UpdateUser = "LPSWorker";
                                            //accounts.Add(account);
                                            var membershipCardAccountId = Convert.ToInt64(resultArr[i]["membershipCardAccountId"]);
                                            var account = tempAccountList.Where(m => m.MembershipCardAccountId == membershipCardAccountId).FirstOrDefault();
                                            var storedValueAccountHistory = new StoredValueAccountHistory()
                                            {
                                                Id = _uniqueIdGeneratorService.Next(),
                                                AccountId = account.Id,
                                                MembershipCardTypeId = item.MembershipCardTypeId,
                                                MembershipCardAccountId = membershipCardAccountId,
                                                MembershipCardId = item.MembershipCardId,
                                                ProfileId = item.ProfileId.Value,
                                                MembershipCardNumber = item.MembershipCardNumber,
                                                TransactionDate = DateTime.Now,
                                                Amount = -value,
                                                OperationType = 9,
                                                LastBalance = lastBalance,
                                                ThisBalance = thisBalance,
                                                ExpireDate = null,
                                                RevenueType = 3,
                                                PlaceCode = "",
                                                Description = "定期调整卡值",
                                                IsVoild = false,
                                                BatchId = Guid.NewGuid(),
                                                IsManual = true,
                                                TransactionId = transactionId,
                                                MembershipCardTransactionId = membershipCardTransactionId,
                                                HotelCode = item.HotelCode,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };

                                            storedValueAccountHistories.Add(storedValueAccountHistory);

                                            Transaction transaction = new Transaction()
                                            {
                                                Id = transactionId,
                                                Amount = -value,
                                                TransactionDate = DateTime.Now,
                                                TransactionType = Core.Enum.TransactionType.Expired,
                                                ProfileId = item.ProfileId.Value,
                                                CurrencyCode = "CNY",
                                                Description = "定期调整卡值",
                                                HotelCode = item.HotelCode,
                                                RealAmount = -value,
                                                PlaceCode = "",
                                                TransactionNumber = Guid.NewGuid().ToString(),
                                                MainId = null,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };
                                            transactions.Add(transaction);

                                            var membershipCardTransaction = new MembershipCardTransaction()
                                            {
                                                Id = membershipCardTransactionId,
                                                TransactionId = transactionId,
                                                MembershipCardId = item.MembershipCardId,
                                                Amount = -value,
                                                RealAmount = 0,
                                                LastBalance = totalLastBalance,
                                                ThisBalance = totalThisBalance,
                                                Points = 0,
                                                MainMembershipCardId = item.MembershipCardId,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };

                                            membershipCardTransactions.Add(membershipCardTransaction);
                                        }
                                    }


                                }
                            }
                            if (RestValueStorageRulesConfig.eliminatefrequency.type == "Month")
                            {
                                var datetime = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                                int monthday = 0;
                                if (RestValueStorageRulesConfig.eliminatefrequency.value.ToString().StartsWith("-"))  
                                {
                                    monthday = (datetime + 1) - Math.Abs(RestValueStorageRulesConfig.eliminatefrequency.value);  
                                }

                                if (RestValueStorageRulesConfig.eliminatefrequency.value == DateTime.Now.Day || DateTime.Now.Day == monthday)
                                {
                                    var membershipCard = _context.MembershipCard.FirstOrDefault(p => p.Id == item.MembershipCardId);
                                    var transactionId = _uniqueIdGeneratorService.Next();
                                    var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                    //创建账户
                                    var accountList = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.StoredValue, new List<long> { item.MembershipCardAccountId }).FirstOrDefault();

                                    if (accountList.Code == "ERROR")
                                    {
                                        _logger.LogError(accountList.Name);
                                        continue;
                                    }
                                    var creditLimitAccount = (from a in _context.Account
                                                              join m in _context.MembershipCardAccount
                                                               on a.MembershipCardAccountId equals m.Id into temp
                                                              from c in temp.DefaultIfEmpty()
                                                              where a.MembershipCardId == membershipCard.Id &&
                                                              a.AccountType == MembershipCardAccountType.StoredValue &&
                                                              c.Type == MembershipCardAccountType.StoredValue &&
                                                              c.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount
                                                              select a).FirstOrDefault();   //获取 当前会员卡下的透支账户
                                    var tempAccountList = accountList.Accounts;
                                    string[] luaResult;

                                    if (tempAccountList.Any())
                                    {
                                        string jsonStr = String.Empty;
                                        var accountValue = _getOrUpdateInfoFromRedisService.GetRedisAccountInfo(item.MembershipCardId, item.MembershipCardAccountId, RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE);
                                        decimal value = Convert.ToDecimal(accountValue[0]);
                                        if (creditLimitAccount == null)
                                        {
                                            jsonStr += "[";
                                            jsonStr += "{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + -value + "\"}";
                                            jsonStr += "]";
                                            luaResult = _getOrUpdateInfoFromRedisService.ChangeStoredValueAccountBalance(item.MembershipCardId, jsonStr);
                                            if (luaResult[0] == "Err")
                                            {
                                                _logger.LogError("redis exist overdraft.");
                                            }
                                        }
                                        else
                                        {
                                            jsonStr = "[{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + item.Amount.ToString() + "\"}";
                                            jsonStr += "]";
                                            luaResult = _getOrUpdateInfoFromRedisService.AddStoredValueAccountBalanceExistOverdraft(item.MembershipCardId, jsonStr);
                                            if (luaResult[0] == "Err")
                                            {
                                                _logger.LogError("redis not exist overdraft.");
                                            }
                                        }
                                        var resultArr = (JArray)JsonConvert.DeserializeObject(luaResult[1]);

                                        decimal totalLastBalance = Convert.ToDecimal(luaResult[3]);
                                        decimal totalThisBalance = Convert.ToDecimal(luaResult[2]);
                                        //var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                        //更新账户
                                        for (int i = 0; i < resultArr.Count; i++)
                                        {
                                            var accountTransactionAmount = Convert.ToDecimal(resultArr[i]["transactionAmount"]);
                                            var thisBalance = Convert.ToDecimal(resultArr[i]["balance"]);
                                            var lastBalance = thisBalance - accountTransactionAmount;
                                            //更新账户在Redis里面处理了
                                            //decimal currentCardValue = 0;
                                            //var account = _context.Account.Where(a => a.Id == item.Id).First();
                                            //currentCardValue = account.Value;
                                            //account.Value = account.Value + item.Amount;
                                            //account.UpdateDate = DateTime.Now;
                                            //account.UpdateUser = "LPSWorker";
                                            //accounts.Add(account);
                                            var membershipCardAccountId = Convert.ToInt64(resultArr[i]["membershipCardAccountId"]);
                                            var account = tempAccountList.Where(m => m.MembershipCardAccountId == membershipCardAccountId).FirstOrDefault();
                                            var storedValueAccountHistory = new StoredValueAccountHistory()
                                            {
                                                Id = _uniqueIdGeneratorService.Next(),
                                                AccountId = account.Id,
                                                MembershipCardTypeId = item.MembershipCardTypeId,
                                                MembershipCardAccountId = membershipCardAccountId,
                                                MembershipCardId = item.MembershipCardId,
                                                ProfileId = item.ProfileId.Value,
                                                MembershipCardNumber = item.MembershipCardNumber,
                                                TransactionDate = DateTime.Now,
                                                Amount = -value,
                                                OperationType = 9,
                                                LastBalance = lastBalance,
                                                ThisBalance = thisBalance,
                                                ExpireDate = null,
                                                RevenueType = 3,
                                                PlaceCode = "",
                                                Description = "定期调整卡值",
                                                IsVoild = false,
                                                BatchId = Guid.NewGuid(),
                                                IsManual = true,
                                                TransactionId = transactionId,
                                                MembershipCardTransactionId = membershipCardTransactionId,
                                                HotelCode = item.HotelCode,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };

                                            storedValueAccountHistories.Add(storedValueAccountHistory);

                                            Transaction transaction = new Transaction()
                                            {
                                                Id = transactionId,
                                                Amount = -value,
                                                TransactionDate = DateTime.Now,
                                                TransactionType = Core.Enum.TransactionType.Expired,
                                                ProfileId = item.ProfileId.Value,
                                                CurrencyCode = "CNY",
                                                Description = "定期调整卡值",
                                                HotelCode = item.HotelCode,
                                                RealAmount = -value,
                                                PlaceCode = "",
                                                TransactionNumber = Guid.NewGuid().ToString(),
                                                MainId = null,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };
                                            transactions.Add(transaction);

                                            var membershipCardTransaction = new MembershipCardTransaction()
                                            {
                                                Id = membershipCardTransactionId,
                                                TransactionId = transactionId,
                                                MembershipCardId = item.MembershipCardId,
                                                Amount = -value,
                                                RealAmount = 0,
                                                LastBalance = totalLastBalance,
                                                ThisBalance = totalThisBalance,
                                                Points = 0,
                                                MainMembershipCardId = item.MembershipCardId,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };

                                            membershipCardTransactions.Add(membershipCardTransaction);
                                        }
                                    }


                                }
                            }
                            if (RestValueStorageRulesConfig.eliminatefrequency.type == "Quarter")
                            {
                                bool result = false;
                                var datetime = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                                var monthday = (datetime + 1) - Math.Abs(RestValueStorageRulesConfig.eliminatefrequency.value);
                                if (RestValueStorageRulesConfig.eliminatefrequency.value > 0)
                                    result = GetFirstQuarter(RestValueStorageRulesConfig.eliminatefrequency.value);
                                else
                                    result = GetLastQuarter(monthday);
                                if (result)
                                {

                                    var membershipCard = _context.MembershipCard.FirstOrDefault(p => p.Id == item.MembershipCardId);
                                    var transactionId = _uniqueIdGeneratorService.Next();
                                    var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                    //创建账户
                                    var accountList = _accountService.RequestAccounts(membershipCard.Id, MembershipCardAccountType.StoredValue, new List<long> { item.MembershipCardAccountId }).FirstOrDefault();

                                    if (accountList.Code == "ERROR")
                                    {
                                        _logger.LogError(accountList.Name);
                                        continue;
                                    }
                                    var creditLimitAccount = (from a in _context.Account
                                                              join m in _context.MembershipCardAccount
                                                               on a.MembershipCardAccountId equals m.Id into temp
                                                              from c in temp.DefaultIfEmpty()
                                                              where a.MembershipCardId == membershipCard.Id &&
                                                              a.AccountType == MembershipCardAccountType.StoredValue &&
                                                              c.Type == MembershipCardAccountType.StoredValue &&
                                                              c.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount
                                                              select a).FirstOrDefault();   //获取 当前会员卡下的透支账户
                                    var tempAccountList = accountList.Accounts;
                                    string[] luaResult;

                                    if (tempAccountList.Any())
                                    {
                                        string jsonStr = String.Empty;
                                        var accountValue = _getOrUpdateInfoFromRedisService.GetRedisAccountInfo(item.MembershipCardId, item.MembershipCardAccountId, RedisLuaScript.ACCOUNT_TYPE_STORED_VALUE);
                                        decimal value = Convert.ToDecimal(accountValue[0]);
                                        if (creditLimitAccount == null)
                                        {
                                            jsonStr += "[";
                                            jsonStr += "{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + -value + "\"}";
                                            jsonStr += "]";
                                            luaResult = _getOrUpdateInfoFromRedisService.ChangeStoredValueAccountBalance(item.MembershipCardId, jsonStr);
                                            if (luaResult[0] == "Err")
                                            {
                                                _logger.LogError("redis exist overdraft.");
                                            }
                                        }
                                        else
                                        {
                                            jsonStr = "[{\"membershipCardAccountId\":\"" + item.MembershipCardAccountId.ToString() + "\",\"transactionAmount\":\"" + item.Amount.ToString() + "\"}";
                                            jsonStr += "]";
                                            luaResult = _getOrUpdateInfoFromRedisService.AddStoredValueAccountBalanceExistOverdraft(item.MembershipCardId, jsonStr);
                                            if (luaResult[0] == "Err")
                                            {
                                                _logger.LogError("redis not exist overdraft.");
                                            }
                                        }
                                        var resultArr = (JArray)JsonConvert.DeserializeObject(luaResult[1]);

                                        decimal totalLastBalance = Convert.ToDecimal(luaResult[3]);
                                        decimal totalThisBalance = Convert.ToDecimal(luaResult[2]);
                                        //var membershipCardTransactionId = _uniqueIdGeneratorService.Next();
                                        //更新账户
                                        for (int i = 0; i < resultArr.Count; i++)
                                        {
                                            var accountTransactionAmount = Convert.ToDecimal(resultArr[i]["transactionAmount"]);
                                            var thisBalance = Convert.ToDecimal(resultArr[i]["balance"]);
                                            var lastBalance = thisBalance - accountTransactionAmount;
                                            //更新账户在Redis里面处理了
                                            //decimal currentCardValue = 0;
                                            //var account = _context.Account.Where(a => a.Id == item.Id).First();
                                            //currentCardValue = account.Value;
                                            //account.Value = account.Value + item.Amount;
                                            //account.UpdateDate = DateTime.Now;
                                            //account.UpdateUser = "LPSWorker";
                                            //accounts.Add(account);
                                            var membershipCardAccountId = Convert.ToInt64(resultArr[i]["membershipCardAccountId"]);
                                            var account = tempAccountList.Where(m => m.MembershipCardAccountId == membershipCardAccountId).FirstOrDefault();
                                            var storedValueAccountHistory = new StoredValueAccountHistory()
                                            {
                                                Id = _uniqueIdGeneratorService.Next(),
                                                AccountId = account.Id,
                                                MembershipCardTypeId = item.MembershipCardTypeId,
                                                MembershipCardAccountId = membershipCardAccountId,
                                                MembershipCardId = item.MembershipCardId,
                                                ProfileId = item.ProfileId.Value,
                                                MembershipCardNumber = item.MembershipCardNumber,
                                                TransactionDate = DateTime.Now,
                                                Amount = -value,
                                                OperationType = 9,
                                                LastBalance = lastBalance,
                                                ThisBalance = thisBalance,
                                                ExpireDate = null,
                                                RevenueType = 3,
                                                PlaceCode = "",
                                                Description = "定期调整卡值",
                                                IsVoild = false,
                                                BatchId = Guid.NewGuid(),
                                                IsManual = true,
                                                TransactionId = transactionId,
                                                MembershipCardTransactionId = membershipCardTransactionId,
                                                HotelCode = item.HotelCode,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };

                                            storedValueAccountHistories.Add(storedValueAccountHistory);

                                            Transaction transaction = new Transaction()
                                            {
                                                Id = transactionId,
                                                Amount = -value,
                                                TransactionDate = DateTime.Now,
                                                TransactionType = Core.Enum.TransactionType.Expired,
                                                ProfileId = item.ProfileId.Value,
                                                CurrencyCode = "CNY",
                                                Description = "定期调整卡值",
                                                HotelCode = item.HotelCode,
                                                RealAmount = -value,
                                                PlaceCode = "",
                                                TransactionNumber = Guid.NewGuid().ToString(),
                                                MainId = null,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };
                                            transactions.Add(transaction);

                                            var membershipCardTransaction = new MembershipCardTransaction()
                                            {
                                                Id = membershipCardTransactionId,
                                                TransactionId = transactionId,
                                                MembershipCardId = item.MembershipCardId,
                                                Amount = -value,
                                                RealAmount = 0,
                                                LastBalance = totalLastBalance,
                                                ThisBalance = totalThisBalance,
                                                Points = 0,
                                                MainMembershipCardId = item.MembershipCardId,
                                                InsertDate = DateTime.Now,
                                                InsertUser = "LPSWorker",
                                                UpdateDate = DateTime.Now,
                                                UpdateUser = "LPSWorker"
                                            };

                                            membershipCardTransactions.Add(membershipCardTransaction);
                                        }
                                    }

                                }
                            }

                        }
                    }



                }

                //_context.Account.UpdateRange(accounts);
                _context.StoredValueAccountHistory.AddRange(storedValueAccountHistories);
                _context.Transaction.AddRange(transactions);
                _context.MembershipCardTransaction.AddRange(membershipCardTransactions);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}


