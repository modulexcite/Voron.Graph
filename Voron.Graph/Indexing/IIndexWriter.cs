using Lucene.Net.Documents;
using Lucene.Net.Search;
using System;

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
