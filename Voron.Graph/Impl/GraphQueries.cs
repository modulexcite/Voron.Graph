using System;
using System.Linq;
using System.Collections.Generic;
using Lucene.Net.Index;
using Voron.Graph.Extensions;
using Lucene.Net.Search;
using System.Linq.Expressions;

namespace Voron.Graph.Impl
{
	public class GraphQueries
	{
		private GraphStorage _storage;

		public GraphQueries(GraphStorage graphStorage)
		{
			_storage = graphStorage;
		}

		public bool ContainsNode(Transaction tx, long key)
		{
			if (tx == null) throw new ArgumentNullException("tx");

			return tx.NodeTree.ReadVersion(key.ToSlice()) > 0;
		}

		public IEnumerable<T> Search<T>(Transaction tx, Expression<Func<T,bool>> predicate)
			where T : class
		{
		    throw new NotImplementedException();
		}
	}
}
