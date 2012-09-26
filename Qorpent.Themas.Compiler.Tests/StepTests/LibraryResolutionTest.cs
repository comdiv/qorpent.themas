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
// Original file : LibraryResolutionTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Linq;
using NUnit.Framework;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class LibraryResolutionTest : ThemaCompilerTestBase {
		[Test]
		public void error_on_import_library() {
			const string code = @"
library X
thema Y
	import X
";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			Assert.False(simplecomp.LastContext.IsComplete);
			Assert.NotNull(simplecomp.LastContext.Errors.FirstOrDefault(x => x.ErrorCode == "TE1205"));
		}


		[Test]
		public void error_on_invalid_ref() {
			const string code = @"
library X
	out ZZ
thema Y
	out REP
		uselib ZZ2
";
			var simplecomp = new CompileSingleAction(code, "nonresolvedlibraryiserror true");
			simplecomp.Process();
			Console.WriteLine(simplecomp.LastContext.Project.GetLog());
			Assert.True(simplecomp.LastContext.Project.NonResolvedLibraryIsError);
			Assert.False(simplecomp.LastContext.IsComplete);
			Assert.NotNull(simplecomp.LastContext.Errors.FirstOrDefault(x => x.ErrorCode == "ERLIBR03"));
		}

		[Test]
		public void library_keyword_works() {
			const string code = @"
library X
library Y, abst
";
			var simplecomp = new CompileSingleAction(code, "");
			var result = (ThemaCompilerResultForQweb) simplecomp.Process();
			Console.WriteLine(simplecomp.LastContext.Project.GetLog());
			//inspection muter
			Assert.NotNull(result.Errors);
			Assert.True(simplecomp.LastContext.IsComplete);
			Assert.True(result.IsComplete);
			Assert.NotNull(result.Log);
			Assert.NotNull(result.Result);
			//
			var td = simplecomp.LastContext.Themas["X"];
			Assert.True(td.IsWorking);
			Assert.True(td.IsLibrary);
			td = simplecomp.LastContext.Themas["Y"];
			Assert.True(td.IsWorking); //check abst ignorance
			Assert.True(td.IsLibrary);
		}

		[Test]
		public void short_lib_references() {
			const string code = @"
library X
	out ZZ
thema Y
	out REP
		uselib ZZ
";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var td = simplecomp.LastContext.Themas["Y"];
			Console.WriteLine(td.Xml);
		}

		[Test]
		public void warn_on_invalid_ref() {
			const string code = @"
library X
	out ZZ
thema Y
	out REP
		uselib ZZ2
";
			var simplecomp = new CompileSingleAction(code, "nonresolvedlibraryiserror false");
			simplecomp.Process();
			Console.WriteLine(simplecomp.LastContext.Project.GetLog());
			Assert.False(simplecomp.LastContext.Project.NonResolvedLibraryIsError);
			simplecomp.LastContext.Project.NonResolvedLibraryIsError = false; //inspection muter
			Assert.True(simplecomp.LastContext.IsComplete);
			Assert.NotNull(simplecomp.LastContext.Errors.FirstOrDefault(x => x.ErrorCode == "WRLIBR03"));
		}
	}
}