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
// Original file : GeneratorsTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Themas.Compiler.Pipelines;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class GeneratorsTest : ThemaCompilerTestBase {
		public class SimpleGenerator : IThemaXmlGenerator {
			public IEnumerable<XElement> Generate(XElement self) {
				return new[] {new XElement("vsg", new XAttribute("x", self.Attr("x").ToInt()*2))};
			}
		}

		public class CompileTimeGenerator : IThemaCompileTimeGenerator {
			public IEnumerable<XElement> Generate(XElement self) {
				return Generate(self, null);
			}

			public IEnumerable<XElement> Generate(XElement self, ThemaCompilerContext context) {
				if (null == self) {
					throw new ArgumentNullException("self");
				}
				if (null != context) {
					return new[] {new XElement("ctg", new XAttribute("step", context.StepIndex))};
				}
				throw new Exception("need context");
			}
		}

		[Test]
		public void can_break_build_if_strict_generator_mode_used() {
			var result = execute<CallGenerators>(new miniproj {NonResolvedGeneratorIsError = true}, LogLevel.All);
			Assert.False(result.IsComplete);
			Assert.NotNull(result.Errors.FirstOrDefault(x => x.ErrorCode == "TE0201"));
		}

		[Test]
		public void generators_called() {
			var result = execute<CallGenerators>(new miniproj(), LogLevel.All);
			Assert.True(result.IsComplete);
			Assert.NotNull(result.Errors.FirstOrDefault(x => x.ErrorCode == "TW2501"));
			Assert.NotNull(result.Errors.FirstOrDefault(x => x.ErrorCode == "TW0201"));
			var td = result.Themas["withgens"];
			var e1 = td.Fullsource.Element("vsg");
			Assert.AreEqual("4", e1.Attr("x"));
			Assert.NotNull(td.Items["A.out"].Element("ctg"));
		}

		[Test]
		public void generators_initiated() {
			var result = execute<RegisterGenerators>(new miniproj(), LogLevel.All);
			Assert.True(result.Generators["g1"].IsValid);
			Assert.AreEqual("g1", result.Generators["g1"].Code);

			Assert.AreEqual(
				"Qorpent.Themas.Compiler.Tests.StepTests.GeneratorsTest+SimpleGenerator,Qorpent.Themas.Compiler.Tests",
				result.Generators["g1"].Typename);
			Assert.True(result.Generators["g2"].IsValid);
			Assert.False(result.Generators["g3"].IsValid);
		}
	}
}