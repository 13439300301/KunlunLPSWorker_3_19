using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.HRTProfile
{
    public interface IHRTProfileService
    {
        void UpdateLpsProfile(GetMemberResponse getMemberResponse);
    }
}
