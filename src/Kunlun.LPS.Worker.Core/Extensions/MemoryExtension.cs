using System;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Extensions
{
    public static class MemoryExtension
    {
        public static string GetString(this ReadOnlyMemory<byte> memory)
        {
            return Encoding.UTF8.GetString(memory.ToArray());
        }
    }
}
