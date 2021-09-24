using Kunlun.LPS.Worker.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.DivisionCards
{
    public class DivisionCard : BaseEntity
    {
        public long Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int CardNumber { get; set; }

        public decimal Balance { get; set; }

        public bool IsValid { get; set; }

        public long LeaderCardId { get; set; }

        public decimal OverdraftBalance { get; set; }
    }
}
