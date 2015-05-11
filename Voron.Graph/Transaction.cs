using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Concurrent;
using Voron.Trees;
using Voron.Impl;
using Voron.Graph.Indexing;
using Lucene.Net.Search;

namespace Voron.Graph
{
	/// <summary>
	/// Encapsulates Voron transaction and provides clean access to Voron.Graph trees. 
	/// There can be only one ReadWrite tx in any given time (enforced with semaphore)
	/// </summary>
	/// <remarks>
	/// might be counter-intuitive, but the transaction does not offer change tracking
	/// </remarks>
	public class Transaction : IDisposable
	{
		private bool _isDisposed;
		private readonly long _nodeCount;
		private readonly GraphStorage _storage;

		public bool IsDisposed
		{
			get
			{
				return _isDisposed;
			}
		}

		public TransactionFlags Flags
		{
			get
			{
				return VoronTransaction.Flags;
			}
		}

		public long NodeCount
		{
			get
			{
				return _nodeCount;
			}
		}

		private readonly IIndexWriter _indexWriter;
		private readonly IndexSearcher _indexSearcher;
		private readonly WriteBatch writeBatch;

		internal Voron.Impl.Transaction VoronTransaction { get; private set; }

		internal Transaction(Voron.Impl.Transaction voronTransaction, 
			string nodeTreeName, 
			string edgesTreeName, 
			string disconnectedNodesTreeName, 
			string keyByEtagTreeName, 
			string etagByKeyTreeName, 
			string graphMetadataKey, 
			long nodeCount,
			GraphStorage storage)
		{
			_isDisposed = false;
			if (voronTransaction == null)
				throw new ArgumentNullException("voronTransaction");

			_storage = storage;

			VoronTransaction = voronTransaction;
			_nodeCount = nodeCount;
			NodeTree = voronTransaction.ReadTree(nodeTreeName);
			EdgeTree = voronTransaction.ReadTree(edgesTreeName);
			DisconnectedNodeTree = voronTransaction.ReadTree(disconnectedNodesTreeName);
			KeyByEtagTree = voronTransaction.ReadTree(keyByEtagTreeName);
			SystemTree = voronTransaction.State.Root;
			GraphMetadataKey = graphMetadataKey;
			EtagByKeyTree = voronTransaction.ReadTree(etagByKeyTreeName);

			if (voronTransaction.Flags == TransactionFlags.ReadWrite)
			{
				_indexWriter = _storage.NewIndexWriter();
				writeBatch = new WriteBatch();
			}

			_indexSearcher = _storage.NewIndexSearcher();
        }

		internal string GraphMetadataKey { get; private set; }

		internal Tree SystemTree { get; private set; }

		internal Tree NodeTree { get; private set; }

		internal Tree EdgeTree { get; private set; }

		internal Tree DisconnectedNodeTree { get; private set; }

		internal Tree KeyByEtagTree { get; private set; }

		internal Tree EtagByKeyTree { get; private set; }

		public WriteBatch WriteBatch
		{
			get
			{
				return writeBatch;
			}
		}

		public IIndexWriter IndexWriter
		{
			get
			{
				return _indexWriter;
			}
		}

		public Searcher Searcher
		{
			get
			{
				return _indexSearcher;
			}
		}

		public void Dispose()
		{
			_isDisposed = true;
			VoronTransaction.Dispose();
			_indexSearcher.Dispose();
            if (_indexWriter != null)
				_indexWriter.Dispose();
		}

		public void Rollback()
		{
			VoronTransaction.Rollback();
			if (_indexWriter != null)
				_indexWriter.Rollback();
        }

		public void Commit()
		{
			VoronTransaction.Commit();
			if (VoronTransaction.Flags == TransactionFlags.ReadWrite)
			{
				_indexWriter.Commit();
				_storage.Write(writeBatch);
            }
		}

		private string _nodeTreeName;
		private string _edgeTreeName;
		private string _disconnectedNodesTreeName;
		private string _keyByEtagTreeName;
		private string _etagByKeyTreeName;
		private string _graphMetadataKey;
		private long _nextId;
		private GraphStorage graphStorage;

    }
}
