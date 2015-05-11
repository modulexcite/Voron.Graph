using System;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.Index;

namespace Voron.Graph.Indexing
{
	internal class LuceneWriter : IIndexWriter
	{
		private readonly IndexWriter _writer;

		public LuceneWriter(IndexWriter writer)
		{
			_writer = writer;
		}

		public void Add<T>(T item)
		{
			_writer.Add<T>(item);
		}

		public void AddDocument(Document doc)
		{
			_writer.AddDocument(doc);
		}

		public void Commit()
		{
			_writer.Flush(false, false, true);
			_writer.Commit();
		}

		public void DeleteDocuments<T>(Query selection)
		{
			_writer.DeleteDocuments<T>(selection);
		}

		public void Rollback()
		{
			_writer.Rollback();
		}

		public void Update<T>(T item, Query selection)
		{
			_writer.Update<T>(item, selection);
		}

		#region IDisposable Implementation
		private bool isDisposed = false; // To detect redundant calls

		protected void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				_writer.Dispose(disposing);
				isDisposed = true;
			}
		}

		~LuceneWriter()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
