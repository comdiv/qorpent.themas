using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comdiv.ThemaLoader.Test.Wrapping;
using Comdiv.ThemaLoader.UI;
using Comdiv.ThemaLoader.Wrap;
using NUnit.Framework;

namespace Comdiv.ThemaLoader.Test.UI
{
	[TestFixture]
	public class TreeBuildingTest:WTBaseTest
	{
		public TreeBuildingTest() {
			code = @"
thema g1, G1, isgroup
thema p1, P1
	link type=group target=g1
thema p2, P2 role = ADMIN
	link type=group target=g1
thema t3, T3 role=ADMIN
	link type=group target=g1
thema p1t1, P1T1
	link type=parent target=p1
thema p2t1, P2T1
	link type=parent target=p2
thema p1t2, P1T2 role = ADMIN
	link type=parent target=p1
";
		}

		[Test]
		public void admin_tree() {
			var result = load("test\\admin").Factory;
			var builder = result.GetUIBuilder();
			var tree = builder.BuildTree("test\\admin");
			var gr = tree.Groups.First(x => x.Code == "g1" && x.Name == "G1");
			var p1 = gr.Roots.First(x => x.Code == "p1" && x.Name == "P1");
			var p2 = gr.Roots.First(x => x.Code == "p2" && x.Name == "P2");
			var t3 = gr.Roots.First(x => x.Code == "t3" && x.Name == "T3");
			var p1t1 = p1.Terminals.First(x => x.Code == "p1t1" && x.Name == "P1T1");
			var p1t2 = p1.Terminals.First(x => x.Code == "p1t2" && x.Name == "P1T2");
			var p2t1 = p2.Terminals.First(x => x.Code == "p2t1" && x.Name == "P2T1");
		}

		[Test]
		public void strict_tree()
		{
			var result = load("test\\strict").Factory;
			var builder = result.GetUIBuilder();
			var tree = builder.BuildTree("test\\strict");
			var gr = tree.Groups.First(x => x.Code == "g1" && x.Name == "G1");
			var p1 = gr.Roots.First(x => x.Code == "p1" && x.Name == "P1");
			Assert.Null(gr.Roots.FirstOrDefault(x => x.Code == "p2" && x.Name == "P2"));
			Assert.Null(gr.Roots.FirstOrDefault(x => x.Code == "t3" && x.Name == "T3"));
			var p1t1 = p1.Terminals.First(x => x.Code == "p1t1" && x.Name == "P1T1");
			Assert.Null(p1.Terminals.FirstOrDefault(x => x.Code == "p1t2" && x.Name == "P1T2"));
			
		} 
	}
}
