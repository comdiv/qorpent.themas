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
// Original file : SubsetExtractionTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using NUnit.Framework;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class SubsetExtractionTest : ThemaCompilerTestBase {
		[Test]
		public void subsets_are_excluded() {
			var result = execute<ExtractSubsets>(new miniproj());
			foreach (var file in result.SourceFiles) {
				Assert.AreEqual(0, result.SourceFileXml[file].Elements("subset").Count());
			}
		}

		[Test]
		public void subsets_are_prepared() {
			var result = execute<ExtractSubsets>(new miniproj());
			Assert.True(result.SubsetIndex.ContainsKey("A"));
			Assert.True(result.SubsetIndex.ContainsKey("B"));
			Assert.True(result.SubsetIndex.ContainsKey("C"));
			Assert.AreEqual(3, result.SubsetIndex["A"].Elements("col").Count()); //tests that XML got from valid file
		}
	}
}