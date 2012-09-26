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
// Original file : CompilerConsoleApplication.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.Applications;

namespace Qorpent.Themas.Compiler.CompilerApp {
	/// <summary>
	/// </summary>
	public class CompilerConsoleApplication {
		/// <summary>
		/// </summary>
		/// <param name="args"> </param>
		/// <exception cref="Exception"></exception>
		public void Run(string[] args) {
			//	Contract.Requires<ArgumentException>(args!=null && args.Length!=0);
			var project = new ThemaProject();
			var projfile = Path.GetFullPath(args[0]);
			var dir = Path.GetDirectoryName(projfile);
			if (null == dir) {
				throw new Exception("не могу определить рабочую директорию");
			}
			Environment.CurrentDirectory = dir;
			var projfilexml = Application.Current.Bxl.Parse(File.ReadAllText(projfile), Path.GetFileName(projfile));
			IList<string> targets = new List<string>();
			if (args.Length > 1) {
				for (var i = 1; i < args.Length; i++) {
					targets.Add(args[i]);
				}
			}
			project.ConfigureFromXml(projfilexml, string.Join(",", targets.ToArray()));

			Console.WriteLine("Project loaded from " + projfile);
			var compiler = new ThemaCompiler();
			Console.WriteLine("Start of compilation");
			Result = compiler.Compile(project);

			if (ErrorLevel.Warning <= project.ErrorLevel) {
				Console.ForegroundColor = ConsoleColor.Yellow;
				foreach (var warn in Result.Errors.Where(x => x.Level <= ErrorLevel.Warning)) {
					Console.WriteLine(warn);
				}
				Console.ResetColor();
			}
			Console.ForegroundColor = ConsoleColor.Red;
			foreach (var error in Result.Errors.Where(x => x.Level > ErrorLevel.Warning)) {
				Console.WriteLine(error);
			}
			Console.ResetColor();

			if (Result.IsComplete) {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("compilation complete");
			}
			else {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("compilation failed!");
			}
			Console.ResetColor();
		}

		/// <summary>
		/// </summary>
		public ThemaCompilerContext Result;
	}
}