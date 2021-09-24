using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.RegularValueStorageRule
{
    public class RegularValueStorageRulesConfig
    {
        public storedfrequency storedfrequency { get; set; }

        public eliminatefrequency eliminatefrequency { get; set; }
    }
    public class storedfrequency
    {
        public string type { get; set; }

        public int value { get; set; }
    }

    public class eliminatefrequency
    {
        public string type { get; set; }

        public int value { get; set; }
    }
}
