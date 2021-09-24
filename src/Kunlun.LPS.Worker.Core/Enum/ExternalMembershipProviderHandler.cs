using System.Runtime.Serialization;

namespace Kunlun.LPS.Worker.Core.Enum
{
    public enum ExternalMembershipProviderHandler
    {
        OpenId,
        [EnumMember(Value = "ffp")]
        FFP
    }
}
