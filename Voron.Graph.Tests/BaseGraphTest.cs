﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Voron.Graph.Tests
{
    public class BaseGraphTest : IDisposable
    {
        public StorageEnvironment Env;
        public ConcurrentQueue<IDisposable> DisposalQueue;

        public StorageEnvironmentOptions StorageOptions;

		public BaseGraphTest()
        {
            Env = new StorageEnvironment(StorageOptions ?? StorageEnvironmentOptions.CreateMemoryOnly());
            DisposalQueue = new ConcurrentQueue<IDisposable>();
        }

        public void Dispose()
        {
            Env.Dispose();
            foreach (var disposable in DisposalQueue.Where(x => x != null))
                disposable.Dispose();
        }

        protected Stream StreamFrom(string s)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
            //just to be sure
            DisposalQueue.Enqueue(stream);

            return stream;
        }

        protected string StringFrom(Stream stream)
        {
            var buffer = new byte[128];
            var readBytes = new List<byte>();
            while (stream.Read(buffer, 0, 128) > 0)
                readBytes.AddRange(buffer.Where(x => x != 0));

            DisposalQueue.Enqueue(stream);
            return Encoding.UTF8.GetString(readBytes.ToArray());
        }

        protected JObject JsonFromValue<T>(T value)
        {
            return JObject.FromObject(new { Value = value });
        }

        protected T ValueFromJson<T>(JObject @object)
        {
            return @object.Value<T>("Value");
        }
    }
}
