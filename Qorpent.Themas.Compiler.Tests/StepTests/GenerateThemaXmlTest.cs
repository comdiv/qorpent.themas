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
// Original file : GenerateThemaXmlTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Themas.Compiler.Pipelines;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class GenerateThemaXmlTest : ThemaCompilerTestBase {
		[Test]
		public void test_links_are_generated() {
			var result = execute<GenerateXml>(new miniproj(), LogLevel.All);

			Console.WriteLine(result.Themas["lchild1"].Xml);
			Console.WriteLine(result.Themas["lparent"].Xml);
			Console.WriteLine(result.Themas["lchild4"].Xml);

			var l = result.Themas["lchild1"].Xml.Element("link");
			var lv = result.Themas["lchild3"].Xml.Element("link");
			var bl = result.Themas["lparent"].Xml.Element("backlink");
			Assert.NotNull(l);
			Assert.NotNull(bl);
			Assert.AreEqual("parent", l.Attr("type"));
			Assert.AreEqual("lparent", l.Attr("target"));
			Assert.AreEqual("parent", l.Attr("type"));
			Assert.AreEqual("lchild1", bl.Attr("source"));
			Assert.AreEqual("myval", lv.Attr("value"));
		}

		[Test]
		public void test_xml() {
			var result = execute<GenerateXml>(new miniproj(), LogLevel.All);
			Console.WriteLine(result.Themas["testxml"].Xml);
			Assert.AreEqual(
				@"<thema id=""testxml"" code=""testxml"" cluster=""DEFAULT"" _file=""/tfolder1/base1.bxl"" _line=""16"" fileidx=""110"" role=""ADMIN"">
  <out _file=""/tfolder1/base1.bxl"" _line=""18"" code=""A.out"" id=""A.out"">
    <col _file=""/tfolder1/base1.bxl"" _line=""39"" code=""a"" id=""a"" name=""testxml_"" evidence=""colset: ua  at /tfolder1/base1.bxl(38:0)"" />
    <var _file=""/tfolder1/base1.bxl"" _line=""40"" code=""Z"" id=""Z"" evidence=""colset: ua  at /tfolder1/base1.bxl(38:0)"">z</var>
  </out>
  <out _file=""/tfolder1/base1.bxl"" _line=""20"" code=""B.out"" id=""B.out"">
    <var _file=""/tfolder1/base1.bxl"" _line=""21"" code=""Z"" id=""Z"">z</var>
    <ctg step=""29"" />
  </out>
  <extradata _file=""/tfolder1/base1.bxl"" _line=""23"" code=""x"" id=""x"">
    <a _file=""/tfolder1/base1.bxl"" _line=""24"" x=""1"" />
  </extradata>
</thema>",
				result.Themas["testxml"].Xml.ToString());
		}

		[Test]
		public void thema_extra_data_prepared() {
			var result = execute<GenerateXml>(new miniproj(), LogLevel.All);
			var ch4 = result.Themas["lchild4"];
			Assert.NotNull(ch4.ExtraData);
			Assert.AreEqual(
				@"<extra>
  <this _file=""/tfolder1/base1.bxl"" _line=""133"" code=""is"" id=""is"" name=""extra"" data=""1"" />
</extra>",
				ch4.ExtraData.ToString());
		}
	}
}