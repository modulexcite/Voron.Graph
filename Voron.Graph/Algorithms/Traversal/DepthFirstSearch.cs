﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Voron.Graph.Algorithms.Search
{
    public class DepthFirstSearch : BaseRootedAlgorithm, ISearchAlgorithm
    {

        private readonly GraphStorage _graphStorage;

        public DepthFirstSearch(GraphStorage graphStorage, CancellationToken cancelToken)
            : base(cancelToken)
        {
            _graphStorage = graphStorage;
        }

        protected override Node GetDefaultRootNode(Transaction tx)
        {
            using (var iter = tx.NodeTree.Iterate())
            {
                if (!iter.Seek(Slice.BeforeAllKeys))
                    return null;

                using (var resultStream = iter.CreateReaderForCurrent().AsStream())
                {
                    Etag etag;
                    JObject value;
                    Util.EtagAndValueFromStream(resultStream, out etag, out value);
                    return new Node(iter.CurrentKey.CreateReader().ReadBigEndianInt64(), value, etag);
                }
            }
        }

        public Task Traverse(Transaction tx,
            Func<JObject, bool> searchPredicate,
            Func<bool> shouldStopPredicate,
            Node rootNode = null,
            ushort? edgeTypeFilter = null,
            uint? traverseDepthLimit = null)
        {
	        if (State == AlgorithmState.Running)
                throw new InvalidOperationException("The search already running");
	        
			return Task.Run(() =>
	        {
		        OnStateChange(AlgorithmState.Running);

		        rootNode = rootNode ?? GetDefaultRootNode(tx);
		        var visitedNodes = new HashSet<long>();
		        var processingQueue = new Stack<NodeVisitedEventArgs>();
		        processingQueue.Push(new NodeVisitedEventArgs(rootNode,null,1,0));

		        while (processingQueue.Count > 0)
		        {
			        if (shouldStopPredicate())
			        {
				        OnStateChange(AlgorithmState.Finished);
				        break;
			        }

			        AbortExecutionIfNeeded();

			        var currentVisitedEventInfo = processingQueue.Pop();
			        if (searchPredicate(currentVisitedEventInfo.VisitedNode.Data))
			        {
				        OnNodeFound(currentVisitedEventInfo.VisitedNode);
                        if (shouldStopPredicate() || 
                            (traverseDepthLimit.HasValue && currentVisitedEventInfo.TraversedEdgeCount >= traverseDepthLimit.Value))
				        {
					        OnStateChange(AlgorithmState.Finished);
					        break;
				        }
			        }

			        if(!visitedNodes.Contains(currentVisitedEventInfo.VisitedNode.Key))
			        {
				        visitedNodes.Add(currentVisitedEventInfo.VisitedNode.Key);
				        OnNodeVisited(currentVisitedEventInfo);

				        foreach (var childNodeWithWeight in 
                            _graphStorage.Queries.GetAdjacentOf(tx, currentVisitedEventInfo.VisitedNode, edgeTypeFilter ?? 0)
					            .Where(nodeWithWeight => !visitedNodes.Contains(nodeWithWeight.Node.Key)))
				        {
					        AbortExecutionIfNeeded();
                            processingQueue.Push(new NodeVisitedEventArgs(childNodeWithWeight.Node,
                                currentVisitedEventInfo.VisitedNode, 
                                currentVisitedEventInfo.TraversedEdgeCount + 1,
                                childNodeWithWeight.WeightOfEdgeToNode));
				        }
			        }
		        }
		        OnStateChange(AlgorithmState.Finished);
	        });
        }

        public event Action<NodeVisitedEventArgs> NodeVisited;

        protected void OnNodeVisited(NodeVisitedEventArgs node)
        {
            var nodeVisited = NodeVisited;
            if (nodeVisited != null)
                nodeVisited(node);
        }

        public event Action<Node> NodeFound;

        protected void OnNodeFound(Node node)
        {
            var nodeFound = NodeFound;
            if (nodeFound != null)
                nodeFound(node);
        }
    }
}
