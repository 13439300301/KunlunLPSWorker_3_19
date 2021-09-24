using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Enum
{
    [Flags]
    public enum CouponExchangeMode
    {
        None = 0,
        Points = 1 << 0,
        CouponPackage = 1 << 1,
        Privilege = 1 << 2,
        Gift = 1 << 3,
        Sell = 1 << 4
    }
}
