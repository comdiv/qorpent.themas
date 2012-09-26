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
// Original file : ThemasExtractionTest.cs
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
	public class ThemasExtractionTest : ThemaCompilerTestBase {
		[Test]
		public void basic_import_provided() {
			var result = execute<ExtractThemas>(new miniproj(), LogLevel.All);
			Assert.AreEqual("child1", result.Themas["child2"].Imports[0]);
			Assert.AreEqual("child2", result.Themas["child3"].Imports[0]);
		}

		[Test]
		public void themas_are_excluded() {
			var result = execute<ExtractThemas>(new miniproj());
			foreach (var file in result.SourceFiles) {
				Assert.AreEqual(0, result.SourceFileXml[file].Elements("thema").Count());
				Assert.AreEqual(0, result.SourceFileXml[file].Elements("child1").Count());
				Assert.AreEqual(0, result.SourceFileXml[file].Elements("child2").Count());
			}
		}

		[Test]
		public void themas_are_prepared() {
			var result = execute<ExtractThemas>(new miniproj(), LogLevel.All);
			Assert.True(result.Themas.ContainsKey("base1"));
			Assert.True(result.Themas.ContainsKey("child1"));
			Assert.True(result.Themas.ContainsKey("child2"));
			Assert.True(result.Themas.ContainsKey("child3"));
		}
	}
}