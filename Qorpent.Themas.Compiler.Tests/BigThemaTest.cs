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
// Original file : BigThemaTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests {
	[TestFixture]
	[Category("LARGE")]
	public class BigThemaTest : ThemaCompilerTestBase {
		[Test]
		public void error_if_no_target_folder() {
			var proj = new ThemaProject();
			proj.SourceFiles = new[] {"~/systh", "~/usrth"};
			proj.OutputFolder = "";
			proj.UseEcoOptimization = true;
			proj.AnalyzeSubsetUsage = false;
			proj.AnalyzeParamUsage = false;
			proj.KeepAbstractThemas = false;
			proj.NonResolvedImportIsError = true;
			var result = execute<CompileToXml>(proj, LogLevel.Error);
			Assert.False(result.IsComplete);
		}

		[Test]
		public void full_non_strict_compilation() {
			var proj = new ThemaProject();
			proj.SourceFiles = new[] {"~/systh", "~/usrth"};
			proj.UseEcoOptimization = true;
			proj.AnalyzeSubsetUsage = true;
			proj.AnalyzeParamUsage = true;
			proj.KeepAbstractThemas = true;
			var result = execute<GenerateXml>(proj, LogLevel.All);
			Assert.True(result.IsComplete);
		}

		[Test]
		public void save_to_disk_abstract_mode() {
			var proj = new ThemaProject();
			proj.SourceFiles = new[] {"~/systh", "~/usrth"};
			proj.OutputFolder = "~/bigoutabst";
			proj.UseEcoOptimization = true;
			proj.AnalyzeSubsetUsage = false;
			proj.AnalyzeParamUsage = false;
			proj.KeepAbstractThemas = true;
			proj.NonResolvedImportIsError = true;
			var result = execute<CompileToXml>(proj, LogLevel.Error);
			Assert.True(result.IsComplete);
		}

		[Test]
		public void save_to_disk_usual_mode() {
			var proj = new ThemaProject();
			proj.SourceFiles = new[] {"~/systh", "~/usrth"};
			proj.OutputFolder = "~/bigout";
			proj.UseEcoOptimization = true;
			proj.AnalyzeSubsetUsage = false;
			proj.AnalyzeParamUsage = false;
			proj.KeepAbstractThemas = false;
			proj.NonResolvedImportIsError = true;
			var result = execute<CompileToXml>(proj, LogLevel.All);
			Assert.True(result.IsComplete);
		}

		[Test]
		public void usual_compilation() {
			var proj = new ThemaProject();
			proj.SourceFiles = new[] {"~/systh", "~/usrth"};
			proj.UseEcoOptimization = true;
			proj.AnalyzeSubsetUsage = false;
			proj.AnalyzeParamUsage = false;
			proj.KeepAbstractThemas = false;
			proj.NonResolvedImportIsError = true;
			var result = execute<GenerateXml>(proj, LogLevel.Error);
			Assert.True(result.IsComplete);
		}
	}
}