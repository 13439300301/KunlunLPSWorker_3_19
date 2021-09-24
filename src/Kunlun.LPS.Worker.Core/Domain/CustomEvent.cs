using Kunlun.LPS.Worker.Core.Domain.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class CustomEvent : BaseEntity
    {
        public int Id { get; set; }

        public string EventCode { get; set; }

        public string Description { get; set; }

        public int TemplateId { get; set; }

        public int StepId { get; set; }

        public string LanguageCode { get; set; }

        public virtual ICollection<CustomEventDetail> CustomEventDetail { get; set; }
    }
}
