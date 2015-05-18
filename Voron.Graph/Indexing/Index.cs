using System;
using System.IO;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Search;

namespace Voron.Graph.Indexing
{
	public class Index : IDisposable
	{
		private readonly Lucene.Net.Store.Directory _indexDirectory;
		private Analyzer _analyzer;

		internal Lucene.Net.Store.Directory Directory
		{
			get
			{
				return _indexDirectory;
			}
		}

	    public Index(string indexPath,bool runInMemory = false, Analyzer analyzer = null)
		{
		    _indexDirectory = runInMemory ? (Lucene.Net.Store.Directory)(new RAMDirectory()) :
											(Lucene.Net.Store.Directory)(new MMapDirectory(new DirectoryInfo(indexPath)));
			_analyzer = analyzer ?? new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
		}

		public IIndexWriter OpenWriter()
		{
			return new LuceneWriter(CreateWriter());
		}

		public IndexSearcher OpenSearcher()
		{
			return new IndexSearcher(_indexDirectory);
		}

		private IndexWriter CreateWriter()
		{
			return new IndexWriter(_indexDirectory, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
		}

// ReSharper disable once UnusedParameter.Local
		private void Dispose(bool isDisposing)
		{
			try
			{
				using (var writer = CreateWriter())
				{
					writer.Flush(true, true, true);
					writer.Optimize(true);
				}
			}
			finally
			{
				_indexDirectory.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Index()
		{
			Dispose(false);
		}		
	}
}
