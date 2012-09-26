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
// Original file : CleanupParametersTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class CleanupParametersTest : ThemaCompilerTestBase {
		[Test]
		public void eco_cleanup_test() {
			var result = execute<EcoCleanupParameters>(new miniproj {UseEcoOptimization = true}, LogLevel.All);
			var full = result.Themas["importgen"];
			var bonly = result.Themas["Bonly"];


			Assert.True(full.ResolvedParameters.ContainsKey("xA"));
			Assert.True(full.ResolvedParameters.ContainsKey("xB"));
			Assert.True(full.ResolvedParameters.ContainsKey("xC"));

			Assert.False(bonly.ResolvedParameters.ContainsKey("xA"));
			Assert.True(bonly.ResolvedParameters.ContainsKey("xB"));
			Assert.False(bonly.ResolvedParameters.ContainsKey("xC"));
		}

		[Test]
		public void simple_parameter_cleanup_test() {
			var result = execute<CleanupParameters>(new miniproj(), LogLevel.All);
			var td = result.Themas["cleanuptest"];
			Assert.True(td.ResolvedParameters.ContainsKey("b"));
			Assert.True(td.ResolvedParameters.ContainsKey("d"));
			Assert.True(td.ResolvedParameters.ContainsKey("g"));

			Assert.False(td.ResolvedParameters.ContainsKey("a"));
			Assert.False(td.ResolvedParameters.ContainsKey("c"));
			Assert.False(td.ResolvedParameters.ContainsKey("e"));
		}
	}
}