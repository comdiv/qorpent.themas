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
// Original file : ResolveParametersTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class ResolveParametersTest : ThemaCompilerTestBase {
		[Test]
		public void generic_parameters_resolved() {
			var result = execute<ResolveParameters>(new miniproj(), LogLevel.All);
			CollectionAssert.IsSubsetOf(
				new Dictionary<string, string> {{"x.", "1"}, {"y.", "2"}},
				result.Themas["genericbase"].ResolvedParameters);
			CollectionAssert.IsSubsetOf(
				new Dictionary<string, string> {{"xA", "1"}, {"yA", "3"}, {"zA", "4"}},
				result.Themas["A"].ResolvedParameters);
			CollectionAssert.IsSubsetOf(
				new Dictionary<string, string> {{"xB", "1"}, {"yB", "4"}, {"zB", "5"}},
				result.Themas["B"].ResolvedParameters);
			CollectionAssert.IsSubsetOf(
				new Dictionary<string, string> {{"xC", "1"}, {"yC", "5"}, {"zC", "6"}},
				result.Themas["C"].ResolvedParameters);
			CollectionAssert.IsSubsetOf(
				new Dictionary<string, string>
					{
						{"xA", "1"},
						{"yA", "3"},
						{"zA", "4"},
						{"xB", "1"},
						{"yB", "4"},
						{"zB", "5"},
						{"xC", "1"},
						{"yC", "5"},
						{"zC", "6"}
					},
				result.Themas["importgen"].ResolvedParameters);
		}

		[Test]
		public void order_independence_for_complex() {
			var code = @"
thema A,  x = 1, +x=2
thema B,  +x=2,   x=1			";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var td = simplecomp.LastContext.Themas["A"];
			Assert.AreEqual("1|2", td.ResolvedParameters["x"]);
			td = simplecomp.LastContext.Themas["B"];
			Assert.AreEqual("1|2", td.ResolvedParameters["x"]);
		}


		[Test]
		public void parameter_calculation_test() {
			/*         
child2 restest
    x = child               child
    f = 1                   1
	y = @code               restest
	a = @x                  child
	b = @y                  restest
	c = @a                  child
	d = "${a}_${b}_${y}"    child_restest_restest
            */
			var result = execute<CalculateParameters>(new miniproj(), LogLevel.All);
			CollectionAssert.IsSubsetOf(new Dictionary<string, string>
				{
					{"x", "child"},
					{"f", "1"},
					{"y", "restest"},
					{"a", "child"},
					{"b", "restest"},
					{"c", "child"},
					{"d", "child_restest_restest"},
				}, result.Themas["restest"].ResolvedParameters);
		}

		[Test]
		public void self_only_attributes_not_inherited() {
			var result = execute<ResolveParameters>(new miniproj(), LogLevel.All);
			Assert.False(result.Themas["importgen"].ResolvedParameters.ContainsKey("generic"));
			Assert.AreNotEqual(result.Themas["child1"].ResolvedParameters["_file"],
			                   result.Themas["base1"].ResolvedParameters["_file"]);
		}

		[Test]
		public void simple_thema_parameters_resolved() {
			var result = execute<ResolveParameters>(new miniproj(), LogLevel.All);
			CollectionAssert.IsSubsetOf(new Dictionary<string, string> {{"x", "@code"}, {"y", "@x"}},
			                            result.Themas["base1"].ResolvedParameters);
			CollectionAssert.IsSubsetOf(new Dictionary<string, string> {{"x", "child"}, {"y", "@x"}},
			                            result.Themas["child1"].ResolvedParameters);
			CollectionAssert.IsSubsetOf(new Dictionary<string, string> {{"x", "child"}, {"y", "@x"}, {"f", "1"}},
			                            result.Themas["child2"].ResolvedParameters);
		}

		[Test]
		public void test_complex_parameter() {
			var code = @"
thema A, x = 1
A B, +x = 2
B C, +x = 3
C D, +x = 4, -x=2
";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var td = simplecomp.LastContext.Themas["D"];
			Assert.AreEqual("1|3|4", td.ResolvedParameters["x"]);
		}
	}
}