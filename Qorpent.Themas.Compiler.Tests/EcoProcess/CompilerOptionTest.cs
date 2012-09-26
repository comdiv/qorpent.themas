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
// Original file : CompilerOptionTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Tests.EcoProcess {
	[TestFixture]
	public class CompilerOptionTest {
		[Test(Description = "Show that  when UseEcoProcess not used on project - no ecoprocess processing provided")]
		public void not_process_eco_processes_if_option_not_setted_up() {
			var ctx = EPTest.Compile(
				@"
orgnode X
	orgnode Y
", x => x.UseEcoProcess = false
				);
			XElement e = null;
			Assert.Null(e = ctx.ExtraData.Element("ecoprocess_rolemap"));
		}

		[Test(Description = "Show that some eco process activity provided when UseEcoProcess used on project")]
		public void process_eco_processes_if_option_setted_up() {
			var ctx = EPTest.Compile(
				@"
orgnode X
	orgnode Y
"
				);
			XElement e = null;
			Assert.NotNull(e = ctx.ExtraData.Element("ecoprocess_rolemap"));
			Assert.NotNull(e.Elements("map").FirstOrDefault(x => x.Attr("from") == "X" && x.Attr("to") == "Y"));
		}
	}
}