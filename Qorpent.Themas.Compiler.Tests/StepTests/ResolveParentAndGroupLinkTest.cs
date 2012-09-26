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
// Original file : ResolveParentAndGroupLinkTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class ResolveParentAndGroupLinkTest : ThemaCompilerTestBase {
		[Test]
		public void bad_group() {
			var code = @"
thema x, isgroup = false
thema y
	group x
";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var result = simplecomp.LastContext;
			Assert.False(result.IsComplete);
			Assert.NotNull(result.Errors.FirstOrDefault(x => x.ErrorCode == "ERLINK03"));
		}

		[Test]
		public void bad_parent_target() {
			var code = @"
thema x
	parent y
";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var result = simplecomp.LastContext;
			Assert.False(result.IsComplete);
			Assert.NotNull(result.Errors.FirstOrDefault(x => x.ErrorCode == "ERLINK01"));
		}

		[Test]
		public void bad_parent_target_isgroup() {
			var code = @"
thema y , isgroup = true
thema x
	parent y
";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var result = simplecomp.LastContext;
			Assert.False(result.IsComplete);
			Assert.NotNull(result.Errors.FirstOrDefault(x => x.ErrorCode == "LP01"));
		}


		[Test]
		public void bad_parent_target_library() {
			var code = @"
library y 
thema x
	parent y
";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var result = simplecomp.LastContext;
			Assert.False(result.IsComplete);
			Assert.NotNull(result.Errors.FirstOrDefault(x => x.ErrorCode == "LP01"));
		}


		[Test]
		public void invalid_grop_parent_mix_grp() {
			var code =
				@"
thema g, isgroup = true
thema g2, isgroup = true
thema p 
	group g
thema c
	parent p
	group g2

";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var result = simplecomp.LastContext;
			Assert.False(result.IsComplete);
			Assert.NotNull(result.Errors.FirstOrDefault(x => x.ErrorCode == "LP02"));
		}

		[Test]
		public void invalid_grop_parent_mix_no_grp() {
			var code = @"
thema g, isgroup = true

thema p 

thema c
	parent p
	group g

";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var result = simplecomp.LastContext;
			Assert.False(result.IsComplete);
			Assert.NotNull(result.Errors.FirstOrDefault(x => x.ErrorCode == "LP02"));
		}

		[Test]
		public void normal_group() {
			var code = @"
thema x, isgroup = true
thema y
	group x
";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var result = simplecomp.LastContext;
			Assert.True(result.IsComplete);
			var th = result.Themas["y"];
			Assert.NotNull(th.Links.FirstOrDefault(x => x.Type.Code == "group" && x.Target.Code == "x"));
		}

		[Test]
		public void simple_thema_parameters_resolved() {
			var result = execute<ResolveLinks>(new miniproj(), LogLevel.All);
			var p = result.Themas["lparent"];
			var c1 = result.Themas["lchild1"];
			var c2 = result.Themas["lchild2"];
			var c3 = result.Themas["lchild3"];
			var c4 = result.Themas["lchild4"];
			Assert.AreEqual(3, p.BackLinks.Count);
			Assert.AreEqual(0, p.Links.Count);
			Assert.AreEqual(1, c1.Links.Count);
			Assert.AreEqual(1, c2.Links.Count);
			Assert.AreEqual(1, c3.Links.Count);
			Assert.AreEqual(1, c4.Links.Count);
			Assert.AreEqual(1, c1.BackLinks.Count);
			Assert.AreEqual("lparent", c1.Links[0].TargetCode);
			Assert.AreEqual("lparent", c2.Links[0].TargetCode);
			Assert.AreEqual("lparent", c3.Links[0].TargetCode);
			Assert.AreEqual("lchild1", c4.Links[0].TargetCode);
		}

		[Test]
		public void valid_grop_parent_mix_grp() {
			var code = @"
thema g, isgroup = true
thema p 
	group g
thema c
	parent p
	group g

";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var result = simplecomp.LastContext;
			Assert.True(result.IsComplete);
		}

		[Test]
		public void valid_grop_parent_mix_no_grp() {
			var code = @"
thema g, isgroup = true
thema p 
	group g
thema c
	parent p

";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var result = simplecomp.LastContext;
			Assert.True(result.IsComplete);
		}
	}
}