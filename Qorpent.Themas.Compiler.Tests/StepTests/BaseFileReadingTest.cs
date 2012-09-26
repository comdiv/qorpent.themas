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
// Original file : BaseFileReadingTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Qorpent.Applications;
using Qorpent.Bxl;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class BaseFileReadingTest : ThemaCompilerTestBase {
		private readonly IBxlParser bxl = Application.Current.Bxl.GetParser();

		private string[] getfiles() {
			var dir1 = Path.GetFullPath("tfolder1");
			var dir2 = Path.GetFullPath("tfolder2");
			var files1 =
				Directory.GetFiles(dir1, "*.bxl").OrderBy(x => x).Select(x => x.Replace("\\", "/").ToLower());
			var files2 =
				Directory.GetFiles(dir2, "*.bxl").OrderBy(x => x).Select(x => x.Replace("\\", "/").ToLower());
			return files1.Union(files2).ToArray();
		}

		[Test]
		public void default_folder_file_set_test() {
			var result = execute<SourceFileList>();
			var files = getfiles();
			var resultfiles = result.SourceFiles.ToArray();
			CollectionAssert.AreEqual(files, resultfiles);
			Assert.True(result.LocalFileNames.ContainsKey(files[0]));
			var tf = files[0].Replace(Environment.CurrentDirectory.ToLower().Replace("\\", "/"), "");
			Assert.AreEqual(tf, result.LocalFileNames[files[0]]);
		}

		[Test]
		public void read_source_data() {
			var result = execute<ReadSourceData>();
			var files = getfiles();
			foreach (var file in files) {
				Assert.True(result.SourceFileData.ContainsKey(file));
				Assert.AreEqual(File.ReadAllText(file), result.SourceFileData[file]);
			}
		}

/*
		[Test]
		public void xml_source_data() {
			var result = execute<ReadXmlData>();
			var files = getfiles();
			foreach (var file in files) {
				Assert.True(result.SourceFileData.ContainsKey(file));
				var differ =
					new XDiffExecutor(
						bxl.Parse(File.ReadAllText(file), file.Replace(Environment.CurrentDirectory.ToLower().Replace("\\", "/"), "")),
						result.SourceFileXml[file]);
				var diff = differ.Execute();
				Assert.AreEqual(0, diff.Length);
			}
		}*/
	}
}