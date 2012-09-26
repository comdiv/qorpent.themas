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
// Original file : invalid_role_resolution.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests.BugFixing {
	[TestFixture]
	public class invalid_role_resolution : BugFixingThemaTest {
		[Test]
		public void check_param_resolution() {
			var result = execute<ResolveParameters>();
			var td = result.Themas["REAL"];
			Assert.AreEqual("DR", td.ResolvedParameters["prefix"]);
			Assert.AreEqual("${prefix}${rolesufA}", td.ResolvedParameters["roleA"]);
		}

		[Test]
		public void invalid_role_resolution_main() {
			var result = execute();
			var td = result.Themas["REAL"];
			Assert.AreEqual("DR_OPER", td.ResolvedParameters["roleA"]);
		}
	}
}