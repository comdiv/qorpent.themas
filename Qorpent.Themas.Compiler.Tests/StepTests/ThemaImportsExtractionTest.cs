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
// Original file : ThemaImportsExtractionTest.cs
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
	public class ThemaImportsExtractionTest : ThemaCompilerTestBase {
		[Test]
		public void thema_extenders_excluded() {
			var result = execute<ExtractThemaExtenders>(new miniproj(), LogLevel.All);
			Assert.AreEqual(0, result.Themas["ext1"].Fullsource.Elements("extend").Count());
			Assert.True(result.Themas["ext1"].IsExtension);
		}

		[Test]
		public void thema_import_forced_error_occured() {
			var result = execute<ExtractThemaImports>(new miniproj {NonResolvedImportIsError = true},
			                                          LogLevel.All);
			Assert.False(result.IsComplete);
			Assert.NotNull(result.Errors.FirstOrDefault(x => x.ErrorCode == "TE1201"));
		}

		[Test]
		public void thema_import_warn_occured() {
			var result = execute<ExtractThemaImports>(new miniproj(), LogLevel.All);
			Assert.NotNull(result.Errors.FirstOrDefault(x => x.ErrorCode == "TW1201"));
		}

		[Test]
		public void thema_imports_and_extenders_are_prepared() {
			var result = execute<ExtractThemaExtenders>(new miniproj(), LogLevel.All);
			CollectionAssert.AreEqual(new[] {"base1", "ext1"}, result.Themas["child1"].Imports);
			CollectionAssert.AreEqual(new[] {"child1"}, result.Themas["child2"].Imports);
			CollectionAssert.AreEqual(new[] {"child2"}, result.Themas["child3"].Imports);
			CollectionAssert.AreEqual(new[] {"A", "B", "C"}, result.Themas["importgen"].Imports);
		}


		[Test]
		public void thema_imports_are_excluded() {
			var result = execute<ExtractThemaImports>(new miniproj());
			Assert.AreEqual(0, result.Themas["child1"].Fullsource.Elements("import").Count());
			Assert.AreEqual(0, result.Themas["child2"].Fullsource.Elements("import").Count());
			Assert.AreEqual(0, result.Themas["child3"].Fullsource.Elements("import").Count());
		}

		[Test]
		public void thema_imports_are_prepared() {
			var result = execute<ExtractThemaImports>(new miniproj(), LogLevel.All);
			CollectionAssert.AreEqual(new[] {"base1"}, result.Themas["child1"].Imports);
			CollectionAssert.AreEqual(new[] {"child1"}, result.Themas["child2"].Imports);
			CollectionAssert.AreEqual(new[] {"child2"}, result.Themas["child3"].Imports);
			CollectionAssert.AreEqual(new[] {"A", "B", "C"}, result.Themas["importgen"].Imports);
		}
	}
}