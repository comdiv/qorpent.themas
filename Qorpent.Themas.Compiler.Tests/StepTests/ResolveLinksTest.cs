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
// Original file : ResolveLinksTest.cs
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
	public class ResolveLinksTest : ThemaCompilerTestBase {
		[Test]
		public void embeded_link_resolved() {
			var result = execute<RemoveUnUsedThemas>(new miniproj(), LogLevel.All);
			var parent = result.Themas["lparent"];
			var child = result.Themas["lchild1"];
			var parentlink = child.Links.FirstOrDefault(x => x.Type.Code == "parent");
			var backlink = parent.BackLinks.FirstOrDefault(x => x.SourceCode == "lchild1" && x.Type.Code == "parent");
			Assert.NotNull(parentlink);
			Assert.NotNull(backlink);
			Assert.NotNull(parentlink.Xml);
			Assert.AreSame(parentlink, backlink);
			Assert.AreEqual("lparent", parentlink.TargetCode);
			Assert.AreEqual("lchild1", parentlink.SourceCode);
			Assert.AreEqual(parent, parentlink.Target);
			Assert.AreEqual(child, parentlink.Source);
		}
	}
}