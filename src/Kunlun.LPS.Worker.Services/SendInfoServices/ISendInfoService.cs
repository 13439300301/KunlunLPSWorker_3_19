using Kunlun.LPS.Worker.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.SendInfoServices
{
    public interface ISendInfoService
    {
        void Insert(SendInfo entity);

        bool SenfInfoByShown();
    }
}
