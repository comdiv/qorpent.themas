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
// Original file : ReadSourceFileContentsStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.IO;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	reads raw content of source files in context.SourceFileData
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ReadSourceFileContentsStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			//direct content mode
			if (Context.Project.DirectSource.Count > 0) {
				foreach (var file in Context.SourceFiles) {
					Context.SourceFileData[file] = Context.Project.DirectSource[file];
				}
				return;
			}
			//file content mode
			foreach (var file in Context.SourceFiles) {
				Context.SourceFileData[file] = File.ReadAllText(file);
			}
		}
	}
}