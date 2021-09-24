using Kunlun.LPS.Worker.Core.Common;

namespace Kunlun.LPS.Worker.Core.Domain.Common
{
    /// <summary>
    /// 带有乐观并发控制的实体类
    /// </summary>
    public class BaseVersionedEntity : BaseEntity
    {
        public byte[] Version { get; set; }
    }
}