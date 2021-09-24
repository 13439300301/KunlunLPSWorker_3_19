using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Enum
{
    public enum CalculationItem
    {
        None = 0,
        Food = 1,
        Drinks = 2,
        Other = 4,
        ServiceCharge = 8,
        Taxes = 16
    }
}
