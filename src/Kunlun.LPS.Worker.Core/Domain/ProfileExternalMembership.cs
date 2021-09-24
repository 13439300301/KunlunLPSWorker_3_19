using Kunlun.LPS.Worker.Core.Common;
using Kunlun.LPS.Worker.Core.Enum;
using System;

namespace Kunlun.LPS.Worker.Core.Domain
{
    public class ProfileExternalMembership : BaseEntity
    {
        public long Id { get; set; }

        public long ProfileId { get; set; }

        public long ProviderId { get; set; }

        public string ProviderKey { get; set; }

        public long? LevelId { get; set; }

        public decimal? Mileage { get; set; }

        public ExternalMembershipProviderHandler ProviderType { get; set; }

        public DateTime? Expires { get; set; }

        public bool IsValid { get; set; }
    }
}
