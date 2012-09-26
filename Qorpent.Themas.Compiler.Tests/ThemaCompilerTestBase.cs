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
// Original file : ThemaCompilerTestBase.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using Qorpent.Log;
using Qorpent.Themas.Compiler.Pipelines;

namespace Qorpent.Themas.Compiler.Tests {
	public class ThemaCompilerTestBase {
		protected ThemaCompilerContext execute<T>(ThemaProject proj = null, LogLevel level = LogLevel.Trace)
			where T : IThemaCompilerSetup, new() {
			sw = new StringWriter();
			proj = proj ?? getDefaultProject();
			proj.UserLog = proj.UserLog ?? BaseTextWriterLogWriter.CreateLog("default", sw, level);
			proj.CustomCompiler = proj.CustomCompiler ?? new T();
			var compiler = new ThemaCompiler();
			var result = compiler.Compile(proj);
			Console.WriteLine(sw.ToString());
			foreach (var e in result.Errors) {
				Console.WriteLine(e);
			}
			return result;
		}

		protected virtual ThemaProject getDefaultProject() {
			return new TestProject();
		}

		public ThemaCompilerContext execute() {
			return execute<GenerateXml>(getDefaultProject(), LogLevel.All);
		}

		protected StringWriter sw;
	}
}