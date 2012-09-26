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
// Original file : SetupSourceFilesInValidOrderStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Linq;
using Qorpent.Applications;
using Qorpent.IO;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Enumerate all source file dirs,lists and masks and
	/// 	generate full list of full paths to source files
	/// 	in compiler context
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class SetupSourceFilesInValidOrderStep : ThemaCompilerStep {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="SetupSourceFilesInValidOrderStep" /> class.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public SetupSourceFilesInValidOrderStep() {
			_resolver = Application.Current.Files.GetResolver();
		}

		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			if (Context.Project.DirectSource.Count != 0) {
				foreach (var s in Context.Project.DirectSource.Keys) {
					Context.SourceFiles.Add(s);
					Context.LocalFileNames[s] = s;
				}
				return;
			}
			_currentresolver = Context.Project.Resolver ?? _resolver;
			foreach (var src in Context.Project.SourceFiles) {
				//file mask
				UserLog.Debug("start lookup: " + src);
				var ext = Path.GetExtension(src);
				if (null == ext) {
					throw new Exception("invalid null extension");
				}
				if (src.Contains("*")) {
					var dir = Path.GetDirectoryName(src);

					var mask = Path.GetFileName(src);
					var files =
						_currentresolver.ResolveAll(new FileSearchQuery
							{
								ExistedOnly = true,
								PathType = FileSearchResultType.FullPath,
								ProbePaths = new[] {dir},
								ProbeFiles = new[] {mask}
							}).OrderBy(
								x => x);
					foreach (var file in files) {
						UserLog.Debug("add file (1): " + file);
						Add(file);
					}
				}
				else if (ext.Length > 0 && ext.Length <= 5) {
					// direct file
					var file = _currentresolver.Resolve(src);
					UserLog.Debug("add file (2): " + file);
					Add(file);
				}
				else {
					//folder
					var dir = _currentresolver.Resolve(src);
					var files = Directory.GetFiles(dir, "*.bxl").OrderBy(x => x);
					foreach (var file in files) {
						UserLog.Debug("add file (3): " + file);
						Add(file);
					}
				}
			}
		}

		/// <summary>
		/// 	Adds the specified file.
		/// </summary>
		/// <param name="file"> The file. </param>
		/// <remarks>
		/// </remarks>
		private void Add(string file) {
			file = file.Replace("\\", "/").ToLower();
			Context.SourceFiles.Add(file);
			var tf = file;
			var root = _currentresolver.Resolve("~/");
			tf = tf.Replace(root, "");
			Context.LocalFileNames[file] = tf;
			UserLog.Trace("src file " + tf + " added");
		}

		/// <summary>
		/// </summary>
		private readonly IFileNameResolver _resolver;

		/// <summary>
		/// </summary>
		private IFileNameResolver _currentresolver;
	}
}