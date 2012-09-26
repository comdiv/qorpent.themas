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
// Original file : EmbedParametersTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Themas.Compiler.Pipelines;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class EmbedParametersTest : ThemaCompilerTestBase {
		[Test]
		public void complex_parameters_and_list_definition() {
			const string code =
				@"
param p1, list= '1|2'
thema A
	p1_list = '2|4|5'
	p1_code = p1
	out A
		ask @p1_code, +list=@p1_list # must support resolve parameters in call 
A B
	+p1_list= '6|7'
	-p1_list= '1|4' # 1 exclusion will fail due to thema params are resolved before item parameters
";
			var simplecomp = new CompileSingleAction(code, "");
			simplecomp.Process();
			var td = simplecomp.LastContext.Themas["B"];
			var p = td.Items["A.out"].Elements("var").First();
			Assert.AreEqual("1|2|5|6|7", p.Attribute("list").Value);
		}


		[Test]
		public void test_embeded_parameters_in_generic() {
			var result = execute<EmbedParameters>(new miniproj(), LogLevel.All);
			var td = result.Themas["importgen"].Items["B.out"];
			Assert.AreEqual("4", td.Attr("param"));
		}

		[Test]
		public void test_embeded_parameters_in_imported_subset() {
			var result = execute<EmbedParameters>(new miniproj(), LogLevel.All);
			var td = result.Themas["usubset"].Items["A.out"];
			var x = td.Elements("param").Where(x_ => "X" == x_.Attribute("code").Value).First();
			var y = td.Elements("var").Where(x_ => "Y" == x_.Attribute("code").Value).First();
			Assert.AreEqual("1_1", x.Value);
			Assert.AreEqual("usubset_1", y.Value);
			Assert.AreEqual("1", y.Attribute("c").Value);
		}
	}
}