#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : ProcessReadingTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Tests.EcoProcess {
	[TestFixture]
	public class ProcessReadingTest {
		[Test]
		public void Prevents_double_coded_processes() {
			var ctx =
				EPTest.Compile(@"
orgnode A
	orgnode B
process Ax, orgnode =A
process Bx, orgnode =B
process Bx, orgnode=B
");
			Assert.False(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "ER_EPI03"));
		}

		[Test]
		public void Prevents_not_coded_processes() {
			var ctx =
				EPTest.Compile(@"
orgnode A
	orgnode B
process Ax, orgnode =A
process Bx, orgnode =B
process orgnode=B
");
			Assert.False(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "ER_EPI03"));
		}

		[Test]
		public void Prevents_not_orgnoded_processes() {
			var ctx = EPTest.Compile(@"
orgnode A
	orgnode B
process Ax, orgnode =A
process Bx, orgnode =B
process Cx
");
			Assert.False(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "ER_EPI03"));
		}

		[Test]
		public void can_read_embeded_to_org_node() {
			var ctx = EPTest.Compile(@"
orgnode A, An
	process Ax, An
	orgnode B
		process Bx
		process Cx
");
			Assert.AreEqual(3, ctx.EcoProcessIndex.Index.Count);
			Assert.AreEqual("A", ctx.EcoProcessIndex["Ax"].OrgNodeCode);
			Assert.AreEqual("B", ctx.EcoProcessIndex["Bx"].OrgNodeCode);
			Assert.AreEqual("B", ctx.EcoProcessIndex["Cx"].OrgNodeCode);
			Assert.AreEqual("B", ctx.EcoProcessIndex["Cx"].OrgNodeCode);
			Assert.AreEqual("An", ctx.EcoProcessIndex["Ax"].Name);
			Assert.AreEqual(1, ctx.OrgNodeIndex.Roots.Count());
			Assert.AreEqual("A", ctx.OrgNodeIndex.Roots.First().Code);
			Assert.AreEqual("An", ctx.OrgNodeIndex.Roots.First().Name);
		}

		[Test]
		public void can_read_outer_to_org_node() {
			var ctx =
				EPTest.Compile(@"
orgnode A
	orgnode B
process Ax, orgnode =A
process Bx, orgnode =B
process Cx, orgnode =B
");
			Assert.AreEqual(3, ctx.EcoProcessIndex.Index.Count);
			Assert.AreEqual("A", ctx.EcoProcessIndex["Ax"].OrgNodeCode);
			Assert.AreEqual("B", ctx.EcoProcessIndex["Bx"].OrgNodeCode);
			Assert.AreEqual("B", ctx.EcoProcessIndex["Cx"].OrgNodeCode);
		}


		[Test]
		public void prevent_miss_coded_process_in() {
			var ctx = EPTest.Compile(@"
orgnode A
	process Ax
	orgnode B
		process Bx
			in Axxx
		process Cx
			in Bx
");

			Assert.False(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "ER_EPI04"));
		}

		[Test]
		public void prevent_not_coded_process_in() {
			var ctx = EPTest.Compile(@"
orgnode A
	process Ax
	orgnode B
		process Bx
			in
		process Cx
			in Bx
");

			Assert.False(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "ER_EPI03"));
		}

		[Test]
		public void prevent_outlock_without_outview() {
			var ctx = EPTest.Compile(@"
orgnode A
	process Ax
		thema A outlock
thema A
");

			Assert.False(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "ER_EPI03"));
		}


		[Test]
		public void prevent_recoursive_process_in() {
			var ctx =
				EPTest.Compile(@"
orgnode A
	process Ax
		in Cx
	orgnode B
		process Bx
			in Ax
		process Cx
			in Bx
");

			Assert.False(ctx.IsComplete);
			Assert.AreEqual(3, ctx.Errors.Where(x => x.ErrorCode == "ER_EPINTEG_3").Count());
		}

		[Test]
		public void prevent_self_coded_process_in() {
			var ctx = EPTest.Compile(@"
orgnode A
	process Ax
	orgnode B
		process Bx
			in Bx
		process Cx
			in Bx
");

			Assert.False(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "ER_EPI04"));
		}

		[Test]
		public void resolves_org_node() {
			var ctx = EPTest.Compile(@"
orgnode A
	process Ax
	orgnode B
		process Bx
		process Cx
");
			Assert.AreEqual(3, ctx.EcoProcessIndex.Index.Count);
			Assert.AreEqual("A", ctx.EcoProcessIndex["Ax"].OrgNode.Code);
			Assert.AreEqual("B", ctx.EcoProcessIndex["Bx"].OrgNode.Code);
			Assert.AreEqual("B", ctx.EcoProcessIndex["Cx"].OrgNode.Code);
		}

		[Test]
		public void resolves_process_in() {
			var ctx = EPTest.Compile(@"
orgnode A
	process Ax
	orgnode B
		process Bx
			in Ax
		process Cx
			in Bx
");

			Assert.AreEqual("Ax", ctx.EcoProcessIndex.Index["Bx"].InDepends.First().Process.Code);
			Assert.AreEqual("Bx", ctx.EcoProcessIndex.Index["Ax"].OutDepends.First().Process.Code);
			Assert.AreEqual("Bx", ctx.EcoProcessIndex.Index["Cx"].InDepends.First().Process.Code);
			Assert.AreEqual("Cx", ctx.EcoProcessIndex.Index["Bx"].OutDepends.First().Process.Code);
		}


		[Test]
		public void resolves_process_role_map() {
			var ctx = EPTest.Compile(@"
orgnode A
	process Ax
	orgnode B
		process Bx
			in Ax
		process Cx
			in Bx
");

			var map = ctx.ExtraEcoProcessRoleMap();
			Console.WriteLine(map.ToString());
			Action<string, string> hasmap = (from, to) =>
				{
					Assert.NotNull(
						map.Elements().FirstOrDefault(
							x => x.Attr("from") == from && x.Attr("to") == to));
				};
			hasmap("A", "Ax_OWN");
			hasmap("B", "Bx_OWN");
			hasmap("B", "Cx_OWN");
			hasmap("Bx_OWN", "Ax_VIEW");
			hasmap("Cx_OWN", "Bx_VIEW");
		}
	}
}