using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.StoredValue
{
    public interface IUpdateDbStoredValueService
    {
        void TestDb();

        void TestDbTiming(int t);
    }
}
