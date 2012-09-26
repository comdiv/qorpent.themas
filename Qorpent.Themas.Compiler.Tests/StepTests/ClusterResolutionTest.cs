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
// Original file : ClusterResolutionTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using NUnit.Framework;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests.StepTests {
	[TestFixture]
	public class ClusterResolutionTest : ThemaCompilerTestBase {
		private const string code =
			@"thema nocluster , cluster=''  # cleanup cluster
thema cluster_def
thema cluster_def_a , +cluster='A'
cluster_def_a cluster_ab , +cluster='B', -cluster='DEFAULT'
";

		private ThemaCompilerContext exec(params string[] clusters) {
			var proj = new ThemaProject();
			proj.DirectSource["main.bxl"] = code;
			proj.SetSelfLog();
			proj.CustomCompiler = new GenerateXml();
			if (null != clusters) {
				foreach (var cluster in clusters) {
					proj.Clusters.Add(cluster);
				}
			}
			var result = new ThemaCompiler().Compile(proj);
			foreach (var t in result.Themas) {
				Console.WriteLine(t.Value.Xml);
			}
			return result;
		}

		[Test]
		public void a_cluster() {
			var result = exec("A");
			Assert.AreEqual(2, result.Themas.Count);
			Assert.True(result.Themas.ContainsKey("cluster_def_a"));
			Assert.True(result.Themas.ContainsKey("cluster_ab"));
		}

		[Test]
		public void b_cluster() {
			var result = exec("B");
			Assert.AreEqual(1, result.Themas.Count);
			Assert.True(result.Themas.ContainsKey("cluster_ab"));
		}

		[Test]
		public void default_cluster() {
			var result = exec("DEFAULT");
			Assert.AreEqual(2, result.Themas.Count);
			Assert.True(result.Themas.ContainsKey("cluster_def"));
			Assert.True(result.Themas.ContainsKey("cluster_def_a"));
		}

		[Test]
		public void no_cluster__all_themas_exists() {
			var result = exec();
			Assert.AreEqual(4, result.Themas.Count);
		}
	}
}