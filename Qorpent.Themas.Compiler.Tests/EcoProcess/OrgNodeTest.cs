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
// Original file : OrgNodeTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Tests.EcoProcess {
	[TestFixture]
	public class OrgNodeTest {
		[Test]
		public void builds_role_mapping_tree() {
			var ctx = EPTest.Compile(@"
orgnode A
	orgnode B
		orgnode C
");
			var maps = ctx.ExtraEcoProcessRoleMap().Elements();
			Assert.AreEqual(2, maps.Count());
			Assert.NotNull(maps.FirstOrDefault(x => x.Attr("from") == "A" && x.Attr("to") == "B"));
			Assert.NotNull(maps.FirstOrDefault(x => x.Attr("from") == "B" && x.Attr("to") == "C"));
		}

		[Test]
		public void prevents_double_coded_orgnodes() {
			var ctx = EPTest.Compile(@"
orgnode A
	orgnode C
		orgnode C
");
			Assert.False(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "ER_EPI02"));
		}

		[Test]
		public void prevents_non_coded_orgnodes() {
			var ctx = EPTest.Compile(@"
orgnode A
	orgnode
		orgnode C
");
			Assert.False(ctx.IsComplete);
			Assert.NotNull(ctx.Errors.FirstOrDefault(x => x.ErrorCode == "ER_EPI02"));
		}
	}
}