using Kunlun.LPS.Worker.Core.Domain.Common;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class CouponInventory : BaseVersionedEntity
    {
        public long Id { get; set; }

        public long CouponTypeId { get; set; }

        public int Inventory { get; set; }

        public virtual CouponType CouponType { get; set; }
    }
}
