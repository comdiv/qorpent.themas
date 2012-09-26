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
// Original file : EmbedItemParametersTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Themas.Compiler.Pipelines;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class EmbedItemParametersTest : ThemaCompilerTestBase {
		[Test]
		public void can_have_errors_on_subset_imports() {
			var result = execute<EmbedItemParameters>(new miniproj {NonResolvedParameterIsError = true},
			                                          LogLevel.All);
			Assert.False(result.IsComplete);
			Assert.NotNull(result.Errors.Where(x => x.ErrorCode == "TE2201").FirstOrDefault());
		}

		[Test]
		public void parameters_are_resolved() {
			var result = execute<EmbedItemParameters>(new miniproj(), LogLevel.All);
			var td = result.Themas["usubset"].Items["A.out"];
			Assert.AreEqual(3, td.Elements("var").Count());
			Assert.AreEqual(1, td.Elements("param").Count());
			var z = td.Elements("var").Where(x_ => "Z" == x_.Attribute("code").Value).First();
			var x = td.Elements("param").Where(x_ => "X" == x_.Attribute("code").Value).First();
			var y = td.Elements("var").Where(x_ => "Y" == x_.Attribute("code").Value).First();
			var a = td.Elements("var").Where(x_ => "A" == x_.Attribute("code").Value).First();
			Assert.AreEqual("${x}_${x}", x.Value);
			Assert.NotNull(y.Attribute("p"));
			Assert.True(a.Value.IsEmpty());
		}


		[Test]
		public void parameters_usage_analyzed() {
			var result = execute<EmbedItemParameters>(new miniproj {AnalyzeParamUsage = true}, LogLevel.All);
			Assert.LessOrEqual(2, result.Errors.Where(x => x.ErrorCode == "TW2202").Count());
		}
	}
}