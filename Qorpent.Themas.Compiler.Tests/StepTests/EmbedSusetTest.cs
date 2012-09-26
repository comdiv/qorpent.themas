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
// Original file : EmbedSusetTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Themas.Compiler.Pipelines;
using Qorpent.Themas.Compiler.Steps;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class EmbedSusetTest : ThemaCompilerTestBase {
		[Test]
		public void can_have_errors_on_subset_imports() {
			var result = execute<EmbedSubsets>(new miniproj {NonResolvedSubsetIsError = true}, LogLevel.All);
			Assert.False(result.IsComplete);
			Assert.NotNull(result.Errors.Where(x => x.ErrorCode == "TE2101").FirstOrDefault());
		}

		[Test]
		public void subset_usage_analyzed() {
			var result = execute<EmbedSubsets>(new miniproj {AnalyzeSubsetUsage = true}, LogLevel.All);
			Assert.LessOrEqual(2, result.Errors.Where(x => x.ErrorCode == "TW2102").Count());
		}

		[Test]
		public void subsets_are_marked() {
			var result = execute<EmbedSubsets>(new miniproj(), LogLevel.All);
			Assert.NotNull(result.SubsetIndex["ua"].Annotation<UsedInWorkingThemaAnnotation>());
			Assert.NotNull(result.SubsetIndex["ub"].Annotation<UsedInWorkingThemaAnnotation>());
			Assert.Null(result.SubsetIndex["nua"].Annotation<UsedInWorkingThemaAnnotation>());
			Assert.Null(result.SubsetIndex["nub"].Annotation<UsedInWorkingThemaAnnotation>());
		}

		[Test]
		public void subsets_are_resolved() {
			var result = execute<EmbedSubsets>(new miniproj(), LogLevel.All);
			var td = result.Themas["usubset"].Items["A.out"];
			Assert.AreEqual(2, td.Elements("col").Count());
			var es = td.Elements("col").ToArray();
			Assert.AreEqual("a", es[0].Attribute("code").Value);
			Assert.AreEqual("b", es[1].Attribute("code").Value);
		}
	}
}