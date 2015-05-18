using System.Linq;
using Voron.Graph.Indexing;
using FluentAssertions;
using Xunit;

namespace Voron.Graph.Tests
{
	public class IndexingTests : BaseGraphTest
	{
		public class FooBar
		{
			public string Foo { get; set; }
			public string Bar { get; set; }
		}

		[Fact]
		public void Can_initialize()
		{
			this.Invoking(_ =>
			{
				using (var index = new Index("TestIndex", true))
				{
				}
			}).ShouldNotThrow();
		}
	
		[Fact]
		public void Can_index_graph_nodes()
		{
			var graph = new GraphStorage("TestGraph", Env);
			var node1Data = new FooBar
			{
				Foo = "Nya",
				Bar = "Foo1"
			};

			var node2Data = new FooBar
			{
				Foo = "Bar2",
				Bar = "Foo2"
			};

			var node3Data = new FooBar
			{
				Foo = "Bar3",
				Bar = "Foo3"
			};

			using (var tx = graph.NewTransaction(TransactionFlags.ReadWrite))
			{
				graph.Commands.CreateNode(tx, node1Data);
				graph.Commands.CreateNode(tx, node2Data);
				graph.Commands.CreateNode(tx, node3Data);

				tx.Commit();
			}

			using (var tx = graph.NewTransaction(TransactionFlags.ReadWrite))
			{
				var queryResults = graph.Queries.Search<FooBar>(tx, doc => doc.Foo == "Bar2");
				var expectedResults = new[] { node2Data, node3Data };
				queryResults.ShouldBeEquivalentTo(expectedResults);
			}
		}

		//[Fact]
		//public void Uncomitted_nodes_wont_appear_in_query()
		//{
		//	var graph = new GraphStorage("TestGraph", Env);
		//	var node1Data = new FooBar
		//	{
		//		Foo = "Bar1",
		//		Bar = "Foo1"
		//	};

		//	var node2Data = new FooBar
		//	{
		//		Foo = "Bar2",
		//		Bar = "Foo2"
		//	};

		//	using (var tx = graph.NewTransaction(TransactionFlags.ReadWrite))
		//	{
		//		graph.Commands.CreateNode(tx, node1Data);
		//		tx.Commit();
		//	}

		//	using (var tx = graph.NewTransaction(TransactionFlags.ReadWrite))
		//	{
		//		graph.Commands.CreateNode(tx, node2Data);

		//		var queryResults1 = graph.Queries.IndexQuery<FooBar>().Where(n => n.Foo.StartsWith("Bar"));
		//		var expectedResults1 = new[] { node1Data }; //node2Data is uncomitted so won't appear in query
		//		queryResults1.ShouldBeEquivalentTo(expectedResults1);

		//		tx.Commit();
		//	}

		//	var queryResults = graph.Queries.IndexQuery<FooBar>().Where(n => n.Foo.StartsWith("Bar"));
		//	var expectedResults = new[] { node1Data, node2Data };
		//	queryResults.ShouldBeEquivalentTo(expectedResults);
		//}
	}
}
