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
// Original file : ConditionalCompilation.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class ConditionalCompilation : ThemaCompilerTestBase {
		/*
		if OPT1 :
			thema opt1
		if !OPT1 :
			thema not_opt1
		if OPT1|OPT2:
			thema opt1_or_opt2
		if OPT1&OPT2:
			thema opt1_and_opt2
		if OP1&!OPT2
			thema opt1_and_not_opt2
		 */

		private void test(string[] options, bool opt1, bool not_opt1, bool opt1_or_opt2,
		                  bool opt1_and_opt2, bool opt1_and_not_opt2) {
			var result = execute<RemoveUnUsedThemas>(new miniproj(options), LogLevel.All);
			Assert.AreEqual(opt1, result.Themas.ContainsKey("opt1"));
			Assert.AreEqual(not_opt1, result.Themas.ContainsKey("not_opt1"));
			Assert.AreEqual(opt1_or_opt2, result.Themas.ContainsKey("opt1_or_opt2"));
			Assert.AreEqual(opt1_and_opt2, result.Themas.ContainsKey("opt1_and_opt2"));
			Assert.AreEqual(opt1_and_not_opt2, result.Themas.ContainsKey("opt1_and_not_opt2"));
		}

		[Test]
		public void no_opt() {
			test(new string[] {}, false, true, false, false, false);
		}

		[Test]
		public void opt1() {
			test(new[] {"OPT1"}, true, false, true, false, true);
		}

		[Test]
		public void opt1_and_opt2() {
			test(new[] {"OPT1", "OPT2"}, true, false, true, true, false);
		}

		[Test]
		public void opt2() {
			test(new[] {"OPT2"}, false, true, true, false, false);
		}
	}
}