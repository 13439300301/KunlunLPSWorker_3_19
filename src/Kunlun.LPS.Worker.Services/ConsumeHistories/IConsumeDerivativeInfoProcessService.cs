using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.ConsumeHistories
{
    public interface IConsumeDerivativeInfoProcessService
    {
        void ConsumeDerivativeInfoProcess(long historyId);
    }
}
