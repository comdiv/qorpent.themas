using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Comdiv.ThemaLoader.Test.Loading
{
	[TestFixture]
	public class BasicLoadingTest {
		private string code = @"
thema l
	out l1.out
thema A,A, isgroup, role = X;Y
thema B,B, role = X
	in A.in, role = X_OP
	link type=group target=A
thema C,C, role = Y
	link type=parent target=B
	in A.in ,role = Y_OP
	in B.in, role = Y_OP
	out A.out, role = Y
		uselib l.l1.out
		col X
	out B.out, role = Y
extra
	hello world
";
		[Test]
		public void back_links_seted_up() {
			var result = load();
			var item = result.Themas.GetReport("C.A.out");
			var col = item.GetColumns().First();
			Assert.NotNull(col.ContainingItem);
			Assert.NotNull(col.ContainingItem.Thema);
		}
		[Test]
		public void one_element_loaded()
		{
			var result = load();
			var item = result.Themas.GetReport("C.A.out");
			Assert.AreEqual(1,item.GetElements<IThemaItemElement>().Count());
		}

		[Test]
		public void themas_parsed() {
			var result = load();
			Assert.AreEqual(4,result.Themas.Index.Count);
			Assert.True(result.Themas.Index.ContainsKey("A"));
			Assert.True(result.Themas.Index.ContainsKey("B"));
			Assert.True(result.Themas.Index.ContainsKey("C"));
			Assert.True(result.Themas["A"].IsGroup);
			Assert.AreEqual("X;Y",result.Themas["A"].Role);
			Assert.AreEqual("X;Y", result.Themas["A"].GetParam("role",""));
		}

		[Test]
		public void group_and_parent_resolved()
		{
			var result = load();
			Assert.AreEqual(result.Themas["A"], result.Themas["B"].Group);
			Assert.AreEqual(result.Themas["B"], result.Themas["C"].Parent);
			Assert.NotNull(result.Themas["A"].InLinks.FirstOrDefault(x=>x.Source==result.Themas["B"]));
			Assert.NotNull(result.Themas["B"].InLinks.FirstOrDefault(x => x.Source == result.Themas["C"]));
			Assert.NotNull(result.Themas["B"].OutLinks.FirstOrDefault(x => x.Target == result.Themas["A"]));
			Assert.NotNull(result.Themas["C"].OutLinks.FirstOrDefault(x => x.Target == result.Themas["B"]));

		}

		[Test]
		public void items_loaded()
		{
			var result = load();
			Assert.NotNull(result.Themas["B"].GetForm("A"));
			Assert.NotNull(result.Themas["C"].GetForm("A"));
			Assert.NotNull(result.Themas["C"].GetForm("B"));
			Assert.NotNull(result.Themas["C"].GetReport("A"));
			Assert.NotNull(result.Themas["C"].GetReport("B"));


		}


		[Test]
		public void items_can_be_searched_through_factory()
		{
			var result = load();
			Assert.NotNull(result.Themas.GetForm("B.A"));
			Assert.NotNull(result.Themas.GetForm("C.A"));
			Assert.NotNull(result.Themas.GetForm("C.B"));
			Assert.NotNull(result.Themas.GetReport("C.A"));
			Assert.NotNull(result.Themas.GetReport("C.B"));
			Assert.AreEqual(3,result.Themas.SearchItems("*.*.in").Count());
			Assert.AreEqual(1, result.Themas.SearchItems("B.*.*").Count());
			Assert.AreEqual(4, result.Themas.SearchItems("C.*.*").Count());
			Assert.AreEqual(3, result.Themas.SearchItems("*.A.*").Count());
			Assert.AreEqual(2, result.Themas.SearchItems("*.B.*").Count());
			


		}


		[Test]
		public void extra_loaded()
		{
			var result = load();
			Console.WriteLine(result.ExtraData);
			Assert.AreEqual(@"<extra>
  <hello _file=""direct"" _line=""17"" code=""world"" id=""world"" />
</extra>", result.ExtraData.ToString());

		}

		private IThemaFactory load() {
			return new ThemaFactory().Load(new BxlThemaSource(code));
		}
	}
}
