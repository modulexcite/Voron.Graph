using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voron.Graph.Impl
{
	public class GraphQueries
	{
		private GraphStorage _storage;

		public GraphQueries(GraphStorage graphStorage)
		{
			_storage = graphStorage;
		}

		public IQueryable<T> IndexQuery<T>()
			where T : class, new()
		{
			return _storage.GetQuery<T>();
		}
	}
}
