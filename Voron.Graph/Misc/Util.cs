using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Voron.Util.Conversion;

namespace Voron.Graph
{
    internal static class Util
    {
		private static readonly int SizeOfUShort = Marshal.SizeOf(typeof(ushort));
		private static readonly int SizeOfInt = Marshal.SizeOf(typeof(int));
		private static readonly int SizeOfLong = Marshal.SizeOf(typeof(long));

    }
}
