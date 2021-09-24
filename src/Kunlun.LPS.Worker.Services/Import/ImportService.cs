using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kunlun.LPS.Worker.Services.Import
{
    public class ImportService : IImportService
    {
        private readonly ILogger<ImportService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IConfigurationService<Place> _placeService;
        private readonly IConfigurationService<Hotel> _hotelService;
        private readonly IConfigurationService<MealPeriod> _mealPeriodService;
        private readonly IConfigurationService<PlaceM> _placeMService;
        private readonly IConsumeHistoryService _consumeHistoryService;

        public ImportService(
             ILogger<ImportService> logger,
             LPSWorkerContext context,
             IUniqueIdGeneratorService uniqueIdGeneratorService,
             IConfigurationService<Place> placeService,
             IConfigurationService<Hotel> hotelService,
             IConfigurationService<MealPeriod> mealPeriodService,
             IConfigurationService<PlaceM> placeMService,
             IConsumeHistoryService consumeHistoryService
             )
        {
            _logger = logger;
            _context = context;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _placeService = placeService;
            _hotelService = hotelService;
            _mealPeriodService = mealPeriodService;
            _placeMService = placeMService;
            _consumeHistoryService = consumeHistoryService;
        }

        public void ImportConsume(ImportConsumeMessage message)
        {
            var userNotification = new UserNotification
            {
                Id = _uniqueIdGeneratorService.Next(),
                Content = $"Import Consume {message.Path} success",
                UserCode = message.UserCode,
                AlreadyRead = false,
                InsertUser = message.UserCode,
                UpdateUser = message.UserCode
            };
            try
            {
                var excelStream = File.Open(message.Path, FileMode.Open, FileAccess.Read);
                var package = new ExcelPackage(excelStream);
                excelStream.Dispose();
                ExcelWorkbook workbook = package.Workbook;
                if (workbook != null && workbook.Worksheets.Count > 0)
                {
                    if (message.ImportType == ImportType.RoomConsumeInputDefault)
                    {
                        var worksheet = workbook.Worksheets[0];
                        var list = ConvertToRoomHistory(worksheet, message);
                        _context.ConsumeHistory.AddRange(list);
                    }
                    else if (message.ImportType == ImportType.RoomConsumeInputTransactionCode)
                    {
                        var worksheet = workbook.Worksheets[0];
                        var list = ConvertToRoomHistory(worksheet, message);
                        var detailSheet = workbook.Worksheets[1];
                        var detailList = ConvertToRoomDetail(detailSheet, list);
                        _context.ConsumeHistory.AddRange(list);
                        _context.ConsumeHistoryDetail.AddRange(detailList);
                    }
                    else if (message.ImportType == ImportType.FoodConsumeInput || message.ImportType == ImportType.MultiConsumeInput)
                    {
                        var worksheet = workbook.Worksheets[0];
                        var list = ConvertToFoodAndMultiHistory(worksheet, message);
                        if (_consumeHistoryService.ExistFoodMultiConsumption(list))
                        {
                            throw new Exception($"{message.ImportType} Consumption Exist!");
                        }
                        var detailSheet = workbook.Worksheets[1];
                        var detailList = ConvertToFoodAndMultiDetail(detailSheet, list);
                        _context.ConsumeHistory.AddRange(list);
                        _context.ConsumeHistoryDetail.AddRange(detailList);
                    }
                }
                userNotification.Type = NotificationType.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                userNotification.Content = ex.Message;
                userNotification.Type = NotificationType.Error;
            }
            userNotification.InsertDate = DateTime.Now;
            userNotification.UpdateDate = DateTime.Now;
            _context.UserNotification.Add(userNotification);
            try
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = false;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        public List<ConsumeHistory> ConvertToRoomHistory(ExcelWorksheet worksheet, ImportConsumeMessage message)
        {
            var list = new List<ConsumeHistory>();
            int rows = worksheet.Dimension.End.Row;

            for (int i = 2; i <= rows; i++)
            {
                var consumeHistory = new ConsumeHistory();
                consumeHistory.ImportId = null;
                if (!String.IsNullOrEmpty(worksheet.Cells[i, 1].Value?.ToString()))
                {
                    consumeHistory.ImportId = Convert.ToInt32(worksheet.Cells[i, 1].Value?.ToString());
                }
                consumeHistory.MembershipCardNumber = worksheet.Cells[i, 2].Value?.ToString();
                consumeHistory.RM_ArrivalTime = Convert.ToDateTime(worksheet.Cells[i, 3].Value?.ToString());
                consumeHistory.RM_DepartureTime = Convert.ToDateTime(worksheet.Cells[i, 4].Value?.ToString());
                consumeHistory.RM_BookerMembershipCardNumber = worksheet.Cells[i, 5].Value?.ToString();
                consumeHistory.RM_ChannelCode = worksheet.Cells[i, 6].Value?.ToString();
                consumeHistory.RM_RoomTypeCode = worksheet.Cells[i, 7].Value?.ToString();
                consumeHistory.RM_RoomNumber = worksheet.Cells[i, 8].Value?.ToString();
                consumeHistory.RM_PaymentCode = worksheet.Cells[i, 9].Value?.ToString();
                consumeHistory.RM_RoomRateCode = worksheet.Cells[i, 10].Value?.ToString();
                consumeHistory.RM_RoomRate = Convert.ToDecimal(worksheet.Cells[i, 11].Value?.ToString());
                consumeHistory.RM_RoomRevenue = Convert.ToDecimal(worksheet.Cells[i, 12].Value?.ToString());
                consumeHistory.RM_FbRevenue = Convert.ToDecimal(worksheet.Cells[i, 13].Value?.ToString());
                consumeHistory.RM_OtherRevenue = Convert.ToDecimal(worksheet.Cells[i, 14].Value?.ToString());
                consumeHistory.Tax = Convert.ToDecimal(worksheet.Cells[i, 15].Value?.ToString());
                consumeHistory.TotalAmount = Convert.ToDecimal(worksheet.Cells[i, 16].Value?.ToString());
                consumeHistory.RM_SourceCode = worksheet.Cells[i, 17].Value?.ToString();
                consumeHistory.RM_MarketCode = worksheet.Cells[i, 18].Value?.ToString();
                consumeHistory.CheckNumber = worksheet.Cells[i, 19].Value?.ToString();
                consumeHistory.OperatorCode = worksheet.Cells[i, 20].Value?.ToString();
                consumeHistory.Guests = Convert.ToInt32(worksheet.Cells[i, 21].Value?.ToString());
                consumeHistory.Description = worksheet.Cells[i, 22].Value?.ToString();
                var hotel = _hotelService.GetAllFromCache().FirstOrDefault(c => c.Code == worksheet.Cells[i, 23].Value?.ToString());
                if (hotel == null)
                {
                    throw new Exception($"Default Row :{i} hotel not exists");
                }
                consumeHistory.StoreCode = hotel.Code;
                var place = _placeService.GetAllFromCache().FirstOrDefault(c => c.Code == worksheet.Cells[i, 24].Value?.ToString());
                if (place == null)
                {
                    throw new Exception($"Default Row :{i} place not exists");
                }
                consumeHistory.OutletCode = place.Code;
                consumeHistory.FFPNumber = worksheet.Cells[i, 25].Value?.ToString();
                consumeHistory.FFPCode = worksheet.Cells[i, 26].Value?.ToString();

                consumeHistory.Id = _uniqueIdGeneratorService.Next();
                consumeHistory.RM_Nights = (consumeHistory.RM_DepartureTime - consumeHistory.RM_ArrivalTime).Value.Days;
                consumeHistory.SubTotalAmount = (consumeHistory.RM_RoomRevenue + consumeHistory.RM_FbRevenue + consumeHistory.RM_OtherRevenue).Value;
                consumeHistory.SurCharges = 0;
                consumeHistory.CurrencyCode = hotel.Currency;

                consumeHistory.TraceId = Guid.NewGuid().ToString();
                consumeHistory.ConsumeTypeCode = "R";
                consumeHistory.TransactionTime = consumeHistory.RM_DepartureTime.Value.Date;
                consumeHistory.IsVoid = false;
                consumeHistory.Points = 0;
                consumeHistory.Growth = 0;
                consumeHistory.Mileages = 0;
                consumeHistory.IsComplete = true;

                consumeHistory.InsertUser = message.UserCode;
                consumeHistory.InsertDate = DateTime.Now;
                consumeHistory.UpdateUser = message.UserCode;
                consumeHistory.UpdateDate = DateTime.Now;
                list.Add(consumeHistory);
            }
            if (list.Count > 0)
            {
                var memberNumber = list.Select(c => c.MembershipCardNumber).Union(list.Select(c => c.RM_BookerMembershipCardNumber));
                var ffp = list.Select(c => new { c.FFPCode, c.FFPNumber });
                var memberInfoDic = new Dictionary<string, dynamic>();
                var ffpDic = new Dictionary<string, long>();
                var memberInfo = from m in _context.MembershipCard.AsNoTracking()
                                 where memberNumber.Contains(m.MembershipCardNumber)
                                 select new
                                 {
                                     m.MembershipCardNumber,
                                     MembershipCardId = m.Id,
                                     m.MainMembershipCardId,
                                     m.ProfileId
                                 };
                foreach (var i in memberInfo)
                {
                    memberInfoDic.Add(i.MembershipCardNumber, i);
                }

                var ffpInfo = from p in _context.ProfileExternalMembership.AsNoTracking()
                              join e in _context.ExternalMembershipProvider.AsNoTracking() on p.ProviderId equals e.Id
                              where p.ProviderType == ExternalMembershipProviderHandler.FFP
                              && p.IsValid == true
                              && ffp.Select(c => c.FFPNumber).Contains(p.ProviderKey)
                              && ffp.Select(c => c.FFPCode).Contains(e.Code)
                              select new
                              {
                                  FFPCode = e.Code,
                                  FFPNumber = p.ProviderKey,
                                  ProfileExternalMembershipId = p.Id
                              };
                foreach (var f in ffpInfo)
                {
                    ffpDic.Add($"{f.FFPCode}|{f.FFPNumber}", f.ProfileExternalMembershipId);
                }

                foreach (var i in list)
                {
                    i.MembershipCardId = memberInfoDic[i.MembershipCardNumber].MembershipCardId;
                    i.MainMembershipCardId = (memberInfoDic[i.MembershipCardNumber].MainMembershipCardId as long?).HasValue ? memberInfoDic[i.MembershipCardNumber].MainMembershipCardId : memberInfoDic[i.MembershipCardNumber].MembershipCardId;
                    i.ProfileId = memberInfoDic[i.MembershipCardNumber].ProfileId;
                    if (!String.IsNullOrEmpty(i.RM_BookerMembershipCardNumber))
                    {
                        i.RM_BookerMembershipCardId = memberInfoDic[i.RM_BookerMembershipCardNumber].MembershipCardId;
                    }
                    if (!String.IsNullOrEmpty(i.FFPCode) && !String.IsNullOrEmpty(i.FFPNumber))
                    {
                        i.ProfileExternalMembershipId = ffpDic[$"{i.FFPCode}|{i.FFPNumber}"];
                    }
                }
            }
            return list;
        }

        public List<ConsumeHistoryDetail> ConvertToRoomDetail(ExcelWorksheet worksheet, List<ConsumeHistory> consumeHistoryList) 
        {
            var list = new List<ConsumeHistoryDetail>();
            int rows = worksheet.Dimension.End.Row;

            for (int i = 2; i <= rows; i++)
            {
                var consumeHistoryDetail = new ConsumeHistoryDetail();
                var importId = Convert.ToInt32(worksheet.Cells[i, 1].Value?.ToString());
                var consumeHistory = consumeHistoryList.FirstOrDefault(c => c.ImportId == importId);
                if (consumeHistory == null)
                {
                    throw new Exception($"Transaction Row :{i} Not found consume history id:{importId}");
                }
                consumeHistoryDetail.HistoryId = consumeHistory.Id;
                consumeHistoryDetail.PostTime = Convert.ToDateTime(worksheet.Cells[i, 2].Value?.ToString());
                consumeHistoryDetail.ItemType = worksheet.Cells[i, 3].Value?.ToString();
                consumeHistoryDetail.ItemCode = worksheet.Cells[i, 4].Value?.ToString();
                consumeHistoryDetail.ItemName = worksheet.Cells[i, 5].Value?.ToString();
                consumeHistoryDetail.RateCode = worksheet.Cells[i, 6].Value?.ToString();
                consumeHistoryDetail.RoomNo = worksheet.Cells[i, 7].Value?.ToString();
                consumeHistoryDetail.UnitPrice = Convert.ToDecimal(worksheet.Cells[i, 8].Value?.ToString());
                consumeHistoryDetail.Quantity = Convert.ToDecimal(worksheet.Cells[i, 9].Value?.ToString());
                consumeHistoryDetail.CurrencyCode = worksheet.Cells[i, 10].Value?.ToString();
                consumeHistoryDetail.Amount = Convert.ToDecimal(worksheet.Cells[i, 11].Value?.ToString());
                consumeHistoryDetail.MembershipCardNumber = worksheet.Cells[i, 12].Value?.ToString();
                consumeHistoryDetail.Description = worksheet.Cells[i, 13].Value?.ToString();

                consumeHistoryDetail.Id = _uniqueIdGeneratorService.Next();
                consumeHistoryDetail.Tax = 0;

                consumeHistoryDetail.InsertUser = consumeHistory.InsertUser;
                consumeHistoryDetail.InsertDate = DateTime.Now;
                consumeHistoryDetail.UpdateUser = consumeHistory.UpdateUser;
                consumeHistoryDetail.UpdateDate = DateTime.Now;
                list.Add(consumeHistoryDetail);
            }         
            return list;
        }

        public List<ConsumeHistory> ConvertToFoodAndMultiHistory(ExcelWorksheet worksheet, ImportConsumeMessage message)
        {
            var list = new List<ConsumeHistory>();
            int rows = worksheet.Dimension.End.Row;

            for (int i = 2; i <= rows; i++)
            {
                var consumeHistory = new ConsumeHistory();
                consumeHistory.ImportId = null;
                if (!String.IsNullOrEmpty(worksheet.Cells[i, 1].Value?.ToString()))
                {
                    consumeHistory.ImportId = Convert.ToInt32(worksheet.Cells[i, 1].Value?.ToString());
                }
                consumeHistory.CheckNumber = worksheet.Cells[i, 2].Value?.ToString();
                consumeHistory.TransactionTime = Convert.ToDateTime(worksheet.Cells[i, 3].Value?.ToString());
                var hotel = _hotelService.GetAllFromCache().FirstOrDefault(c => c.Code == worksheet.Cells[i, 4].Value?.ToString());
                if (hotel == null)
                {
                    throw new Exception($"Default Row :{i} hotel not exists");
                }
                consumeHistory.StoreCode = hotel.Code;
                var placeCode = (from p in _placeService.GetAllFromCache()
                                 join m in _placeMService.GetAllFromCache() on p.MCode equals m.Code
                                 where m.HotelCode == hotel.Code
                                 && p.PosPlaceCode == worksheet.Cells[i, 5].Value?.ToString()
                                 select p.Code).FirstOrDefault();
                if (String.IsNullOrEmpty(placeCode))
                {
                    throw new Exception($"Default Row :{i} place not exists");
                }
                consumeHistory.OutletCode = placeCode;
                consumeHistory.MembershipCardNumber = worksheet.Cells[i, 6].Value?.ToString();
                consumeHistory.TraceId = worksheet.Cells[i, 7].Value?.ToString();
                consumeHistory.FB_CheckOpenTime = Convert.ToDateTime(worksheet.Cells[i, 8].Value?.ToString() ?? worksheet.Cells[i, 3].Value?.ToString());
                consumeHistory.TotalAmount = Convert.ToDecimal(worksheet.Cells[i, 9].Value?.ToString());
                consumeHistory.CurrencyCode = worksheet.Cells[i, 10].Value?.ToString();
                consumeHistory.SubTotalAmount = Convert.ToDecimal(worksheet.Cells[i, 11].Value?.ToString());
                consumeHistory.Tax = Convert.ToDecimal(worksheet.Cells[i, 12].Value?.ToString());
                consumeHistory.SurCharges = Convert.ToDecimal(worksheet.Cells[i, 13].Value?.ToString());
                consumeHistory.Guests = Convert.ToInt32(worksheet.Cells[i, 14].Value?.ToString());
                consumeHistory.ExternalOperatorId = worksheet.Cells[i, 15].Value?.ToString();
                consumeHistory.Description = worksheet.Cells[i, 16].Value?.ToString();
                consumeHistory.GuestRemark = worksheet.Cells[i, 17].Value?.ToString();
                consumeHistory.InternalRemark = worksheet.Cells[i, 18].Value?.ToString();
                consumeHistory.TerminalId = worksheet.Cells[i, 19].Value?.ToString();
                consumeHistory.FB_OrderTypeCode = worksheet.Cells[i, 20].Value?.ToString();
                consumeHistory.FB_TableNumber = worksheet.Cells[i, 21].Value?.ToString();
                consumeHistory.RM_ChannelCode = worksheet.Cells[i, 23].Value?.ToString();

                consumeHistory.Id = _uniqueIdGeneratorService.Next();
                var mealPeriod = _mealPeriodService.GetAllFromCache().FirstOrDefault(c => c.Code == worksheet.Cells[i, 22].Value?.ToString());
                if (message.ImportType == ImportType.FoodConsumeInput && mealPeriod == null)
                {
                    throw new Exception($"Default Row :{i} mealPeriod not exists");
                }
                consumeHistory.FB_MealPeriodId = mealPeriod?.Id;
                consumeHistory.OperatorCode = message.UserCode;

                consumeHistory.ConsumeTypeCode = message.ConsumeTypeCode;
                consumeHistory.IsVoid = false;
                consumeHistory.Points = 0;
                consumeHistory.Growth = 0;
                consumeHistory.Mileages = 0;
                consumeHistory.IsComplete = true;
                consumeHistory.TraceId = Guid.NewGuid().ToString();

                consumeHistory.InsertUser = message.UserCode;
                consumeHistory.InsertDate = DateTime.Now;
                consumeHistory.UpdateUser = message.UserCode;
                consumeHistory.UpdateDate = DateTime.Now;
                list.Add(consumeHistory);
            }
            if (list.Count > 0)
            {
                var memberNumber = list.Select(c => c.MembershipCardNumber);
                var memberInfoDic = new Dictionary<string, dynamic>();
                var memberInfo = from m in _context.MembershipCard.AsNoTracking()
                                 where memberNumber.Contains(m.MembershipCardNumber)
                                 select new
                                 {
                                     m.MembershipCardNumber,
                                     MembershipCardId = m.Id,
                                     m.MainMembershipCardId,
                                     m.ProfileId
                                 };
                foreach (var i in memberInfo)
                {
                    memberInfoDic.Add(i.MembershipCardNumber, i);
                }

                foreach (var i in list)
                {
                    i.MembershipCardId = memberInfoDic[i.MembershipCardNumber].MembershipCardId;
                    i.MainMembershipCardId = (memberInfoDic[i.MembershipCardNumber].MainMembershipCardId as long?).HasValue ? memberInfoDic[i.MembershipCardNumber].MainMembershipCardId : memberInfoDic[i.MembershipCardNumber].MembershipCardId;
                    i.ProfileId = memberInfoDic[i.MembershipCardNumber].ProfileId;
                }
            }
            return list;
        }

        public List<ConsumeHistoryDetail> ConvertToFoodAndMultiDetail(ExcelWorksheet worksheet, List<ConsumeHistory> consumeHistoryList)
        {
            var list = new List<ConsumeHistoryDetail>();
            int rows = worksheet.Dimension.End.Row;

            for (int i = 2; i <= rows; i++)
            {
                var consumeHistoryDetail = new ConsumeHistoryDetail();
                var importId = Convert.ToInt32(worksheet.Cells[i, 1].Value?.ToString());
                var consumeHistory = consumeHistoryList.FirstOrDefault(c => c.ImportId == importId);
                if (consumeHistory == null)
                {
                    throw new Exception($"Transaction Row :{i} Not found consume history id:{importId}");
                }
                consumeHistoryDetail.HistoryId = consumeHistory.Id;
                consumeHistoryDetail.PostTime = Convert.ToDateTime(worksheet.Cells[i, 3].Value?.ToString());
                consumeHistoryDetail.ItemType = worksheet.Cells[i, 6].Value?.ToString();
                consumeHistoryDetail.ItemCode = worksheet.Cells[i, 7].Value?.ToString();
                consumeHistoryDetail.ItemName = worksheet.Cells[i, 8].Value?.ToString();
                consumeHistoryDetail.UnitPrice = Convert.ToDecimal(worksheet.Cells[i, 9].Value?.ToString());
                consumeHistoryDetail.Quantity = Convert.ToDecimal(worksheet.Cells[i, 10].Value?.ToString());
                consumeHistoryDetail.Amount = Convert.ToDecimal(worksheet.Cells[i, 11].Value?.ToString());
                consumeHistoryDetail.CurrencyCode = worksheet.Cells[i, 12].Value?.ToString();
                consumeHistoryDetail.Tax = Convert.ToDecimal(worksheet.Cells[i, 13].Value?.ToString());
                consumeHistoryDetail.Description = worksheet.Cells[i, 14].Value?.ToString();
                consumeHistoryDetail.MembershipCardNumber = worksheet.Cells[i, 15].Value?.ToString();

                consumeHistoryDetail.Id = _uniqueIdGeneratorService.Next();

                consumeHistoryDetail.InsertUser = consumeHistory.InsertUser;
                consumeHistoryDetail.InsertDate = DateTime.Now;
                consumeHistoryDetail.UpdateUser = consumeHistory.UpdateUser;
                consumeHistoryDetail.UpdateDate = DateTime.Now;
                list.Add(consumeHistoryDetail);
            }
            return list;
        }
    }
}
