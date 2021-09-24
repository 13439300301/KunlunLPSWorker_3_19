using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Enum
{
    [Flags]
    public enum RoomPointsRulesCalculationItem
    {
        None = 0,
        Room = 0x1,
        Fb = 0x02,
        Other = 0x04,
        Tax = 0x08
    }
}
