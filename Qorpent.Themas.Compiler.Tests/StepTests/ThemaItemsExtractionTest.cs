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
// Original file : ThemaItemsExtractionTest.cs
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
	public class ThemaItemsExtractionTest : ThemaCompilerTestBase {
		[Test]
		public void thema_items_are_excluded() {
			var result = execute<ExtractThemaSelfItems>(new miniproj());
			Assert.AreEqual(0, result.Themas["base1"].Fullsource.Elements("out").Count());
			Assert.AreEqual(0, result.Themas["child1"].Fullsource.Elements("outset").Count());
			Assert.AreEqual(0, result.Themas["child2"].Fullsource.Elements("outext").Count());
		}

		[Test]
		public void thema_items_are_prepared() {
			var result = execute<ExtractThemaSelfItems>(new miniproj(), LogLevel.All);
			Assert.True(result.Themas["base1"].SelfThemaItems.ContainsKey("A.out"));
			Assert.True(result.Themas["child1"].SelfThemaItemsSets.ContainsKey("A.out"));
			Assert.True(result.Themas["child2"].SelfThemaItemsExtensions.ContainsKey("A.out"));
		}
	}
}