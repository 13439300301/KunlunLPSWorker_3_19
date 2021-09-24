using EFCore.BulkExtensions;
using Kunlun.LPS.Worker.Core.Consts;
using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.SendInfoServices;
using Kunlun.LPS.Worker.Services.StoredValue;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kunlun.LPS.Worker.Services.Coupons
{
    public class CouponService : ICouponService
    {
        private readonly ILogger<CouponService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly ICouponChangeReminderService _couponChangeReminderService;
        private readonly IGetOrUpdateInfoFromRedisService _getOrUpdateInfoFromRedisService;
        private readonly IMessageQueueProducer _messageQueueProducer;

        Random rd;
        public CouponService(
            ILogger<CouponService> logger,
            LPSWorkerContext context,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            ICouponChangeReminderService couponChangeReminderService,
            IGetOrUpdateInfoFromRedisService getOrUpdateInfoFromRedisService,
            IMessageQueueProducer messageQueueProducer
             )
        {
            _logger = logger;
            _context = context;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _couponChangeReminderService = couponChangeReminderService;
            _getOrUpdateInfoFromRedisService = getOrUpdateInfoFromRedisService;
            _messageQueueProducer = messageQueueProducer;
            rd = new Random();
        }

        public void GiftCoupons(RegisterCouponsMessage message, Dictionary<long, int> exchangeCoupons, long? couponChannel, string placeCode, DateTime date)
        {
            try
            {
                var batchId = Guid.NewGuid();
                var userCode = message.UserCode;
                var profileId = message.ProfileId;
                var hotelCode = message.HotelCode;
                if (string.IsNullOrEmpty(userCode))
                {
                    userCode = "Worker";
                }

                string mark;

                if (!string.IsNullOrEmpty(placeCode) && placeCode.Contains("-room"))
                {
                    placeCode = placeCode.Remove(placeCode.Length - 5, 5);
                    mark = "客房消费赠送券";
                }
                else if (!string.IsNullOrEmpty(placeCode) && placeCode.Contains("-custom"))
                {
                    placeCode = placeCode.Remove(placeCode.Length - 7, 7);
                    mark = "自定义日期赠送券";
                }
                else
                {
                    mark = "Register Gift Coupons";
                }

                var couponTypeIdList = exchangeCoupons.Select(c => c.Key);

                var transactionId = _uniqueIdGeneratorService.Next();
                var couponTransactionHistoryList = new List<CouponTransactionHistory>();
                var insertCouponsList = new List<Coupon>();
                var couponInventoryDic = new Dictionary<CouponType, int>();

                var transaction = new Transaction()
                {
                    Id = transactionId,
                    Points = 0,
                    ProfileId = profileId,
                    TransactionDate = date,
                    TransactionType = TransactionType.GiftCoupons,
                    TransactionNumber = Guid.NewGuid().ToString(),
                    HotelCode = hotelCode,
                    PlaceCode = placeCode,
                    Description = mark,
                    InsertUser = userCode,
                    InsertDate = date,
                    UpdateUser = userCode,
                    UpdateDate = date,
                };
                var couponTypes = _context.CouponType.AsNoTracking().ToList();
                foreach (var exchangeCoupon in exchangeCoupons)
                {
                    var couponTypeId = exchangeCoupon.Key;
                    var count = exchangeCoupon.Value;
                    var couponType = couponTypes.FirstOrDefault(ct => ct.Id == couponTypeId);
                    if (couponType.NeedManageInventory ?? false)
                    {
                        var inventory = _getOrUpdateInfoFromRedisService.GetCouponInventory(couponTypeId);
                        if (inventory != null)
                        {
                            if (count > inventory)
                            {
                                UserNotification userNotification = new UserNotification()
                                {
                                    Id = _uniqueIdGeneratorService.Next(),
                                    Content = couponType.Name + "库存不足",
                                    InsertDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                    InsertUser = userCode,
                                    UpdateUser = userCode,
                                    Type = NotificationType.Error,
                                    UserCode = userCode
                                };
                                _context.UserNotification.Add(userNotification);
                                _context.SaveChanges();
                                return;
                            }
                        }
                    }
                    switch (couponType.Category)
                    {
                        case CouponCategory.Discount:
                        case CouponCategory.Item:
                        case CouponCategory.Free:
                        case CouponCategory.Cash:
                        case CouponCategory.Gift:
                            
                            var coupons = GetCoupon(couponType.Id, couponType.Category, count, couponType.Prefix, userCode);

                            foreach (var coupon in coupons)
                            {
                                coupon.BindingDate = date;
                                coupon.OwnerId = profileId;
                                coupon.CouponChannelId = couponChannel;
                                coupon.Points = couponType.ExchangeNeedPoints;
                                coupon.FaceValue = couponType.FaceValue;
                                coupon.DiscountRate = couponType.DiscountRate;
                                if (couponType.TimeLimitMode == CouponTimeLimitMode.Date)
                                {
                                    coupon.BeginDate = couponType.TimeLimitBeginDate;
                                    coupon.EndDate = couponType.TimeLimitEndDate;
                                }
                                else
                                {
                                    coupon.BeginDate = date;
                                    coupon.EndDate = date.Date.AddDays(couponType.TimeLimitDays.Value + 1).AddSeconds(-1);
                                }
                                coupon.ExchangeMode = CouponExchangeMode.Gift;
                                coupon.UpdateDate = date;
                                coupon.UpdateUser = userCode;
                                insertCouponsList.Add(coupon);

                                var couponTransactionHistoryId = _uniqueIdGeneratorService.Next();
                                var couponTransactionHistory = new CouponTransactionHistory()
                                {
                                    Id = couponTransactionHistoryId,
                                    BatchId = batchId,
                                    CouponId = coupon.Id,
                                    Description = couponType.Name,
                                    HistoryId = null,
                                    Points = 0.00M,
                                    TransactionDate = date,
                                    OperationType = CouponTransactionHistoryOperationType.Gift,
                                    TransactionId = transactionId,
                                    ProfileId = profileId,
                                    CouponTypeId = coupon.CouponTypeId,
                                    FaceValue = couponType.FaceValue,
                                    PlaceCode = placeCode,
                                    HotelCode = hotelCode,
                                    InsertUser = userCode,
                                    InsertDate = date,
                                    UpdateUser = userCode,
                                    UpdateDate = date,
                                };

                                couponTransactionHistoryList.Add(couponTransactionHistory);
                            }
                            couponInventoryDic.Add(couponType, -count);
                            break;
                        case CouponCategory.GeneralCoupon:
                            var rd = new Random();
                            for (int i = 0; i < count; i++)
                            {
                                var coupon = new Coupon();
                                coupon.Id = _uniqueIdGeneratorService.Next();
                                coupon.CouponTypeId = couponTypeId;
                                coupon.CouponCategory = couponType.Category;
                                var number = GetCouponNumber(rd);
                                coupon.Number = couponType.Prefix + number;
                                if (couponType.TimeLimitMode == CouponTimeLimitMode.Date)
                                {
                                    coupon.BeginDate = couponType.TimeLimitBeginDate;
                                    coupon.EndDate = couponType.TimeLimitEndDate;
                                }
                                else
                                {
                                    coupon.BeginDate = date;
                                    coupon.EndDate = date.Date.AddDays(couponType.TimeLimitDays.Value + 1).AddSeconds(-1);
                                }
                                coupon.BindingDate = date;
                                coupon.OwnerId = profileId;
                                coupon.IsUsed = false;
                                coupon.IsExpired = false;
                                coupon.CouponChannelId = couponChannel;
                                coupon.Points = couponType.ExchangeNeedPoints;
                                coupon.ExchangeMode = couponType.ExchangeMode;
                                coupon.Version = new byte[] { 1, 0, 0, 0 };
                                coupon.UpdateDate = date;
                                coupon.UpdateUser = userCode;
                                insertCouponsList.Add(coupon);

                                var couponTransactionHistory = new CouponTransactionHistory()
                                {
                                    Id = _uniqueIdGeneratorService.Next(),
                                    BatchId = batchId,
                                    CouponId = coupon.Id,
                                    Description = couponType.Name,
                                    HistoryId = null,
                                    Points = 0.00M,
                                    TransactionDate = date,
                                    OperationType = CouponTransactionHistoryOperationType.Gift,
                                    TransactionId = transactionId,
                                    ProfileId = profileId,
                                    CouponTypeId = coupon.CouponTypeId,
                                    FaceValue = couponType.FaceValue,
                                    InsertUser = userCode,
                                    InsertDate = date,
                                    UpdateUser = userCode,
                                    UpdateDate = date,
                                };
                                couponTransactionHistoryList.Add(couponTransactionHistory);
                            }
                            break;
                    }
                }

                var transactionList = new List<Transaction>();
                transactionList.Add(transaction);
                using (var transaction1 = _context.Database.BeginTransaction())
                {
                    _context.BulkInsert(transactionList);
                    _context.BulkInsert(insertCouponsList);
                    _context.BulkInsert(couponTransactionHistoryList);
                    transaction1.Commit();
                }
                foreach (var item in couponInventoryDic)
                {
                    CouponUpdateInventoryMessage couponUpdateInventoryMessage = new CouponUpdateInventoryMessage()
                    {
                        CouponTypeId = item.Key.Id,
                        Inventory = item.Value,
                        NeedManageInventory = item.Key.NeedManageInventory.Value,
                        IsDeductInventory = false
                    };
                    _messageQueueProducer.PublishInternal(couponUpdateInventoryMessage);
                    _getOrUpdateInfoFromRedisService.UpdateRedisCouponInventory(item.Key.Id, item.Value);

                }
                _couponChangeReminderService.SendInfo(message, couponTransactionHistoryList, date, "GIFTCOUPONS");
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException)
                {
                    var sqlEx = ex.InnerException as SqlException;
                    if (sqlEx.Number == ConfigurationKey.FIELDREPEAT || sqlEx.Number == ConfigurationKey.KEYREPEAT)
                    {
                        throw ex;
                    }
                }

                _logger.LogError(ex, ex.Message);
            }
        }
        public List<Coupon> GetCoupon(long id, CouponCategory type, int totalCount, string prefix, string userCode)
        {
            var list = new List<Coupon>();

            for (var i = 0; i < totalCount; i++)
            {
                var number = GetCouponNumber(rd);
                var coupon = new Coupon()
                {
                    Id = _uniqueIdGeneratorService.Next(),
                    CouponCategory = type,
                    CouponTypeId = id,
                    Number = prefix + number,
                    Version = new byte[] { 1, 0, 0, 0 },
                    InsertUser = userCode,
                    InsertDate = DateTime.Now,
                    UpdateUser = userCode,
                    UpdateDate = DateTime.Now
                };
                list.Add(coupon);
            }


            return list;
        }
        public void UpdateCouponInventory(CouponUpdateInventoryMessage message)
        {
            try
            {
                var couponType = _context.CouponType.FirstOrDefault(m => m.Id == message.CouponTypeId);
                var couponTypeInventory = _context.CouponInventory.FirstOrDefault(m => m.CouponTypeId == message.CouponTypeId);

                if (message.NeedManageInventory)
                {
                    couponTypeInventory.Inventory += message.Inventory;
                    if (message.IsDeductInventory)
                    {
                        couponType.TotalCount += message.Inventory;
                    }
                    couponType.NeedManageInventory = true;
                }
                else
                {
                    couponTypeInventory.Inventory = 0;
                    if (message.IsDeductInventory)
                    {
                        couponType.TotalCount = 0;
                    }
                    couponType.InventoryAlertLine = 0;
                    couponType.NeedManageInventory = false;
                }



                _context.CouponType.Update(couponType);
                _context.CouponInventory.Update(couponTypeInventory);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public void BatchGiftCoupons(BatchGiftCouponMessage message)
        {
            var sw = Stopwatch.StartNew();
            //var swa = Stopwatch.StartNew();
            sw.Start();
            bool ISPerform = true;
            //swa.Start();
            var userNotification = new UserNotification
            {
                Id = _uniqueIdGeneratorService.Next(),
                Content = $"Import Coupon success",
                UserCode = message.UserCode,
                AlreadyRead = false,
                InsertUser = message.UserCode,
                UpdateUser = message.UserCode
            };
            try
            {
                var profileIdList = message.ProfileIdList;
                var exchangeCoupons = message.ExchangeCoupons;
                var exchangeCouponPackages = message.ExchangeCouponPackages;

                if (exchangeCouponPackages.Any())
                {
                    var couponPackage = _context.CouponPackageTypeDetail.AsNoTracking()
                        .Where(o => exchangeCouponPackages.Select(e => e.CouponPackageTypeId).Contains(o.CouponPackageTypeId));
                    foreach (var item in exchangeCouponPackages)
                    {
                        var couponPackageDetails = couponPackage.Select(m => new ExchangeCouponTypeModel { CouponTypeId = m.CouponTypeId, Count = item.Quantity * m.Quantity }).ToList();

                        exchangeCoupons.AddRange(couponPackageDetails);
                    }
                }
                //_logger.LogInformation("exchangeCouponPackages：" + swa.ElapsedMilliseconds);
                //swa.Restart();

                var couponTypeIdList = exchangeCoupons.Select(ct => ct.CouponTypeId).Distinct().ToList();
                var couponTypeInfo = GetCouponTypeInfo(couponTypeIdList);

                //_logger.LogInformation("couponTypeInfo：" + swa.ElapsedMilliseconds);
                //swa.Restart();
                if (ISPerform)
                {
                    if (profileIdList == null)
                    {
                        var skipNumber = 0;
                        while (true)
                        {
                            var list = _context.UserMetadata.AsNoTracking().Where(o => o.Key == UserMetadataKey.AdvancedSearchTemporaryStorage && o.UserCode == message.UserCode)
                                .Skip(skipNumber)
                                .Take(1000)
                                .Select(t => t.Value)
                                .Distinct()
                                .ToList();
                            if (!list.Any())
                            {
                                break;
                            }
                            skipNumber += list.Count;
                            var profileIdCouponTypeDic = new List<ProfileIdCouponTypeId>();
                            foreach (var profileId in list)
                            {
                                long.TryParse(profileId, out var a);
                                var detailList = exchangeCoupons.Select(o => o).ToList();
                                profileIdCouponTypeDic.Add(new ProfileIdCouponTypeId
                                {
                                    ProfileId = a,
                                    CouponTypeDetail = detailList
                                });
                            }
                            userNotification = CouponTypeInsert(profileIdCouponTypeDic, message, userNotification, couponTypeInfo);
                        }
                    }
                    else
                    {
                        var profileIdCouponTypeDic = new List<ProfileIdCouponTypeId>();
                        foreach (var profileId in profileIdList)
                        {
                            var detailList = exchangeCoupons.Select(o => o).ToList();
                            profileIdCouponTypeDic.Add(new ProfileIdCouponTypeId
                            {
                                ProfileId = profileId,
                                CouponTypeDetail = detailList
                            });
                        }
                        //_logger.LogInformation("profileIdCouponTypeDic：" + swa.ElapsedMilliseconds);
                        //swa.Restart();
                        userNotification = CouponTypeInsert(profileIdCouponTypeDic, message, userNotification, couponTypeInfo);
                        //_logger.LogInformation("CouponTypeInsert：" + swa.ElapsedMilliseconds);
                        //swa.Restart();
                    }
                    #region 

                    //var transactionList = new List<Transaction>();
                    //var batchId = Guid.NewGuid();
                    //var couponList = new List<Coupon>();
                    //var couponInventoryList = new List<CouponInventory>();
                    //var couponTransactionHistoryList = new List<CouponTransactionHistory>();
                    ////先更新库存
                    //foreach (var giftCoupon in exchangeCoupons)
                    //{
                    //    var couponTypeId = giftCoupon.CouponTypeId;
                    //    var couponType = couponTypes.First(m => m.Id == giftCoupon.CouponTypeId);
                    //    if (couponType.NeedManageInventory)
                    //    {
                    //        var count = giftCoupon.Count * profileIdList.Count;
                    //        var inventory = _context.CouponInventory
                    //            .Where(ci => ci.CouponType.Category != CouponCategory.GeneralCoupon)
                    //            .FirstOrDefault(ci => ci.CouponTypeId == giftCoupon.CouponTypeId);
                    //        if (inventory == null)
                    //        {
                    //            continue;
                    //        }
                    //        inventory.Inventory -= count;
                    //        _getOrUpdateInfoFromRedisService.UpdateRedisCouponInventory(couponTypeId, -count);
                    //        couponInventoryList.Add(inventory);
                    //    }
                    //}

                    //Dictionary<long, long> profileIdAndTransactionId = new Dictionary<long, long>();

                    //foreach (var profileId in profileIdList)
                    //{
                    //    var transactionId = _uniqueIdGeneratorService.Next();
                    //    var transaction = new Transaction()
                    //    {
                    //        Id = transactionId,
                    //        Points = 0,
                    //        ProfileId = profileId,
                    //        TransactionDate = DateTime.Now,
                    //        TransactionType = TransactionType.GiftCoupons,
                    //        TransactionNumber = Guid.NewGuid().ToString(),
                    //        HotelCode = hotelCode,
                    //        PlaceCode = message.PlaceCode,
                    //        Description = "LPS批量赠送券"
                    //    };
                    //    transactionList.Add(transaction);
                    //    profileIdAndTransactionId.Add(profileId, transactionId);
                    //}

                    //foreach (var giftCoupon in exchangeCoupons)
                    //{
                    //    var couponTypeId = giftCoupon.CouponTypeId;
                    //    var count = giftCoupon.Count;
                    //    var couponType = couponTypes.First(ct => ct.Id == couponTypeId);
                    //    switch (couponType.Category)
                    //    {
                    //        case CouponCategory.Cash:
                    //        case CouponCategory.Free:
                    //        case CouponCategory.Gift:
                    //        case CouponCategory.Item:
                    //        case CouponCategory.Discount:
                    //            var coupons = GetCoupon(couponType.Id, couponType.Category, giftCoupon.Count * profileIdList.Count, couponType.Prefix);

                    //            int profileIndex = 0;
                    //            foreach (var coupon in coupons)
                    //            {
                    //                var profileId = profileIdList.Skip(profileIndex).FirstOrDefault();
                    //                profileIndex++;
                    //                if (profileIndex == profileIdList.Count)
                    //                {
                    //                    profileIndex = 0;
                    //                }
                    //                coupon.BindingDate = DateTime.Now;
                    //                coupon.CouponChannelId = couponChannel;
                    //                coupon.Points = couponType.ExchangeNeedPoints;
                    //                coupon.FaceValue = couponType.FaceValue;
                    //                coupon.OwnerId = profileId;
                    //                if (couponType.Category == CouponCategory.Item)
                    //                {
                    //                    int paymentWayCode = GetPaymentCodebyCouponTypeId(giftCoupon.CouponTypeId) == null ? 2 : GetPaymentCodebyCouponTypeId(giftCoupon.CouponTypeId).PaymentWay;
                    //                    if (paymentWayCode != 2)
                    //                    {
                    //                        coupon.UnitPrice = paymentWayCode == 0 ? giftCoupon.UnitPrice : 0;
                    //                        if (paymentWayCode == 0 && giftCoupon.UnitPrice == 0)
                    //                        {
                    //                            userNotification.Content = "项目券单价不能为0";
                    //                            userNotification.Type = NotificationType.Error;
                    //                            throw new Exception(userNotification.Content);
                    //                        }
                    //                    }
                    //                }
                    //                if (couponType.Category == CouponCategory.Cash)
                    //                {
                    //                    coupon.UnitPrice = giftCoupon.UnitPrice;
                    //                }
                    //                if (couponType.TimeLimitMode == CouponTimeLimitMode.Date)
                    //                {
                    //                    coupon.BeginDate = couponType.TimeLimitBeginDate;
                    //                    coupon.EndDate = couponType.TimeLimitEndDate;
                    //                }
                    //                else
                    //                {
                    //                    coupon.BeginDate = DateTime.Now;
                    //                    coupon.EndDate = DateTime.Now.Date.AddDays(couponType.TimeLimitDays.Value + 1).AddSeconds(-1);
                    //                }
                    //                coupon.ExchangeMode = CouponExchangeMode.Gift;

                    //                var couponTransactionHistory = new CouponTransactionHistory()
                    //                {
                    //                    Id = _uniqueIdGeneratorService.Next(),
                    //                    BatchId = batchId,
                    //                    CouponId = coupon.Id,
                    //                    Description = couponType.Name,
                    //                    HistoryId = null,
                    //                    Points = 0.00M,
                    //                    TransactionDate = DateTime.Now,
                    //                    OperationType = CouponTransactionHistoryOperationType.Gift,
                    //                    TransactionId = profileIdAndTransactionId[profileId],
                    //                    ProfileId = profileId,
                    //                    CouponTypeId = coupon.CouponTypeId,
                    //                    FaceValue = couponType.FaceValue,
                    //                    PlaceCode = message.PlaceCode,
                    //                    HotelCode = message.HotelCode
                    //                };
                    //                couponTransactionHistoryList.Add(couponTransactionHistory);
                    //            }

                    //            couponList.AddRange(coupons);
                    //            break;
                    //        case CouponCategory.GeneralCoupon:
                    //            profileIndex = 0;
                    //            var rd = new Random();
                    //            for (int i = 0; i < count * profileIdList.Count; i++) //a 券10张  每人10张
                    //            {
                    //                var profileId = profileIdList.Skip(profileIndex % profileIdList.Count).FirstOrDefault();
                    //                profileIndex++;

                    //                var coupon = new Coupon();
                    //                coupon.Id = _uniqueIdGeneratorService.Next();
                    //                coupon.CouponTypeId = couponTypeId;
                    //                coupon.CouponCategory = couponType.Category;
                    //                var number = GetCouponNumber(rd);
                    //                coupon.Number = couponType.Prefix + number;
                    //                if (couponType.TimeLimitMode == CouponTimeLimitMode.Date)
                    //                {
                    //                    coupon.BeginDate = couponType.TimeLimitBeginDate;
                    //                    coupon.EndDate = couponType.TimeLimitEndDate;
                    //                }
                    //                else
                    //                {
                    //                    coupon.BeginDate = DateTime.Now;
                    //                    coupon.EndDate = DateTime.Now.Date.AddDays(couponType.TimeLimitDays.Value + 1).AddSeconds(-1);
                    //                }
                    //                coupon.FaceValue = couponType.FaceValue;
                    //                coupon.BindingDate = DateTime.Now;
                    //                coupon.OwnerId = profileId;
                    //                coupon.IsUsed = false;
                    //                coupon.IsExpired = false;
                    //                coupon.CouponChannelId = couponChannel;
                    //                coupon.Points = 0;
                    //                coupon.ExchangeMode = CouponExchangeMode.Gift;
                    //                coupon.Version = new byte[] { 1, 0, 0, 0 };

                    //                couponList.Add(coupon);

                    //                var couponTransactionHistory = new CouponTransactionHistory()
                    //                {
                    //                    Id = _uniqueIdGeneratorService.Next(),
                    //                    BatchId = batchId,
                    //                    CouponId = coupon.Id,
                    //                    Description = couponType.Name,
                    //                    HistoryId = null,
                    //                    Points = 0.00M,
                    //                    TransactionDate = DateTime.Now,
                    //                    OperationType = CouponTransactionHistoryOperationType.Gift,
                    //                    TransactionId = profileIdAndTransactionId[profileId],
                    //                    ProfileId = profileId,
                    //                    CouponTypeId = coupon.CouponTypeId,
                    //                    FaceValue = couponType.FaceValue

                    //                };
                    //                couponTransactionHistoryList.Add(couponTransactionHistory);
                    //            }
                    //            break;
                    //    }
                    //}
                    //_context.Coupon.AddRange(couponList);
                    //_context.Transaction.AddRange(transactionList);
                    //_context.CouponInventory.UpdateRange(couponInventoryList);
                    //_context.CouponTransactionHistory.AddRange(couponTransactionHistoryList);

                    //_context.SaveChanges();
                    #endregion
                    userNotification.Type = NotificationType.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                userNotification.Content = ex.Message;
                userNotification.Type = NotificationType.Error;
            }
            try
            {
                userNotification.InsertDate = DateTime.Now;
                userNotification.UpdateDate = DateTime.Now;
                _context.UserNotification.Add(userNotification);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            sw.Stop();
            _logger.LogInformation("批量送券用时：" + sw.ElapsedMilliseconds);

        }

        public Dictionary<long, CouponType> GetCouponTypeInfo(List<long> couponTypeIdList)
        {
            var dic = new Dictionary<long, CouponType>();
            var couponType = _context.CouponType.AsNoTracking().Where(o => couponTypeIdList.Contains(o.Id)).ToList();
            foreach (var c in couponType)
            {
                dic.Add(c.Id, c);
            }
            return dic;
        }

        public UserNotification CouponTypeInsert(List<ProfileIdCouponTypeId> profileIdCouponTypeDic, BatchGiftCouponMessage batchGiftCouponMessage, UserNotification userNotification, Dictionary<long, CouponType> couponTypeInfo)
        {
            //var sw = Stopwatch.StartNew();
            //var couponInventoryMQList = new List<CouponUpdateInventoryMessage>();
            try
            {
                var transactionList = new List<Transaction>();
                var batchId = Guid.NewGuid();
                var couponList = new List<Coupon>();
                //var couponInventoryList = new List<CouponInventory>();
                var couponTransactionHistoryList = new List<CouponTransactionHistory>();

                foreach (var dic in profileIdCouponTypeDic)
                {
                    var transactionId = _uniqueIdGeneratorService.Next();
                    var transaction = new Transaction()
                    {
                        Id = transactionId,
                        Points = 0,
                        ProfileId = dic.ProfileId,
                        TransactionDate = DateTime.Now,
                        TransactionType = TransactionType.GiftCoupons,
                        TransactionNumber = Guid.NewGuid().ToString(),
                        HotelCode = batchGiftCouponMessage.HotelCode,
                        PlaceCode = batchGiftCouponMessage.PlaceCode,
                        Description = "LPS批量赠送券",
                        InsertDate = DateTime.Now,
                        InsertUser = batchGiftCouponMessage.UserCode,
                        UpdateDate = DateTime.Now,
                        UpdateUser = batchGiftCouponMessage.UserCode
                    };
                    transactionList.Add(transaction);

                    foreach (var d in dic.CouponTypeDetail)
                    {
                        var couponType = couponTypeInfo[d.CouponTypeId];

                        var count = d.Count;

                        switch (couponType.Category)
                        {
                            case CouponCategory.Cash:
                            case CouponCategory.Free:
                            case CouponCategory.Gift:
                            case CouponCategory.Item:
                            case CouponCategory.Discount:
                                var coupons = GetCoupons(couponType.Id, couponType.Category, count, couponType.Prefix, d.SerialNumberPrefix, d.SerialNumber);
                                foreach (var coupon in coupons)
                                {
                                    
                                    coupon.BindingDate = DateTime.Now;
                                    coupon.CouponChannelId = batchGiftCouponMessage.CouponChannel;
                                    coupon.Points = couponType.ExchangeNeedPoints;
                                    coupon.FaceValue = couponType.FaceValue;
                                    coupon.OwnerId = dic.ProfileId;
                                    if (couponType.ExchangeMode == CouponExchangeMode.Sell)
                                    {
                                        var paymentWay = GetPaymentCodebyCouponTypeId(d.CouponTypeId);
                                        int paymentWayCode = paymentWay == null ? 2 : paymentWay.PaymentWay;
                                        if (paymentWayCode != 2)
                                        {
                                            coupon.UnitPrice = paymentWayCode == 0 ? d.UnitPrice : 0;
                                            if (paymentWayCode == 0 && d.UnitPrice == 0)
                                            {
                                                userNotification.Content = "项目券单价不能为0";
                                                userNotification.Type = NotificationType.Error;
                                                throw new Exception(userNotification.Content);
                                            }
                                        }
                                    }
                                    if (couponType.Category == CouponCategory.Cash || couponType.Category == CouponCategory.Item)
                                    {
                                        coupon.UnitPrice = d.UnitPrice;
                                    }
                                    if (couponType.TimeLimitMode == CouponTimeLimitMode.Date)
                                    {
                                        coupon.BeginDate = couponType.TimeLimitBeginDate;
                                        coupon.EndDate = couponType.TimeLimitEndDate;
                                    }
                                    else
                                    {
                                        coupon.BeginDate = DateTime.Now;
                                        coupon.EndDate = DateTime.Now.Date.AddDays(couponType.TimeLimitDays.Value + 1).AddSeconds(-1);
                                    }
                                    coupon.ExchangeMode = CouponExchangeMode.Gift;
                                    coupon.IsActivation = true;
                                    if (!batchGiftCouponMessage.IsActivation)
                                    {
                                        coupon.IsActivation = false;
                                        coupon.EndDate = null;
                                    }
                                    var couponTransactionHistory = new CouponTransactionHistory()
                                    {
                                        Id = _uniqueIdGeneratorService.Next(),
                                        BatchId = batchId,
                                        CouponId = coupon.Id,
                                        Description = couponType.Name,
                                        HistoryId = null,
                                        Points = 0.00M,
                                        TransactionDate = DateTime.Now,
                                        OperationType = CouponTransactionHistoryOperationType.Gift,
                                        TransactionId = transactionId,
                                        ProfileId = dic.ProfileId,
                                        CouponTypeId = coupon.CouponTypeId,
                                        FaceValue = couponType.FaceValue,
                                        PlaceCode = batchGiftCouponMessage.PlaceCode,
                                        HotelCode = batchGiftCouponMessage.HotelCode,
                                        InsertDate = DateTime.Now,
                                        InsertUser = batchGiftCouponMessage.UserCode,
                                        UpdateDate = DateTime.Now,
                                        UpdateUser = batchGiftCouponMessage.UserCode,
                                        OperatorId = batchGiftCouponMessage.UserCode
                                    };
                                    couponTransactionHistoryList.Add(couponTransactionHistory);
                                }

                                couponList.AddRange(coupons);
                                CouponUpdateInventoryMessage couponUpdateInventoryMessage = new CouponUpdateInventoryMessage()
                                {
                                    CouponTypeId = couponType.Id,
                                    Inventory = -count,
                                    NeedManageInventory = couponType.NeedManageInventory.Value,
                                    IsDeductInventory = false
                                };
                                _messageQueueProducer.PublishInternal(couponUpdateInventoryMessage);

                                break;
                            case CouponCategory.GeneralCoupon:
                                var rd = new Random();
                                for (int i = 0; i < count; i++)
                                {
                                    var coupon = new Coupon();
                                    coupon.Id = _uniqueIdGeneratorService.Next();
                                    coupon.CouponTypeId = d.CouponTypeId;
                                    coupon.CouponCategory = couponType.Category;
                                    var number = GetCouponNumber(rd);
                                    coupon.Number = couponType.Prefix + number;
                                    if (couponType.TimeLimitMode == CouponTimeLimitMode.Date)
                                    {
                                        coupon.BeginDate = couponType.TimeLimitBeginDate;
                                        coupon.EndDate = couponType.TimeLimitEndDate;
                                    }
                                    else
                                    {
                                        coupon.BeginDate = DateTime.Now;
                                        coupon.EndDate = DateTime.Now.Date.AddDays(couponType.TimeLimitDays.Value + 1).AddSeconds(-1);
                                    }
                                    coupon.FaceValue = couponType.FaceValue;
                                    coupon.BindingDate = DateTime.Now;
                                    coupon.OwnerId = dic.ProfileId;
                                    coupon.IsUsed = false;
                                    coupon.IsExpired = false;
                                    coupon.CouponChannelId = batchGiftCouponMessage.CouponChannel;
                                    coupon.Points = 0;
                                    coupon.ExchangeMode = CouponExchangeMode.Gift;
                                    coupon.Version = new byte[] { 1, 0, 0, 0 };
                                    coupon.InsertDate = DateTime.Now;
                                    coupon.InsertUser = batchGiftCouponMessage.UserCode;
                                    coupon.UpdateDate = DateTime.Now;
                                    coupon.UpdateUser = batchGiftCouponMessage.UserCode;
                                    coupon.IsActivation = true;
                                    if (!batchGiftCouponMessage.IsActivation)
                                    {
                                        coupon.IsActivation = false;
                                        coupon.EndDate = null;
                                    }
                                    couponList.Add(coupon);

                                    var couponTransactionHistory = new CouponTransactionHistory()
                                    {
                                        Id = _uniqueIdGeneratorService.Next(),
                                        BatchId = batchId,
                                        CouponId = coupon.Id,
                                        Description = couponType.Name,
                                        HistoryId = null,
                                        Points = 0.00M,
                                        TransactionDate = DateTime.Now,
                                        OperationType = CouponTransactionHistoryOperationType.Gift,
                                        TransactionId = transactionId,
                                        ProfileId = dic.ProfileId,
                                        CouponTypeId = coupon.CouponTypeId,
                                        FaceValue = couponType.FaceValue,
                                        InsertDate = DateTime.Now,
                                        InsertUser = batchGiftCouponMessage.UserCode,
                                        UpdateDate = DateTime.Now,
                                        UpdateUser = batchGiftCouponMessage.UserCode
                                    };
                                    couponTransactionHistoryList.Add(couponTransactionHistory);
                                }
                                break;
                        }
                    }
                }
                //_logger.LogInformation("init：" + sw.ElapsedMilliseconds);
                //sw.Restart();

                //_context.Coupon.AddRange(couponList);
                //_context.Transaction.AddRange(transactionList);
                //_context.CouponInventory.UpdateRange(couponInventoryList);
                //_context.CouponTransactionHistory.AddRange(couponTransactionHistoryList);

                //_context.ChangeTracker.AutoDetectChangesEnabled = false;
                //_context.SaveChanges();

                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.BulkInsert(couponList);
                    _context.BulkInsert(transactionList);
                    _context.BulkInsert(couponTransactionHistoryList);
                    transaction.Commit();
                }
                foreach (var item in transactionList)
                {
                    _messageQueueProducer.PublishInternal(new WorkerBatchGiftCouponMessage()

                    {
                        TransactionId = item.Id,
                        ProfileId = item.ProfileId

                    });
                }
                //_logger.LogInformation("SaveChanges：" + sw.ElapsedMilliseconds);
                //sw.Restart();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
                userNotification.Content = ex.Message;
                userNotification.Type = NotificationType.Error;
                batchGiftCouponMessage.ProfileIdList = profileIdCouponTypeDic.Select(o => o.ProfileId).ToList();
                _messageQueueProducer.PublishInternal(batchGiftCouponMessage);
                return userNotification;
            }
            return userNotification;
        }

        public List<Coupon> GetCoupons(long id, CouponCategory type, int totalCount, string prefix, string SerialNumberPrefix, string SerialNumber)
        {
            var rd = new Random();
            var list = new List<Coupon>();
            for (var i = 0; i < totalCount; i++)
            {
                var number = GetCouponNumber(rd);
                var coupon = new Coupon()
                {
                    Id = _uniqueIdGeneratorService.Next(),
                    CouponCategory = type,
                    CouponTypeId = id,
                    Number = prefix + number,
                    Version = new byte[] { 1, 0, 0, 0 },
                    InsertDate = DateTime.Now,
                    InsertUser = "admin",
                    UpdateDate = DateTime.Now,
                    UpdateUser = "admin"
                };
                if (!string.IsNullOrEmpty(SerialNumberPrefix))
                {
                    coupon.SerialNumberPrefix = SerialNumberPrefix;
                }
                if (!string.IsNullOrEmpty(SerialNumber))
                {
                    int tLen = SerialNumber.Length - SerialNumber.TrimStart('0').Length;
                    if (tLen > 0)
                    {
                        coupon.SerialNumber = (int.Parse(SerialNumber) + i).ToString().PadLeft(tLen + 1, '0');
                        if(SerialNumber.Length!= coupon.SerialNumber.Length)
                        {
                            coupon.SerialNumber.Substring(1, SerialNumber.Length - 1);
                        }
                    }
                    else
                    {
                        coupon.SerialNumber = (int.Parse(SerialNumber) + i).ToString();
                    }
                }
                list.Add(coupon);
            }

            return list;
        }

        public CouponTypePaymentWay GetPaymentCodebyCouponTypeId(long CouponTypeId)
        {
            var paymentWay = _context.CouponTypePaymentWay_Map.AsNoTracking().Where(m => m.CouponTypeId == CouponTypeId);
            if (!paymentWay.Any())
            {
                return null;
            }
            return _context.CouponTypePaymentWay.AsNoTracking().First(m => m.Id == paymentWay.FirstOrDefault().PaymentWayId);
        }
        private List<CouponType> GetCouponListId(List<long> rd)
        {
            return _context.CouponType.Where(t => rd.Contains(t.Id)).ToList();
        }
        //private bool GetIsSerialNumber(List<string> str,string SerialNumberPrefix)
        //{
        //    var list = new List<Coupon>();
        //    if (!string.IsNullOrEmpty(SerialNumberPrefix))
        //    {
        //         list = _context.Coupon.Where(t => t.SerialNumberPrefix == SerialNumberPrefix).ToList();
        //    }
        //    else
        //    {
        //         list = _context.Coupon.Where(t => t.SerialNumberPrefix == null).ToList();
        //    }
        //    if (list.Where(t => str.Contains(t.SerialNumber)).Count() > 0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        private string GetCouponNumber(Random rd)
        {
            var number = "";
            for (var a = 0; a < 16; a++)
            {
                number += rd.Next(0, 10).ToString();
            }
            return number;
        }
    }
}
