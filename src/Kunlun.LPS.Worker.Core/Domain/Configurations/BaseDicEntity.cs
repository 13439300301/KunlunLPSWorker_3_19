using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public abstract class BaseDicEntity : BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        private int? _seq;
        public int? Seq
        {
            get
            {
                return _seq;
            }
            set
            {
                _seq = value ?? 0;
            }
        }
    }
}
