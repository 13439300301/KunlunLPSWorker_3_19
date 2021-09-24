using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.Domain.DivisionCards;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kunlun.LPS.Worker.Services.DivisionCards
{
    public class DivisionCardService : IDivisionCardService
    {
        private readonly ILogger<DivisionCardService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IConfigurationService<MembershipCardType> _membershipCardTypeService;
        private readonly IConfigurationService<MembershipCardAccount> _membershipCardAccountService;

        public DivisionCardService(
            ILogger<DivisionCardService> logger,
            LPSWorkerContext context,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            IConfigurationService<MembershipCardType> membershipCardTypeService,
            IConfigurationService<MembershipCardAccount> membershipCardAccountService)
        {
            _logger = logger;
            _context = context;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _membershipCardTypeService = membershipCardTypeService;
            _membershipCardAccountService = membershipCardAccountService;

            _logger.LogDebug(nameof(DivisionCardService));
        }


        /// <summary>
        /// 分裂卡扣减卡值
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="storedValueAccountId"></param>
        public void DeductCardFee(long? transactionId, long? storedValueAccountId = null)
        {
            #region 获取基本信息
            DivisionCardRequest divisionCardRequest = new DivisionCardRequest();
            if (transactionId.HasValue || storedValueAccountId.HasValue)
            {
                var storedValueAccountHistories = _context.StoredValueAccountHistory.Where(t => (transactionId.HasValue ? t.TransactionId == transactionId.Value : true) && (storedValueAccountId.HasValue ? t.Id == storedValueAccountId.Value : true)).AsNoTracking().ToList();
                foreach (var storedValueAccountHistory in storedValueAccountHistories)
                {
                    var membershipCardAccount = _membershipCardAccountService.GetAllFromCache().Where(t => t.Id == storedValueAccountHistory.MembershipCardAccountId).FirstOrDefault();
                    if (membershipCardAccount != null)
                    {
                        if (membershipCardAccount.Type == MembershipCardAccountType.StoredValue && membershipCardAccount.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount)
                        {
                            divisionCardRequest.OverdraftAmount += storedValueAccountHistory.Amount;
                        }
                        else
                        {
                            divisionCardRequest.Amount += storedValueAccountHistory.Amount;
                        }
                    }
                    else
                    {
                        throw new Exception("账户不存在");
                    }

                    divisionCardRequest.TransactionDate = storedValueAccountHistory.TransactionDate;
                    divisionCardRequest.TransactionId = storedValueAccountHistory.TransactionId;
                    divisionCardRequest.LeaderCardId = storedValueAccountHistory.MembershipCardId;
                    divisionCardRequest.OperationType = storedValueAccountHistory.OperationType;
                    divisionCardRequest.RevenueType = storedValueAccountHistory.RevenueType;
                    divisionCardRequest.PaymentMode = storedValueAccountHistory.PaymentMode;
                    divisionCardRequest.PaymentCardNumber = storedValueAccountHistory.PaymentCardNumber;
                    divisionCardRequest.PlaceCode = storedValueAccountHistory.PlaceCode;
                    divisionCardRequest.CheckNumber = storedValueAccountHistory.CheckNumber;
                    divisionCardRequest.Description = storedValueAccountHistory.Description;
                    divisionCardRequest.IsManual = storedValueAccountHistory.IsManual;
                    divisionCardRequest.StoredValueHistoryId = storedValueAccountHistory.Id;
                    divisionCardRequest.CardTransactionId = storedValueAccountHistory.MembershipCardTransactionId;
                    divisionCardRequest.HotelCode = storedValueAccountHistory.HotelCode;
                    divisionCardRequest.SourceCode = storedValueAccountHistory.SourceCode;
                }

            }

            #endregion

            try
            {
                var divisionCardResult = _context.DivisionCard.Where(t => t.LeaderCardId == divisionCardRequest.LeaderCardId && t.IsValid).AsNoTracking().OrderBy(t => t.CardNumber);

                decimal allAmount = 0;
                decimal allOverdraftAmount = 0;
                foreach (var divisionCard in divisionCardResult)
                {
                    allAmount += divisionCard.Balance;
                    allOverdraftAmount += divisionCard.OverdraftBalance;
                }
                if (allAmount < divisionCardRequest.Amount || allOverdraftAmount < divisionCardRequest.OverdraftAmount)
                {
                    // 这块是为了在卡值不足以支付的时候让程序报错
                    int a = 0;
                    var i = 1 / a;
                }

                decimal deductionAmount = -divisionCardRequest.Amount; //要扣减的卡值，传入的是负值，所以转换为正值
                decimal deductionOverdraftAmount = -divisionCardRequest.OverdraftAmount;// 要扣减的透支，传入的是负值，所以转换为正值

                List<DivisionCardFeeHistory> divisionCardFeeHistories = new List<DivisionCardFeeHistory>();
                List<DivisionCard> divisionCards = new List<DivisionCard>();
                foreach (var divisionCard in divisionCardResult)
                {
                    if (deductionAmount <= 0 && deductionOverdraftAmount <= 0)
                    {
                        break;
                    }

                    decimal currentAmount = 0m; // 当前分裂卡要扣减的卡值
                    decimal currentOverdraftAmount = 0m; // 当前分裂卡要扣减的透支

                    if (deductionAmount > 0 && divisionCard.Balance > 0)
                    {
                        if (divisionCard.Balance >= deductionAmount)
                        {
                            divisionCard.Balance -= deductionAmount;
                            currentAmount = deductionAmount;
                            deductionAmount = 0;
                        }
                        else
                        {
                            currentAmount = divisionCard.Balance;
                            deductionAmount = deductionAmount - divisionCard.Balance;
                            divisionCard.Balance = 0;
                        }

                        DivisionCardFeeHistory divisionCardFeeHistory = new DivisionCardFeeHistory();
                        divisionCardFeeHistory.Id = _uniqueIdGeneratorService.Next();
                        divisionCardFeeHistory.CardId = divisionCard.Id;
                        divisionCardFeeHistory.CardNumber = divisionCard.CardNumber.ToString();
                        divisionCardFeeHistory.TransactionDate = divisionCardRequest.TransactionDate;
                        divisionCardFeeHistory.Amount = -currentAmount;
                        divisionCardFeeHistory.OperationType = divisionCardRequest.OperationType;
                        divisionCardFeeHistory.LastBalance = divisionCard.Balance + currentAmount;
                        divisionCardFeeHistory.ThisBalance = divisionCard.Balance;
                        divisionCardFeeHistory.LastOverdraftBalance = divisionCard.OverdraftBalance;
                        divisionCardFeeHistory.ThisOverdraftBalance = divisionCard.OverdraftBalance;
                        divisionCardFeeHistory.RevenueType = divisionCardRequest.RevenueType;
                        divisionCardFeeHistory.PaymentMode = divisionCardRequest.PaymentMode;
                        divisionCardFeeHistory.PaymentCardNumber = divisionCardRequest.PaymentCardNumber;
                        divisionCardFeeHistory.PlaceCode = divisionCardRequest.PlaceCode;
                        divisionCardFeeHistory.CheckNumber = divisionCardRequest.CheckNumber;
                        divisionCardFeeHistory.Description = divisionCardRequest.Description;
                        divisionCardFeeHistory.IsManual = divisionCardRequest.IsManual;
                        divisionCardFeeHistory.IsOverdraft = false;
                        divisionCardFeeHistory.TransactionId = divisionCardRequest.TransactionId;
                        divisionCardFeeHistory.LeaderStoredValueHistoryId = divisionCardRequest.StoredValueHistoryId;
                        divisionCardFeeHistory.HotelCode = divisionCardRequest.HotelCode;
                        divisionCardFeeHistory.SourceCode = divisionCardRequest.SourceCode;
                        divisionCardFeeHistory.LeaderMembershipCardId = divisionCardRequest.LeaderCardId;
                        divisionCardFeeHistory.InsertUser = "admin";
                        divisionCardFeeHistory.InsertDate = DateTime.Now;
                        divisionCardFeeHistory.UpdateUser = "admin";
                        divisionCardFeeHistory.UpdateDate = DateTime.Now;
                        divisionCardFeeHistories.Add(divisionCardFeeHistory);
                    }

                    if (deductionOverdraftAmount > 0 && divisionCard.OverdraftBalance > 0)
                    {
                        if (divisionCard.OverdraftBalance >= deductionOverdraftAmount)
                        {
                            divisionCard.OverdraftBalance -= deductionOverdraftAmount;
                            currentOverdraftAmount = deductionOverdraftAmount;
                            deductionOverdraftAmount = 0;
                        }
                        else
                        {
                            currentOverdraftAmount = divisionCard.OverdraftBalance;
                            deductionOverdraftAmount = deductionOverdraftAmount - divisionCard.OverdraftBalance;
                            divisionCard.OverdraftBalance = 0;
                        }

                        DivisionCardFeeHistory divisionCardFeeHistory = new DivisionCardFeeHistory();
                        divisionCardFeeHistory.Id = _uniqueIdGeneratorService.Next();
                        divisionCardFeeHistory.CardId = divisionCard.Id;
                        divisionCardFeeHistory.CardNumber = divisionCard.CardNumber.ToString();
                        divisionCardFeeHistory.TransactionDate = divisionCardRequest.TransactionDate;
                        divisionCardFeeHistory.Amount = -currentOverdraftAmount;
                        divisionCardFeeHistory.OperationType = divisionCardRequest.OperationType;
                        divisionCardFeeHistory.LastBalance = divisionCard.Balance;
                        divisionCardFeeHistory.ThisBalance = divisionCard.Balance;
                        divisionCardFeeHistory.LastOverdraftBalance = divisionCard.OverdraftBalance + currentOverdraftAmount;
                        divisionCardFeeHistory.ThisOverdraftBalance = divisionCard.OverdraftBalance;
                        divisionCardFeeHistory.RevenueType = divisionCardRequest.RevenueType;
                        divisionCardFeeHistory.PaymentMode = divisionCardRequest.PaymentMode;
                        divisionCardFeeHistory.PaymentCardNumber = divisionCardRequest.PaymentCardNumber;
                        divisionCardFeeHistory.PlaceCode = divisionCardRequest.PlaceCode;
                        divisionCardFeeHistory.CheckNumber = divisionCardRequest.CheckNumber;
                        divisionCardFeeHistory.Description = divisionCardRequest.Description;
                        divisionCardFeeHistory.IsManual = divisionCardRequest.IsManual;
                        divisionCardFeeHistory.IsOverdraft = true;
                        divisionCardFeeHistory.TransactionId = divisionCardRequest.TransactionId;
                        divisionCardFeeHistory.LeaderStoredValueHistoryId = divisionCardRequest.StoredValueHistoryId;
                        divisionCardFeeHistory.HotelCode = divisionCardRequest.HotelCode;
                        divisionCardFeeHistory.SourceCode = divisionCardRequest.SourceCode;
                        divisionCardFeeHistory.LeaderMembershipCardId = divisionCardRequest.LeaderCardId;
                        divisionCardFeeHistory.InsertUser = "admin";
                        divisionCardFeeHistory.InsertDate = DateTime.Now;
                        divisionCardFeeHistory.UpdateUser = "admin";
                        divisionCardFeeHistory.UpdateDate = DateTime.Now;
                        divisionCardFeeHistories.Add(divisionCardFeeHistory);
                    }
                    if (divisionCard.Balance <= 0 && divisionCard.OverdraftBalance <= 0)
                    {
                        divisionCard.IsValid = false;
                    }

                    divisionCards.Add(divisionCard);
                }

                _context.DivisionCardFeeHistory.AddRange(divisionCardFeeHistories);
                _context.DivisionCard.UpdateRange(divisionCards);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                FailCardTransaction failCardTransaction = new FailCardTransaction
                {
                    Id = _uniqueIdGeneratorService.Next(),
                    LeaderMembershipCardTransactionId = divisionCardRequest.CardTransactionId,
                    LeaderStoredValueHistoryId = divisionCardRequest.StoredValueHistoryId,
                    InsertUser = "admin",
                    InsertDate = DateTime.Now,
                    UpdateUser = "admin",
                    UpdateDate = DateTime.Now
                };
                _context.FailCardTransaction.Add(failCardTransaction);
                _context.SaveChanges();
                _logger.LogDebug(ex.StackTrace);
                throw new DataException("扣减卡值出错", ex);
            }
        }

        /// <summary>
        /// 有卡值增项时的逻辑
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="operationTypeMain"></param>
        public void AddCardFee(long? transactionId, long? storedValueAccountId = null)
        {
            _logger.LogDebug("进入卡值增项");
            #region 获取基本信息
            DivisionCardRequest divisionCardRequest = new DivisionCardRequest();
            if (transactionId.HasValue || storedValueAccountId.HasValue)
            {
                var storedValueAccountHistories = _context.StoredValueAccountHistory.Where(t => (transactionId.HasValue ? t.TransactionId == transactionId.Value : true) && (storedValueAccountId.HasValue ? t.Id == storedValueAccountId.Value : true)).AsNoTracking().ToList();

                int operationType = storedValueAccountHistories.FirstOrDefault().OperationType;
                if (storedValueAccountHistories.Count > 1 && storedValueAccountHistories.Any(t => t.OperationType == 11))
                {
                    operationType = storedValueAccountHistories.Where(t => t.OperationType != 11).FirstOrDefault().OperationType;
                }

                foreach (var storedValueAccountHistory in storedValueAccountHistories)
                {
                    var membershipCardAccount = _membershipCardAccountService.GetAllFromCache().Where(t => t.Id == storedValueAccountHistory.MembershipCardAccountId).FirstOrDefault();
                    if (membershipCardAccount != null)
                    {
                        if (membershipCardAccount.Type == MembershipCardAccountType.StoredValue && membershipCardAccount.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount)
                        {
                            divisionCardRequest.OverdraftAmount += storedValueAccountHistory.Amount;
                        }
                        else
                        {
                            divisionCardRequest.Amount += storedValueAccountHistory.Amount;
                        }
                    }
                    else
                    {
                        throw new Exception("账户不存在");
                    }

                    divisionCardRequest.TransactionDate = storedValueAccountHistory.TransactionDate;
                    divisionCardRequest.TransactionId = storedValueAccountHistory.TransactionId;
                    divisionCardRequest.LeaderCardId = storedValueAccountHistory.MembershipCardId;
                    divisionCardRequest.OperationType = operationType;
                    divisionCardRequest.RevenueType = storedValueAccountHistory.RevenueType;
                    divisionCardRequest.PaymentMode = storedValueAccountHistory.PaymentMode;
                    divisionCardRequest.PaymentCardNumber = storedValueAccountHistory.PaymentCardNumber;
                    divisionCardRequest.PlaceCode = storedValueAccountHistory.PlaceCode;
                    divisionCardRequest.CheckNumber = storedValueAccountHistory.CheckNumber;
                    divisionCardRequest.Description = storedValueAccountHistory.Description;
                    divisionCardRequest.IsManual = storedValueAccountHistory.IsManual;
                    divisionCardRequest.StoredValueHistoryId = storedValueAccountHistory.Id;
                    divisionCardRequest.CardTransactionId = storedValueAccountHistory.MembershipCardTransactionId;
                    divisionCardRequest.HotelCode = storedValueAccountHistory.HotelCode;
                    divisionCardRequest.SourceCode = storedValueAccountHistory.SourceCode;
                }

            }

            #endregion

            try
            {
                _logger.LogDebug("开始增项处理");
                #region 获取最后一张分裂卡的信息
                var lastDivisionCardResult = _context.DivisionCard.Where(t => t.LeaderCardId == divisionCardRequest.LeaderCardId && t.IsValid).AsNoTracking().OrderByDescending(t => t.CardNumber).FirstOrDefault();

                decimal lastOverdraftBalance = 0; // 最后一张分裂卡的透支余额
                decimal lastBalance = 0; // 最后一张分裂卡的余额
                long lastDivisionCardId = _uniqueIdGeneratorService.Next();
                string lastDivisionCardNumber = "0";
                if (lastDivisionCardResult != null)
                {
                    lastOverdraftBalance = lastDivisionCardResult.OverdraftBalance;
                    lastBalance = lastDivisionCardResult.Balance;
                    lastDivisionCardId = lastDivisionCardResult.Id;
                    lastDivisionCardNumber = Convert.ToString(lastDivisionCardResult.CardNumber);
                }
                #endregion

                decimal amount = divisionCardRequest.Amount;

                List<DivisionCardFeeHistory> divisionCardFeeHistories = new List<DivisionCardFeeHistory>();
                List<DivisionCard> divisionCards = new List<DivisionCard>();
                if (amount > 0)
                {
                    _logger.LogDebug("有实际增项");
                    // 转出透支额度
                    if (lastOverdraftBalance > 0)
                    {
                        DivisionCardFeeHistory divisionCardFeeHistory = new DivisionCardFeeHistory
                        {
                            Id = _uniqueIdGeneratorService.Next(),
                            CardId = lastDivisionCardId,
                            CardNumber = lastDivisionCardNumber,
                            TransactionDate = DateTime.Now,
                            Amount = -lastOverdraftBalance,
                            OperationType = 1999,
                            LastBalance = lastBalance,
                            ThisBalance = lastBalance,
                            LastOverdraftBalance = lastOverdraftBalance,
                            ThisOverdraftBalance = 0,
                            RevenueType = divisionCardRequest.RevenueType,
                            PaymentMode = divisionCardRequest.PaymentMode,
                            PaymentCardNumber = divisionCardRequest.PaymentCardNumber,
                            PlaceCode = divisionCardRequest.PlaceCode,
                            CheckNumber = divisionCardRequest.CheckNumber,
                            Description = divisionCardRequest.Description,
                            IsManual = divisionCardRequest.IsManual,
                            IsOverdraft = true,
                            TransactionId = divisionCardRequest.TransactionId,
                            LeaderStoredValueHistoryId = divisionCardRequest.StoredValueHistoryId,
                            HotelCode = divisionCardRequest.HotelCode,
                            SourceCode = divisionCardRequest.SourceCode,
                            LeaderMembershipCardId = divisionCardRequest.LeaderCardId,
                            InsertUser = "admin",
                            InsertDate = DateTime.Now,
                            UpdateUser = "admin",
                            UpdateDate = DateTime.Now
                        };

                        divisionCardFeeHistories.Add(divisionCardFeeHistory);
                    }

                    // 获取卡值上限信息
                    var cardResult = _context.MembershipCard.Where(t => t.Id == divisionCardRequest.LeaderCardId).AsNoTracking().FirstOrDefault();
                    var cardTypeResult = _membershipCardTypeService.GetAllFromCache().Where(t => t.Id == cardResult.MembershipCardTypeId).FirstOrDefault();

                    bool isLimitBalance = cardTypeResult.IsLimitBalance;

                    //获取证件号来判断是否实名
                    string idNumber = "";
                    idNumber = _context.Profile.Where(t => t.Id == cardResult.ProfileId && t.IdTypeCode == "0001").Select(t => t.IdNumber).FirstOrDefault();
                    if (string.IsNullOrEmpty(idNumber))
                    {
                        idNumber = _context.ProfileIdDetail.Where(t => t.ProfileId == cardResult.ProfileId && t.IdTypeCode == "0001").Select(t => t.IdNumber).FirstOrDefault();
                    }
                    
                    decimal limitBalance;
                    //实名卡值限制字段和卡值限制字段的值不相等，并且证件号等于18位则是实名会员，否则是非实名会员
                    if ((cardTypeResult.LimitBalance != cardTypeResult.RealNameLimitBalance) && (!string.IsNullOrEmpty(idNumber) && idNumber.Length == 18))
                    {
                        limitBalance = cardTypeResult.RealNameLimitBalance.Value;
                    }
                    else 
                    {
                        limitBalance = cardTypeResult.LimitBalance.HasValue ? cardTypeResult.LimitBalance.Value : 0;
                    }
                    decimal balance = 0m;

                    long divisionCardId = 0;

                    while (amount > 0)
                    {
                        if (isLimitBalance)
                        {
                            if (amount > limitBalance)
                            {
                                balance = limitBalance;
                                amount -= limitBalance;
                            }
                            else
                            {
                                balance = amount;
                                amount = 0;
                            }
                        }
                        else
                        {
                            balance = amount;
                            amount = 0;
                        }

                        divisionCardId = _uniqueIdGeneratorService.Next();

                        DivisionCard divisionCard = new DivisionCard();
                        divisionCard.Id = divisionCardId;
                        divisionCard.Balance = balance;
                        divisionCard.OverdraftBalance = 0;
                        if (amount == 0)
                        {
                            divisionCard.OverdraftBalance = divisionCardRequest.OverdraftAmount + lastOverdraftBalance;
                        }
                        divisionCard.IsValid = true;
                        divisionCard.LeaderCardId = divisionCardRequest.LeaderCardId;
                        divisionCard.InsertUser = "admin";
                        divisionCard.InsertDate = DateTime.Now;
                        divisionCard.UpdateUser = "admin";
                        divisionCard.UpdateDate = DateTime.Now;
                        divisionCards.Add(divisionCard);

                        DivisionCardFeeHistory divisionCardFeeHistory = new DivisionCardFeeHistory
                        {
                            Id = _uniqueIdGeneratorService.Next(),
                            CardId = divisionCardId,
                            CardNumber = "0",
                            TransactionDate = divisionCardRequest.TransactionDate,
                            Amount = balance,
                            OperationType = divisionCardRequest.OperationType,
                            LastBalance = 0,
                            ThisBalance = balance,
                            LastOverdraftBalance = 0,
                            ThisOverdraftBalance = 0,
                            RevenueType = divisionCardRequest.RevenueType,
                            PaymentMode = divisionCardRequest.PaymentMode,
                            PaymentCardNumber = divisionCardRequest.PaymentCardNumber,
                            PlaceCode = divisionCardRequest.PlaceCode,
                            CheckNumber = divisionCardRequest.CheckNumber,
                            Description = divisionCardRequest.Description,
                            IsManual = divisionCardRequest.IsManual,
                            IsOverdraft = false,
                            TransactionId = divisionCardRequest.TransactionId,
                            LeaderStoredValueHistoryId = divisionCardRequest.StoredValueHistoryId,
                            HotelCode = divisionCardRequest.HotelCode,
                            SourceCode = divisionCardRequest.SourceCode,
                            LeaderMembershipCardId = divisionCardRequest.LeaderCardId,
                            InsertUser = "admin",
                            InsertDate = DateTime.Now,
                            UpdateUser = "admin",
                            UpdateDate = DateTime.Now
                        };

                        divisionCardFeeHistories.Add(divisionCardFeeHistory);

                        if (amount == 0)
                        {
                            // 转入透支额度
                            if (lastOverdraftBalance > 0)
                            {
                                DivisionCardFeeHistory overdraftTransferToRow = new DivisionCardFeeHistory();
                                overdraftTransferToRow.Id = _uniqueIdGeneratorService.Next();
                                overdraftTransferToRow.CardId = divisionCardId;
                                overdraftTransferToRow.CardNumber = "0";
                                overdraftTransferToRow.TransactionDate = DateTime.Now;
                                overdraftTransferToRow.Amount = lastOverdraftBalance;
                                overdraftTransferToRow.OperationType = 1999;
                                overdraftTransferToRow.LastBalance = balance;
                                overdraftTransferToRow.ThisBalance = balance;
                                overdraftTransferToRow.LastOverdraftBalance = 0;
                                overdraftTransferToRow.ThisOverdraftBalance = lastOverdraftBalance;
                                overdraftTransferToRow.RevenueType = divisionCardRequest.RevenueType;
                                overdraftTransferToRow.PaymentMode = divisionCardRequest.PaymentMode;
                                overdraftTransferToRow.PaymentCardNumber = divisionCardRequest.PaymentCardNumber;
                                overdraftTransferToRow.PlaceCode = divisionCardRequest.PlaceCode;
                                overdraftTransferToRow.CheckNumber = divisionCardRequest.CheckNumber;
                                overdraftTransferToRow.Description = divisionCardRequest.Description;
                                overdraftTransferToRow.IsManual = divisionCardRequest.IsManual;
                                overdraftTransferToRow.IsOverdraft = true;
                                overdraftTransferToRow.TransactionId = divisionCardRequest.TransactionId;
                                overdraftTransferToRow.LeaderStoredValueHistoryId = divisionCardRequest.StoredValueHistoryId;
                                overdraftTransferToRow.HotelCode = divisionCardRequest.HotelCode;
                                overdraftTransferToRow.SourceCode = divisionCardRequest.SourceCode;
                                overdraftTransferToRow.LeaderMembershipCardId = divisionCardRequest.LeaderCardId;
                                overdraftTransferToRow.InsertUser = "admin";
                                overdraftTransferToRow.InsertDate = DateTime.Now;
                                overdraftTransferToRow.UpdateUser = "admin";
                                overdraftTransferToRow.UpdateDate = DateTime.Now;
                                divisionCardFeeHistories.Add(overdraftTransferToRow);
                            }
                            // 还款
                            if (divisionCardRequest.OverdraftAmount > 0)
                            {
                                DivisionCardFeeHistory rePay = new DivisionCardFeeHistory();
                                rePay.Id = _uniqueIdGeneratorService.Next();
                                rePay.CardId = divisionCardId;
                                rePay.CardNumber = "0";
                                rePay.TransactionDate = divisionCardRequest.TransactionDate;
                                rePay.Amount = divisionCardRequest.OverdraftAmount;
                                rePay.OperationType = 11;
                                rePay.LastBalance = balance;
                                rePay.ThisBalance = balance;
                                rePay.LastOverdraftBalance = lastOverdraftBalance;
                                rePay.ThisOverdraftBalance = lastOverdraftBalance + divisionCardRequest.OverdraftAmount;
                                rePay.RevenueType = divisionCardRequest.RevenueType;
                                rePay.PaymentMode = divisionCardRequest.PaymentMode;
                                rePay.PaymentCardNumber = divisionCardRequest.PaymentCardNumber;
                                rePay.PlaceCode = divisionCardRequest.PlaceCode;
                                rePay.CheckNumber = divisionCardRequest.CheckNumber;
                                rePay.Description = divisionCardRequest.Description;
                                rePay.IsManual = divisionCardRequest.IsManual;
                                rePay.IsOverdraft = true;
                                rePay.TransactionId = divisionCardRequest.TransactionId;
                                rePay.LeaderStoredValueHistoryId = divisionCardRequest.StoredValueHistoryId;
                                rePay.HotelCode = divisionCardRequest.HotelCode;
                                rePay.SourceCode = divisionCardRequest.SourceCode;
                                rePay.LeaderMembershipCardId = divisionCardRequest.LeaderCardId;
                                rePay.InsertUser = "admin";
                                rePay.InsertDate = DateTime.Now;
                                rePay.UpdateUser = "admin";
                                rePay.UpdateDate = DateTime.Now;
                                divisionCardFeeHistories.Add(rePay);
                            }
                        }
                    }

                    _context.DivisionCard.AddRange(divisionCards);
                    _context.DivisionCardFeeHistory.AddRange(divisionCardFeeHistories);
                    _context.SaveChanges();

                    var updateDivisionCardFeeHistoryResult = _context.DivisionCardFeeHistory.Where(t => t.LeaderMembershipCardId == divisionCardRequest.LeaderCardId && t.CardNumber == "0").ToList();
                    foreach (var cardFee in updateDivisionCardFeeHistoryResult)
                    {
                        var card = _context.DivisionCard.Where(t => t.Id == cardFee.CardId).FirstOrDefault();
                        cardFee.CardNumber = card.CardNumber.ToString();
                    }
                    // 更新分裂卡流水的卡号
                    _context.DivisionCardFeeHistory.UpdateRange(updateDivisionCardFeeHistoryResult);

                    // 更新上一个分裂卡的透支额度为0
                    if (lastDivisionCardResult != null)
                    {
                        lastDivisionCardResult.OverdraftBalance = 0;
                        lastDivisionCardResult.IsValid = lastBalance != 0;
                        _context.DivisionCard.Update(lastDivisionCardResult);
                    }

                    _context.SaveChanges();
                }
                else
                {
                    _logger.LogDebug("有其他增项");
                    DivisionCard divisionCard = null;
                    int operationType = 11; // 还款
                    if (lastDivisionCardResult == null)
                    {
                        operationType = 12; // 初始化透支额度
                        long divisionCardId = _uniqueIdGeneratorService.Next();
                        divisionCard = new DivisionCard();
                        divisionCard.Id = divisionCardId;
                        divisionCard.Balance = 0;
                        divisionCard.OverdraftBalance = 0;
                        divisionCard.IsValid = true;
                        divisionCard.LeaderCardId = divisionCardRequest.LeaderCardId;
                        divisionCard.InsertUser = "admin";
                        divisionCard.InsertDate = DateTime.Now;
                        divisionCard.UpdateUser = "admin";
                        divisionCard.UpdateDate = DateTime.Now;

                        lastDivisionCardId = divisionCardId;
                    }
                    var divisionCardFeeHistoryId = _uniqueIdGeneratorService.Next();
                    // 还款
                    DivisionCardFeeHistory divisionCardFeeHistory = new DivisionCardFeeHistory();
                    if (divisionCardRequest.OverdraftAmount > 0)
                    {
                        _logger.LogDebug("透支有值");
                        divisionCardFeeHistory.Id = divisionCardFeeHistoryId;
                        divisionCardFeeHistory.CardId = lastDivisionCardId;
                        divisionCardFeeHistory.CardNumber = lastDivisionCardNumber;
                        divisionCardFeeHistory.TransactionDate = divisionCardRequest.TransactionDate;
                        divisionCardFeeHistory.Amount = divisionCardRequest.OverdraftAmount;
                        divisionCardFeeHistory.OperationType = operationType;
                        divisionCardFeeHistory.LastBalance = lastBalance;
                        divisionCardFeeHistory.ThisBalance = lastBalance;
                        divisionCardFeeHistory.LastOverdraftBalance = lastOverdraftBalance;
                        divisionCardFeeHistory.ThisOverdraftBalance = lastOverdraftBalance + divisionCardRequest.OverdraftAmount;
                        divisionCardFeeHistory.RevenueType = divisionCardRequest.RevenueType;
                        divisionCardFeeHistory.PaymentMode = divisionCardRequest.PaymentMode;
                        divisionCardFeeHistory.PaymentCardNumber = divisionCardRequest.PaymentCardNumber;
                        divisionCardFeeHistory.PlaceCode = divisionCardRequest.PlaceCode;
                        divisionCardFeeHistory.CheckNumber = divisionCardRequest.CheckNumber;
                        divisionCardFeeHistory.Description = divisionCardRequest.Description;
                        divisionCardFeeHistory.IsManual = divisionCardRequest.IsManual;
                        divisionCardFeeHistory.IsOverdraft = true;
                        divisionCardFeeHistory.TransactionId = divisionCardRequest.TransactionId == null ? 0 : divisionCardRequest.TransactionId;
                        divisionCardFeeHistory.LeaderStoredValueHistoryId = divisionCardRequest.StoredValueHistoryId == null ? 0 : divisionCardRequest.StoredValueHistoryId;
                        divisionCardFeeHistory.HotelCode = divisionCardRequest.HotelCode;
                        divisionCardFeeHistory.SourceCode = divisionCardRequest.SourceCode;
                        divisionCardFeeHistory.LeaderMembershipCardId = divisionCardRequest.LeaderCardId;
                        divisionCardFeeHistory.InsertUser = "admin";
                        divisionCardFeeHistory.InsertDate = DateTime.Now;
                        divisionCardFeeHistory.UpdateUser = "admin";
                        divisionCardFeeHistory.UpdateDate = DateTime.Now;
                        divisionCardFeeHistories.Add(divisionCardFeeHistory);
                    }
                    if (divisionCard != null)
                    {
                        _context.DivisionCard.Add(divisionCard);
                    }

                    _context.DivisionCardFeeHistory.Add(divisionCardFeeHistory);
                    _context.SaveChanges();

                    // 更新分裂卡流水的卡号
                    var updateDivisionCardFeeHistoryResult = _context.DivisionCardFeeHistory.Where(t => t.Id == divisionCardFeeHistoryId && t.CardNumber == "0").FirstOrDefault();
                    if (updateDivisionCardFeeHistoryResult != null)
                    {
                        var card = _context.DivisionCard.Where(t => t.Id == updateDivisionCardFeeHistoryResult.CardId).FirstOrDefault();
                        updateDivisionCardFeeHistoryResult.CardNumber = card.CardNumber.ToString();
                        _context.DivisionCardFeeHistory.Update(updateDivisionCardFeeHistoryResult);
                    }

                    // 更新最后一个分裂卡的透支额度
                    var updateDivisionCardResult = _context.DivisionCard.Where(t => t.Id == lastDivisionCardId).FirstOrDefault();
                    updateDivisionCardResult.OverdraftBalance = lastOverdraftBalance + divisionCardRequest.OverdraftAmount;
                    _context.DivisionCard.Update(updateDivisionCardResult);

                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                FailCardTransaction failCardTransaction = new FailCardTransaction
                {
                    Id = _uniqueIdGeneratorService.Next(),
                    LeaderMembershipCardTransactionId = divisionCardRequest.CardTransactionId,
                    LeaderStoredValueHistoryId = divisionCardRequest.StoredValueHistoryId,
                    InsertUser = "admin",
                    InsertDate = DateTime.Now,
                    UpdateUser = "admin",
                    UpdateDate = DateTime.Now
                };
                _context.FailCardTransaction.Add(failCardTransaction);
                _context.SaveChanges();

                _logger.LogDebug(ex.StackTrace);

                throw new DataException("分裂卡执行错误", ex);
            }
        }

        /// <summary>
        /// 分裂卡
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="membershipCardId"></param>
        public void DivisionCard(long? transactionId, long? membershipCardId, List<long> cardIds = null)
        {
            _logger.LogDebug("进入分裂卡方法");
            // 查找所有需要分裂卡的卡
            var membershipCardTypeIds = _membershipCardTypeService.GetAllFromCache().Where(t => t.IsLimitBalance).Select(t => t.Id).ToList();

            // 只要有transactionId，代表是正常的卡值操作，所以只要看transactionId就行了。
            if (transactionId.HasValue && transactionId.Value > 0)
            {
                var storedValueAccountHistories = _context.StoredValueAccountHistory.Where(t => t.TransactionId == transactionId.Value && membershipCardTypeIds.Contains(t.MembershipCardTypeId)).AsNoTracking().ToList();
                if (storedValueAccountHistories.Count > 0)
                {
                    // 如果既有卡值增加，又有减少
                    if (storedValueAccountHistories.Any(t => t.Amount < 0) && storedValueAccountHistories.Any(t => t.Amount > 0))
                    {
                        foreach (var s in storedValueAccountHistories)
                        {
                            if (s.Amount > 0)
                            {
                                AddCardFee(transactionId, s.Id);
                            }
                            else if(s.Amount < 0)
                            {
                                DeductCardFee(transactionId, s.Id);
                            }
                        }
                    }
                    else
                    {
                        if (storedValueAccountHistories.Any(t => t.Amount < 0))
                        {
                            DeductCardFee(transactionId);
                        }
                        else if (storedValueAccountHistories.Any(t => t.Amount > 0))
                        {
                            AddCardFee(transactionId);
                        }
                    }
                }
            }
            // 如果没有transactionId，在目前情况下，基本只有初始化透支和卡值这两种情况，也就是只有注册、新发卡、购卡这些动作。所以这会用membershipcardId操作。
            else if (membershipCardId.HasValue)
            {
                
                _logger.LogDebug("进入会员卡注册分裂");
                var storedValueAccountHistories = _context.StoredValueAccountHistory.Where(t => t.MembershipCardId == membershipCardId.Value && membershipCardTypeIds.Contains(t.MembershipCardTypeId) && t.Amount != 0).AsNoTracking().ToList();
                if (storedValueAccountHistories.Count > 0)
                {
                    foreach (var s in storedValueAccountHistories)
                    {
                        AddCardFee(null, s.Id);
                    }
                }
            }
            // 如果没有transactionid和membershipCardId，但是cardIds有值，就代表为不记名发卡。
            // 为了方便代码逻辑，所以限定场景为：不记名发卡。
            // 那么对于不记名发卡，初始化时，用到的账户最多只有本金和透支两个。（有初始卡值则写入本金，有透支的话就写透支）
            else if (cardIds.Count > 0)
            {
                _logger.LogDebug("查询所有流水");
                var storedValueAccountHistories = _context.StoredValueAccountHistory.Where(t => cardIds.Contains(t.MembershipCardId) && membershipCardTypeIds.Contains(t.MembershipCardTypeId) && t.Amount != 0).AsNoTracking().ToList();
                
                if (storedValueAccountHistories.Count > 0)
                {
                    _logger.LogDebug("开始处理");
                    var membershipCardAccounts = _membershipCardAccountService.GetAllFromCache().Where(t => t.MembershipCardTypeId == storedValueAccountHistories.FirstOrDefault().MembershipCardTypeId && t.Type == MembershipCardAccountType.StoredValue).ToList();

                    var overdraftMembershipCardAccount = membershipCardAccounts.Where(t => t.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.OverdraftAccount).FirstOrDefault();
                    var principalMembershipCardAccount = membershipCardAccounts.Where(t => t.StoredValueAccountType == MembershipCardAccountStoredValueAccountType.PrincipalAccount).FirstOrDefault();

                    List<DivisionCard> divisionCards = new List<DivisionCard>();
                    // 先生成本金的分裂卡
                    if (principalMembershipCardAccount != null)
                    {
                        _logger.LogDebug("生成本金的分裂卡");
                        var principalStoredValueAccountHistories = storedValueAccountHistories.Where(t => t.MembershipCardAccountId == principalMembershipCardAccount.Id).ToList();

                        if (principalStoredValueAccountHistories.Count > 0)
                        {
                            divisionCards = GetAnonymousCardPrincipalInfo(principalStoredValueAccountHistories);
                        }
                    }

                    // 再生成透支的分裂卡
                    if (overdraftMembershipCardAccount != null)
                    {
                        _logger.LogDebug("生成透支的分裂卡");
                        var overdraftStoredValueAccountHistories = storedValueAccountHistories.Where(t => t.MembershipCardAccountId == overdraftMembershipCardAccount.Id).ToList();

                        if (overdraftStoredValueAccountHistories.Count > 0)
                        {
                            GetAnonymousCardOverdraftInfo(overdraftStoredValueAccountHistories,divisionCards);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 合并会员卡功能的分裂卡
        /// </summary>
        /// <param name="masterCardId"></param>
        public void MergeDivisionCard(List<long> transactionIds)
        {
            _logger.LogDebug("进入分裂卡方法");
            // 查找所有需要分裂卡的卡
            var membershipCardTypeIds = _membershipCardTypeService.GetAllFromCache().Where(t => t.IsLimitBalance).Select(t => t.Id).ToList();

            if (transactionIds.Count>0)
            {
                _logger.LogDebug("获取分裂卡的流水");
                var storedValueAccountHistories = _context.StoredValueAccountHistory.Where(t => transactionIds.Contains(t.TransactionId.Value) && membershipCardTypeIds.Contains(t.MembershipCardTypeId) && t.Amount != 0 ).AsNoTracking().ToList();
                if (storedValueAccountHistories.Count > 0)
                {
                    foreach (var s in storedValueAccountHistories)
                    {
                        if (s.Amount > 0)
                        {
                            AddCardFee(null, s.Id);
                        }
                        else if (s.Amount < 0)
                        {
                            DeductCardFee(null, s.Id);
                        }
                    }


                }
            }
        }

        public void GetAnonymousCardOverdraftInfo(List<StoredValueAccountHistory> storedValueAccountHistories, List<DivisionCard> divisionCards)
        {

            List<DivisionCardFeeHistory> divisionCardFeeHistories = new List<DivisionCardFeeHistory>();
            List<DivisionCard> newDivisionCards = new List<DivisionCard>();
            List<DivisionCard> lastDivisionCards = new List<DivisionCard>();
            _logger.LogDebug("开始循环透支流水");
            foreach (var storedValueAccountHistory in storedValueAccountHistories)
            {
                #region 获取最后一张分裂卡的信息
                var lastDivisionCardResult = divisionCards.Where(t => t.LeaderCardId == storedValueAccountHistory.MembershipCardId && t.IsValid).OrderByDescending(t => t.CardNumber).FirstOrDefault();

                decimal lastOverdraftBalance = 0; // 最后一张分裂卡的透支余额
                decimal lastBalance = 0; // 最后一张分裂卡的余额
                long lastDivisionCardId = _uniqueIdGeneratorService.Next();
                string lastDivisionCardNumber = "0";
                if (lastDivisionCardResult != null)
                {
                    lastOverdraftBalance = lastDivisionCardResult.OverdraftBalance;
                    lastBalance = lastDivisionCardResult.Balance;
                    lastDivisionCardId = lastDivisionCardResult.Id;
                    lastDivisionCardNumber = Convert.ToString(lastDivisionCardResult.CardNumber);
                    lastDivisionCards.Add(lastDivisionCardResult);
                }
                #endregion



                int operationType = 12; // 初始化透支额度
                if (lastDivisionCardResult == null)
                {
                    long divisionCardId = _uniqueIdGeneratorService.Next();
                    DivisionCard divisionCard = new DivisionCard();
                    divisionCard.Id = divisionCardId;
                    divisionCard.Balance = 0;
                    divisionCard.OverdraftBalance = 0;
                    divisionCard.IsValid = true;
                    divisionCard.LeaderCardId = storedValueAccountHistory.MembershipCardId;
                    divisionCard.InsertUser = "admin";
                    divisionCard.InsertDate = DateTime.Now;
                    divisionCard.UpdateUser = "admin";
                    divisionCard.UpdateDate = DateTime.Now;
                    newDivisionCards.Add(divisionCard);
                    lastDivisionCardId = divisionCardId;
                }
                var divisionCardFeeHistoryId = _uniqueIdGeneratorService.Next();
                // 还款
                DivisionCardFeeHistory divisionCardFeeHistory = new DivisionCardFeeHistory();
                if (storedValueAccountHistory.Amount > 0)
                {

                    divisionCardFeeHistory.Id = divisionCardFeeHistoryId;
                    divisionCardFeeHistory.CardId = lastDivisionCardId;
                    divisionCardFeeHistory.CardNumber = lastDivisionCardNumber;
                    divisionCardFeeHistory.TransactionDate = storedValueAccountHistory.TransactionDate;
                    divisionCardFeeHistory.Amount = storedValueAccountHistory.Amount;
                    divisionCardFeeHistory.OperationType = operationType;
                    divisionCardFeeHistory.LastBalance = lastBalance;
                    divisionCardFeeHistory.ThisBalance = lastBalance;
                    divisionCardFeeHistory.LastOverdraftBalance = lastOverdraftBalance;
                    divisionCardFeeHistory.ThisOverdraftBalance = lastOverdraftBalance + storedValueAccountHistory.Amount;
                    divisionCardFeeHistory.RevenueType = storedValueAccountHistory.RevenueType;
                    divisionCardFeeHistory.PaymentMode = storedValueAccountHistory.PaymentMode;
                    divisionCardFeeHistory.PaymentCardNumber = storedValueAccountHistory.PaymentCardNumber;
                    divisionCardFeeHistory.PlaceCode = storedValueAccountHistory.PlaceCode;
                    divisionCardFeeHistory.CheckNumber = storedValueAccountHistory.CheckNumber;
                    divisionCardFeeHistory.Description = storedValueAccountHistory.Description;
                    divisionCardFeeHistory.IsManual = storedValueAccountHistory.IsManual;
                    divisionCardFeeHistory.IsOverdraft = true;
                    divisionCardFeeHistory.TransactionId = storedValueAccountHistory.TransactionId;
                    divisionCardFeeHistory.LeaderStoredValueHistoryId = storedValueAccountHistory.Id;
                    divisionCardFeeHistory.HotelCode = storedValueAccountHistory.HotelCode;
                    divisionCardFeeHistory.SourceCode = storedValueAccountHistory.SourceCode;
                    divisionCardFeeHistory.LeaderMembershipCardId = storedValueAccountHistory.MembershipCardId;
                    divisionCardFeeHistory.InsertUser = "admin";
                    divisionCardFeeHistory.InsertDate = DateTime.Now;
                    divisionCardFeeHistory.UpdateUser = "admin";
                    divisionCardFeeHistory.UpdateDate = DateTime.Now;
                    divisionCardFeeHistories.Add(divisionCardFeeHistory);
                }

            }
            _logger.LogDebug("开始写入透支分裂卡");
            if (newDivisionCards.Count > 0)
            {
                _context.DivisionCard.AddRange(newDivisionCards);
            }
            _context.DivisionCardFeeHistory.AddRange(divisionCardFeeHistories);
            _context.SaveChanges();
            _logger.LogDebug("开始循环获取流水卡号");
            foreach (var cardFee in divisionCardFeeHistories)
            {
                if (newDivisionCards.Count > 0)
                {
                    var card = newDivisionCards.Where(t => t.Id == cardFee.CardId).FirstOrDefault();
                    cardFee.CardNumber = card.CardNumber.ToString();

                    card.OverdraftBalance = cardFee.Amount;
                }
                if (lastDivisionCards.Count > 0)
                {
                    var card = lastDivisionCards.Where(t => t.Id == cardFee.CardId).FirstOrDefault();
                    cardFee.CardNumber = card.CardNumber.ToString();

                    card.OverdraftBalance = cardFee.Amount;
                }
            }
            _logger.LogDebug("更新流水的卡号");
            // 更新流水的卡号
            _context.DivisionCardFeeHistory.UpdateRange(divisionCardFeeHistories);
            if (newDivisionCards.Count > 0)
            {
                _context.DivisionCard.UpdateRange(newDivisionCards);
            }
            if (lastDivisionCards.Count > 0)
            {
                _context.DivisionCard.UpdateRange(lastDivisionCards);
            }
            _context.SaveChanges();
            _logger.LogDebug("透支完成");
        }
        
        public List<DivisionCard> GetAnonymousCardPrincipalInfo(List<StoredValueAccountHistory> storedValueAccountHistories)
        {
            List<DivisionCardFeeHistory> divisionCardFeeHistories = new List<DivisionCardFeeHistory>();
            List<DivisionCard> divisionCards = new List<DivisionCard>();
            _logger.LogDebug("开始循环本金流水");
            foreach (var storedValueAccountHistory in storedValueAccountHistories)
            {
                decimal amount = storedValueAccountHistory.Amount;
                if (amount > 0)
                {
                    // 获取卡值上限信息
                    var cardTypeResult = _membershipCardTypeService.GetAllFromCache().Where(t => t.Id == storedValueAccountHistory.MembershipCardTypeId).FirstOrDefault();
                    bool isLimitBalance = cardTypeResult.IsLimitBalance;
                    decimal limitBalance = cardTypeResult.LimitBalance.HasValue ? cardTypeResult.LimitBalance.Value : 0;
                    decimal balance = 0m;

                    long divisionCardId = 0;

                    while (amount > 0)
                    {
                        if (isLimitBalance)
                        {
                            if (amount > limitBalance)
                            {
                                balance = limitBalance;
                                amount -= limitBalance;
                            }
                            else
                            {
                                balance = amount;
                                amount = 0;
                            }
                        }
                        else
                        {
                            balance = amount;
                            amount = 0;
                        }

                        divisionCardId = _uniqueIdGeneratorService.Next();

                        DivisionCard divisionCard = new DivisionCard();
                        divisionCard.Id = divisionCardId;
                        divisionCard.Balance = balance;
                        divisionCard.OverdraftBalance = 0;
                        divisionCard.IsValid = true;
                        divisionCard.LeaderCardId = storedValueAccountHistory.MembershipCardId;
                        divisionCard.InsertUser = "admin";
                        divisionCard.InsertDate = DateTime.Now;
                        divisionCard.UpdateUser = "admin";
                        divisionCard.UpdateDate = DateTime.Now;
                        divisionCards.Add(divisionCard);

                        DivisionCardFeeHistory divisionCardFeeHistory = new DivisionCardFeeHistory
                        {
                            Id = _uniqueIdGeneratorService.Next(),
                            CardId = divisionCardId,
                            CardNumber = "0",
                            TransactionDate = storedValueAccountHistory.TransactionDate,
                            Amount = balance,
                            OperationType = storedValueAccountHistory.OperationType,
                            LastBalance = 0,
                            ThisBalance = balance,
                            LastOverdraftBalance = 0,
                            ThisOverdraftBalance = 0,
                            RevenueType = storedValueAccountHistory.RevenueType,
                            PaymentMode = storedValueAccountHistory.PaymentMode,
                            PaymentCardNumber = storedValueAccountHistory.PaymentCardNumber,
                            PlaceCode = storedValueAccountHistory.PlaceCode,
                            CheckNumber = storedValueAccountHistory.CheckNumber,
                            Description = storedValueAccountHistory.Description,
                            IsManual = storedValueAccountHistory.IsManual,
                            IsOverdraft = false,
                            TransactionId = storedValueAccountHistory.TransactionId,
                            LeaderStoredValueHistoryId = storedValueAccountHistory.Id,
                            HotelCode = storedValueAccountHistory.HotelCode,
                            SourceCode = storedValueAccountHistory.SourceCode,
                            LeaderMembershipCardId = storedValueAccountHistory.MembershipCardId,
                            InsertUser = "admin",
                            InsertDate = DateTime.Now,
                            UpdateUser = "admin",
                            UpdateDate = DateTime.Now
                        };

                        divisionCardFeeHistories.Add(divisionCardFeeHistory);
                    }
                }
            }
            _logger.LogDebug("开始写入本金");
            _context.DivisionCard.AddRange(divisionCards);
            _context.DivisionCardFeeHistory.AddRange(divisionCardFeeHistories);
            _context.SaveChanges();
            _logger.LogDebug("开始循环获取流水卡号");
            foreach (var cardFee in divisionCardFeeHistories)
            {
                var card = divisionCards.Where(t => t.Id == cardFee.CardId).FirstOrDefault();
                cardFee.CardNumber = card.CardNumber.ToString();
            }
            _logger.LogDebug("开始更新流水卡号");
            // 更新流水的卡号
            _context.DivisionCardFeeHistory.UpdateRange(divisionCardFeeHistories);
            _context.SaveChanges();
            _logger.LogDebug("本金完成");
            return divisionCards;
        }
    }
}
