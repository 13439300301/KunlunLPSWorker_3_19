using System.Diagnostics;
using System.Reflection;

namespace Kunlun.LPS.Worker.Core
{
    public static class LPSHelper
    {
        public static string GetProductVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            return fileVersionInfo.ProductVersion;
        }
    }
}
