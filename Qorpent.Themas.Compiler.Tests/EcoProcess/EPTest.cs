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
// Original file : EPTest.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using Qorpent.Log;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests.EcoProcess {
	public class EPTest {
		public static ThemaCompilerContext Compile(string code, Action<ThemaProject> prepareProject = null) {
			var project = new ThemaProject {ErrorLevel = ErrorLevel.Error, UseEcoOptimization = true, UseEcoProcess = true};
			project.SetSelfLog(LogLevel.Debug);
			project.NonResolvedProcessThemaRefIsError = true;
			project.CustomCompiler = new ApplyEcoProcesses();
			project.DirectSource["main.bxl"] = code;
			if (null != prepareProject) {
				prepareProject(project);
			}
			var compiler = new ThemaCompiler();

			var context = compiler.Compile(project);
			Console.WriteLine(project.GetLog());
			foreach (var e in context.Errors) {
				Console.WriteLine(e);
			}
			return context;
		}
	}
}