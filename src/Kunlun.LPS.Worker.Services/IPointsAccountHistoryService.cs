﻿using Kunlun.LPS.Worker.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services
{
    public interface IPointsAccountHistoryService
    {
        PointsAccountHistory GetById(long id);
    }
}
