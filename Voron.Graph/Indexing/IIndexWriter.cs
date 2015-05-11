using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voron.Graph.Indexing
{
	public interface IIndexWriter : IDisposable
	{
		void AddDocument(Document doc);
		void Add<T>(T obj);
		void DeleteDocuments<T>(Query selection);		
		void Update<T>(T obj, Query selection);
		void Commit();
		void Rollback();
	}
}
