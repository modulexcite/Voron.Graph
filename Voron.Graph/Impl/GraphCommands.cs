using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voron.Graph.Extensions;

namespace Voron.Graph.Impl
{
	public class GraphCommands
	{
		private readonly GraphQueries _queries;
		private readonly Conventions _conventions;

		public GraphCommands(GraphQueries Queries, Conventions Conventions)
		{
			this._queries = Queries;
			this._conventions = Conventions;
		}

		internal void CreateNode<T>(Transaction tx, T value)
			where T : class, new()
		{
			if (tx == null) throw new ArgumentNullException("tx");
			if (value == null) throw new ArgumentNullException("value");

			var key = _conventions.GetNextNodeKey();

			var nodeKey = key.ToSlice();
			var etag = Etag.Generate();

			var serializedValue = JObject.FromObject(value);

			tx.NodeTree.Add(nodeKey, serializedValue.ToStream());
			tx.KeyByEtagTree.Add(etag.ToSlice(), nodeKey);
			tx.DisconnectedNodeTree.Add(nodeKey, serializedValue.ToStream());

			tx.Index<T>().Add(value);
		}
	}
}
