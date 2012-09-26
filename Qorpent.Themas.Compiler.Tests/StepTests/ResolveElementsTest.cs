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
// Original file : ResolveElementsTest.cs
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
	public class ResolveElementsTest : ThemaCompilerTestBase {
		[Test]
		public void eco_cleanup_elements_works() {
			var result = execute<EcoCleanupElements>(new miniproj {UseEcoOptimization = true}, LogLevel.All);
			var td = result.Themas["importgen"];
			var bonly = result.Themas["Bonly"];
			Assert.True(td.ImportedThemaItems.ContainsKey("A.out"));
			Assert.True(td.ImportedThemaItems.ContainsKey("B.out"));
			Assert.True(td.ImportedThemaItems.ContainsKey("C.out"));

			Assert.False(bonly.ImportedThemaItems.ContainsKey("A.out"));
			Assert.True(bonly.ImportedThemaItems.ContainsKey("B.out"));
			Assert.False(bonly.ImportedThemaItems.ContainsKey("C.out"));
		}

		[Test]
		public void inactive_themas_removed_after_element_bindings() {
			var result = execute<RemoveUnUsedThemas>(new miniproj(), LogLevel.All);
			Assert.True(result.Themas.ContainsKey("child1"));
			Assert.True(result.Themas.ContainsKey("child2"));
			Assert.True(result.Themas.ContainsKey("child3"));

			Assert.False(result.Themas.ContainsKey("base1"));
			Assert.False(result.Themas.ContainsKey("genericbase"));
			Assert.False(result.Themas.ContainsKey("A"));
			Assert.False(result.Themas.ContainsKey("B"));
			Assert.False(result.Themas.ContainsKey("C"));
			Assert.False(result.Themas.ContainsKey("ext1"));
			Assert.False(result.Themas.ContainsKey("erext"));
		}

		[Test]
		public void simple_element_xml_test() {
			var result = execute<ResolveElements>(new miniproj(), LogLevel.All);
			var td = result.Themas["child2"];
			Assert.AreEqual(3, td.Items["A.out"].Elements("col").Count());
			Assert.AreEqual("C", td.Items["A.out"].Elements("col").First().Attribute("code").Value);
			Assert.AreEqual("D", td.Items["A.out"].Elements("col").Skip(1).First().Attribute("code").Value);
			Assert.AreEqual("E", td.Items["A.out"].Elements("col").Skip(2).First().Attribute("code").Value);
		}


		[Test]
		public void simple_generic_test_import() {
			var result = execute<ResolveElementsImport>(new miniproj(), LogLevel.All);
			var td = result.Themas["importgen"];
			var tda = result.Themas["A"];
			var tdb = result.Themas["B"];
			var tdc = result.Themas["C"];
			var gbase = result.Themas["genericbase"];
			Assert.True(td.ImportedThemaItems["A.out"] == tda.ImportedThemaItems["A.out"]);
			Assert.True(td.ImportedThemaItems["B.out"] == tdb.ImportedThemaItems["B.out"]);
			Assert.True(td.ImportedThemaItems["C.out"] == tdc.ImportedThemaItems["C.out"]);
			Assert.True(td.ImportedThemaItems["A.out"] == gbase.SelfThemaItems["_..out"]);
			Assert.True(td.ImportedThemaItems["B.out"] == gbase.SelfThemaItems["_..out"]);
			Assert.True(td.ImportedThemaItems["C.out"] == gbase.SelfThemaItems["_..out"]);

			Assert.True(td.ImportedThemaItems.ContainsKey("B.out"));
			Assert.True(td.SelfThemaItemsExtensions.ContainsKey("C.out"));
		}

		[Test]
		public void simple_scenario_of_self_set_ext_import() {
			var result = execute<ResolveElementsImport>(new miniproj(), LogLevel.All);
			var td = result.Themas["child2"];
			var tdbase = result.Themas["base1"];
			var tdc1 = result.Themas["child1"];
			Assert.True(td.ImportedThemaItems["A.out"] == tdbase.SelfThemaItems["A.out"]);
			Assert.True(td.ImportedThemaItemsSets["A.out"] == tdc1.SelfThemaItemsSets["A.out"]);
		}
	}
}