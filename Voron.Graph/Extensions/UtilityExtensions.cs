using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Voron.Graph.Indexing;
using Voron.Util.Conversion;

namespace Voron.Graph.Extensions
{
    public static class UtilityExtensions
    {
        private static readonly int SizeOfUShort = Marshal.SizeOf(typeof(ushort));
        private static readonly int SizeOfShort = Marshal.SizeOf(typeof(short));
        private static readonly int SizeOfInt = Marshal.SizeOf(typeof(int));
        private static readonly int SizeOfLong = Marshal.SizeOf(typeof(long));

		
        internal static Stream ToStream(this JObject jsonObject)
        {
            if (jsonObject == null)
                return Stream.Null;

            var stream = new MemoryStream();
            var writer = new BsonWriter(stream);
            jsonObject.WriteTo(writer);

            stream.Position = 0;
            return stream;
        }

        internal static JObject ToJObject(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanRead || !stream.CanSeek)
                throw new ArgumentException("cannot deserialize unreadable stream");

            stream.Seek(0, SeekOrigin.Begin);

            var reader = new BsonReader(stream);
            return JObject.Load(reader);
        }

        internal static Slice ToSlice(this long key)
        {
            var buffer = new byte[SizeOfLong]; //TODO: refactor this with BufferPool implementation
            EndianBitConverter.Big.CopyBytes(key, buffer, 0);
            return new Slice(buffer);
        }

        internal static Slice ToSlice(this int key)
        {
            var buffer = new byte[SizeOfInt]; //TODO: refactor this with BufferPool implementation
            EndianBitConverter.Big.CopyBytes(key, buffer, 0);
            return new Slice(buffer);
        }

   

	    internal static long ToNodeKey(this Slice key)
	    {
		    var keyData = new byte[SizeOfLong];
		    key.CopyTo(keyData);

		    return EndianBitConverter.Big.ToInt64(keyData, 0);
	    }

      

    }
}
