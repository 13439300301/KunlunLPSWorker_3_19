using Kunlun.LPS.Worker.Core.Domain.Configurations;
using Kunlun.LPS.Worker.Services.Configurations;
using System.Linq;
using EFCore.BulkExtensions;
using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using Kunlun.LPS.Worker.Core.MessageQueue;
using Kunlun.LPS.Worker.Core.MessageQueue.MessageEntity;
using Kunlun.LPS.Worker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Kunlun.LPS.Worker.Services.Common
{
    public class CommonService : ICommonService
    {
        private readonly IConfigurationService<Sysparam> _sysparamService;
        private readonly IConfigurationService<Place> _placeService;
        private readonly IConfigurationService<PlaceM> _placeMService;
        private readonly IConfigurationService<Hotel> _hotelService;
        private readonly IUniqueIdGeneratorService _uniqueIdGeneratorService;
        private readonly LPSWorkerContext _context;
        private readonly IMessageQueueProducer _messageQueueProducer;
        private readonly ILogger<CommonService> _logger;

        public CommonService(
            IConfigurationService<Sysparam> sysparamService,
            IConfigurationService<Place> placeService,
            IConfigurationService<PlaceM> placeMService,
            IConfigurationService<Hotel> hotelService,
            IUniqueIdGeneratorService uniqueIdGeneratorService,
            LPSWorkerContext context,
            IMessageQueueProducer messageQueueProducer,
            ILogger<CommonService> logger
            )
        {
            _sysparamService = sysparamService;
            _placeService = placeService;
            _placeMService = placeMService;
            _hotelService = hotelService;
            _uniqueIdGeneratorService = uniqueIdGeneratorService;
            _context = context;
            _messageQueueProducer = messageQueueProducer;
            _logger = logger;
        }

        public string GetDefaultPlaceCode()
        {
            var res = _sysparamService.GetAllFromCache().FirstOrDefault(c => c.Code == "DFT_POINT_PLACE");
            if (res==null)//缓存中没有，查库
            {
                res =_context.Sysparam.FirstOrDefault(c => c.Code == "DFT_POINT_PLACE");
            }
            return res?.ParValue;
        }

        public string GetDefaultPlaceCode(string HotelCode)
        {
            var res = _sysparamService.GetAllFromCache().FirstOrDefault(c => c.Code == "DFT_POINT_PLACE" && c.HotelCode == HotelCode);
            if (res==null)//缓存中没有，查库
            {
                res=_context.Sysparam.FirstOrDefault(c => c.Code == "DFT_POINT_PLACE" && c.HotelCode == HotelCode);
            }
            return res?.ParValue;
        }
        public string GetGroupHotelCode()
        {
            var res = _hotelService.GetAllFromCache().FirstOrDefault(c => c.Flag == "1");
            if (res==null)
            {
                res=_context.Hotel.FirstOrDefault(c => c.Flag == "1");
            }
            return res?.Code;
        }

        public string GetHotelCode(string placeCode)
        {
            var place = _placeService.GetAllFromCache().FirstOrDefault(o => o.Code == placeCode);
            if (place==null)
            {
                place =_context.Place.FirstOrDefault(o => o.Code == placeCode);
            }
            var res = _placeMService.GetAllFromCache().FirstOrDefault(o => o.Code == place?.MCode);
            if (res==null)
            {
                res =_context.PlaceM.FirstOrDefault(o => o.Code == place.MCode);
            }
            return res?.HotelCode;
        }

        public List<Coupon> GetCoupon(long id, CouponCategory type, int totalCount, string prefix)
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
                list.Add(coupon);
            }
            return list;
        }

        private string GetCouponNumber(Random rd)
        {
            var number = GetSerialNoLen13();
            return number;
        }

        private string GetSerialNoLen13()
        {
            //13位日期+1到8位随机数，13个字符
            string timestamp = ToUnixTime(DateTime.Now).ToString();
            string strRandomResult = NextRandom(999999, 6).ToString();
            return timestamp + strRandomResult;
        }

        /// <summary>
        /// 当前时间转换成GMT时间戳
        /// </summary>
        /// <param name="time">当前时间</param>
        /// <returns>秒</returns>
        private long ToUnixTime(DateTime time)
        {
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, 0);
            return (long)(time.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
        }
        /// <summary>
        ///     参考：msdn上的RNGCryptoServiceProvider例子
        /// </summary>
        /// <param name="numSeeds"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static int NextRandom(int numSeeds, int length)
        {
            var randomNumber = new byte[length];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomNumber);
            uint randomResult = 0x0;
            for (var i = 0; i < length; i++) randomResult |= (uint)randomNumber[i] << ((length - 1 - i) * 8);
            return (int)(randomResult % numSeeds) + 1;
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
    }
}
