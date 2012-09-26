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
// Original file : invalid_generic_import_in_assoi_eco.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests.BugFixing {
	[TestFixture]
	public class invalid_generic_import_in_assoi_eco : BugFixingThemaTest {
		[Test]
		public void invalid_generic_import_in_assoi_eco_main() {
			var result = execute();
			var td = result.Themas["REALBASIS"];
			Assert.AreEqual("x", td.ResolvedParameters["p1A"]);
			Assert.AreEqual("x", td.ResolvedParameters["p2A"]);
			Assert.AreEqual("x", td.ResolvedParameters["p3A"]);
		}

		[Test]
		public void invalid_generic_import_in_assoi_eco_resolveparameters() {
			var result = execute<ResolveParameters>();
			var td = result.Themas["REALBASIS"];
			Assert.AreEqual("x", td.ResolvedParameters["p1A"]);
			Assert.AreEqual("@p1A", td.ResolvedParameters["p2A"]);
			Assert.AreEqual("${p1A}", td.ResolvedParameters["p3A"]);
		}
	}
}