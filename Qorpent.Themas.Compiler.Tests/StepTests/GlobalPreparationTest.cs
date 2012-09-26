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
// Original file : GlobalPreparationTest.cs
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
	public class GlobalPreparationTest : ThemaCompilerTestBase {
		[Test]
		public void globals_are_embeded() {
			var result = execute<EmbedGlobals>(new miniproj(), LogLevel.All);
			var myfile = result.GetGlobalByLocal("/tfolder2/child1.bxl");
			Assert.AreEqual("zzza",
			                result.SourceFileXml[myfile].Elements("subset").First().Elements("col").First().Attribute("name").
				                Value);
		}

		[Test]
		public void globals_are_excluded_from_SourceXml() {
			var result = execute<ExtractGlobals>(new miniproj());
			foreach (var xml in result.SourceFileXml) {
				Assert.AreEqual(0, xml.Value.Elements("global").Count());
			}
		}

		[Test]
		public void globals_are_reincluded_to_eachOther() {
			var result = execute<ReIncludeGlobalsToGlobals>(new miniproj(), LogLevel.All);
			Assert.AreEqual("zzza", result.Globals["A"]);
		}

		[Test]
		public void globals_are_stored_in_Globals() {
			var result = execute<ExtractGlobals>(new miniproj());
			Assert.True(result.Globals.ContainsKey("X"));
			Assert.True(result.Globals.ContainsKey("Y"));
			Assert.AreEqual("zzz", result.Globals["X"]);
			Assert.AreEqual("Yyy", result.Globals["Y"]);
		}


		[Test]
		public void only_tildet_files_are_marked_to_process_globals() {
			var result = execute<ExtractGlobals>(new miniproj(), LogLevel.All);
			foreach (var e in result.SourceFileXml) {
				if (!e.Key.Contains("child2")) {
					Assert.NotNull(e.Value.Annotation<HaveToBeProcessedWithGlobalsAnnotation>());
				}
				else {
					Assert.Null(e.Value.Annotation<HaveToBeProcessedWithGlobalsAnnotation>());
				}
			}
		}
	}
}