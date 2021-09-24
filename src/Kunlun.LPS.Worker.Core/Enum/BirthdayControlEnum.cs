using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Enum
{
    [Flags]
    public enum BirthdayControlEnum
    {
        None = 0,
        Day = 1,
        Week = 2,
        Month = 4
    }
}
