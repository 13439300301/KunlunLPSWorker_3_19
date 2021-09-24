using Kunlun.LPS.Worker.Core.Common;

namespace Kunlun.LPS.Worker.Core.Domain
{
    /// <summary>
    /// 操作员元数据
    /// </summary>
    public class UserMetadata : BaseEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 操作员code
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
