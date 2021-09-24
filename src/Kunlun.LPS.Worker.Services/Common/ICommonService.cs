using Kunlun.LPS.Worker.Core.Domain;
using Kunlun.LPS.Worker.Core.Enum;
using System.Collections.Generic;

namespace Kunlun.LPS.Worker.Services.Common
{
    public interface ICommonService
    {
        string GetDefaultPlaceCode();
        string GetDefaultPlaceCode(string HotelCode);

        string GetGroupHotelCode();

        string GetHotelCode(string placeCode);

        public List<Coupon> GetCoupon(long id, CouponCategory type, int totalCount, string prefix);
    }
}
