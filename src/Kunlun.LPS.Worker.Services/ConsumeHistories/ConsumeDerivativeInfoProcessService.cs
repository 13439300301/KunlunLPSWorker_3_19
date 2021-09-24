using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Data;
using Kunlun.LPS.Worker.Services.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kunlun.LPS.Worker.Services.ConsumeHistories
{
    public class ConsumeDerivativeInfoProcessService : IConsumeDerivativeInfoProcessService
    {
        private readonly ILogger<ConsumeDerivativeInfoProcessService> _logger;
        private readonly LPSWorkerContext _context;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly IConfigurationService<FbItem> _fbItemService;

        public ConsumeDerivativeInfoProcessService(
            ILogger<ConsumeDerivativeInfoProcessService> logger,
            LPSWorkerContext context,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            IConfigurationService<FbItem> fbItemService)
        {
            _logger = logger;
            _context = context;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _fbItemService = fbItemService;

            _logger.LogInformation(nameof(ConsumeDerivativeInfoProcessService));
        }

        public void ConsumeDerivativeInfoProcess(long historyId)
        {
            var consumeHistory = _context.ConsumeHistory.Where(t => t.Id == historyId).FirstOrDefault();
            var consumeHistoryDetails = _context.ConsumeHistoryDetail.Where(t => t.HistoryId == historyId).ToList();

            //var fbItems = _fbItemService.GetAllFromCache().Where(t => t.HotelCode == consumeHistory.StoreCode).ToList();
            var fbItems = _context.FbItem.Where(t => t.HotelCode == consumeHistory.StoreCode).ToList();

            List<FbItem> fbItemList = new List<FbItem>();
            foreach(var d in consumeHistoryDetails)
            {
                FbItem fbItem = new FbItem();
                if (d.ItemType == "C" || d.ItemType == "Discount" || d.ItemType == "SurCharge" || d.ItemType == "FC")
                {
                    if (!fbItems.Any(t => t.Code.ToUpper() == d.ItemCode.ToUpper()))
                    {
                        fbItem.Code = d.ItemCode;
                        fbItem.Name = d.ItemName;
                        fbItem.Price = d.UnitPrice;
                        fbItem.HotelCode = consumeHistory.StoreCode;
                        fbItem.InsertUser = "admin";
                        fbItem.InsertDate = DateTime.Now;
                        fbItem.UpdateUser = "admin";
                        fbItem.UpdateDate = DateTime.Now;
                        fbItemList.Add(fbItem);
                    }
                }
                
            }
            fbItemList = fbItemList.Distinct(new Compare()).ToList();
            _context.FbItem.AddRange(fbItemList);

            List<ConsumeHistoryMetadata> consumeHistoryMetadatas = new List<ConsumeHistoryMetadata>();

            var transactionIdList = _context.Transaction.Where(t =>
            t.PlaceCode == consumeHistory.OutletCode &&
            t.CheckNumber == consumeHistory.CheckNumber &&
            t.TransactionType == TransactionType.Consumption &&
            t.TransactionDate.Date == consumeHistory.TransactionTime.Date).Select(t => t.Id).ToList();
            if (transactionIdList.Count > 0)
            {
                foreach (var transactionId in transactionIdList)
                {
                    if (!consumeHistoryMetadatas.Any(t => t.Value == transactionId.ToString()))
                    {
                        ConsumeHistoryMetadata consumeHistoryMetadata = new ConsumeHistoryMetadata();
                        consumeHistoryMetadata.Id = _uniqueIdGeneratorService.Next();
                        consumeHistoryMetadata.HistoryId = consumeHistory.Id;
                        consumeHistoryMetadata.Key = "SYS.Transaction";
                        consumeHistoryMetadata.Value = transactionId.ToString();
                        consumeHistoryMetadata.InsertUser = "admin";
                        consumeHistoryMetadata.InsertDate = DateTime.Now;
                        consumeHistoryMetadata.UpdateUser = "admin";
                        consumeHistoryMetadata.UpdateDate = DateTime.Now;
                        consumeHistoryMetadatas.Add(consumeHistoryMetadata);
                    }
                }
            }
            _context.ConsumeHistoryMetadata.AddRange(consumeHistoryMetadatas);

            _context.SaveChanges();
        }

        /// <summary>
        /// list去重
        /// </summary>
        public class Compare : IEqualityComparer<FbItem>
        {
            public bool Equals(FbItem x, FbItem y)
            {
                return x.Code == y.Code && x.HotelCode == y.HotelCode;//可以自定义去重规则，此处将code,hotelcode相同的就作为重复记录
            }
            public int GetHashCode(FbItem obj)
            {
                return obj.Code.GetHashCode();
            }
        }
    }
}
