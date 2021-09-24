using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class LocaleStringResource : ConfigurationBase
    {
        public int Id { get; set; }

        public string LanguageId { get; set; }
    }
}
