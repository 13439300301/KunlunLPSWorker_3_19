using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services
{
    public interface IConsumptionTimesReminderService
    {
        void ConsumptionTimesReminder(long membershipCardId);
    }
}
