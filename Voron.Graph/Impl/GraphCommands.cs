using Lucene.Net.Documents;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Voron.Graph.Extensions;

namespace Voron.Graph.Impl
{
	public class GraphCommands
	{
		private readonly GraphQueries _queries;
		private readonly Conventions _conventions;

		public GraphCommands(GraphQueries Queries, Conventions Conventions)
		{
			_queries = Queries;
			_conventions = Conventions;
		}

		public long CreateNode<T>(Transaction tx, T value)
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
			tx.EtagByKeyTree.Add(nodeKey, etag.ToStream());
			tx.DisconnectedNodeTree.Add(nodeKey, serializedValue.ToStream());

			var indexDocument = value.ToDocument();
			var idField = new NumericField(Constants.NodeIdFieldName);
			idField.SetLongValue(key);
			indexDocument.Add(idField);

			tx.IndexWriter.AddDocument(indexDocument);

			return key;
		}

		//public bool TryUpdate<T>(Transaction tx, long key, T newValue)
		//	where T : class, new()
		//{
		//	if (tx == null) throw new ArgumentNullException("tx");

		//	if (!_queries.ContainsNode(tx, key))
		//		return false;

		//	//Voron's method name here is misleading --> it performs updates as well
		//	var etagReadResult = tx.EtagByKeyTree.Read(key.ToSlice());
		//	etagReadResult.Reader.
		//	tx.KeyByEtagTree.Delete(node.Etag.ToSlice());
		//	var newEtag = Etag.Generate();
		//	node.Etag = newEtag;

		//	tx.NodeTree.Add(node.Key.ToSlice(), Util.EtagAndValueToStream(newEtag, node.Data));

		//	tx.KeyByEtagTree.Add(newEtag.ToSlice(), node.Key.ToSlice());
		//	return true;
		//}
	}
}
