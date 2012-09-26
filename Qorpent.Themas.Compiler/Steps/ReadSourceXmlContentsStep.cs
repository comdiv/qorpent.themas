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
// Original file : ReadSourceXmlContentsStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.Bxl;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Prepare xml representation of source files in context.SourceFileXml
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ReadSourceXmlContentsStep : ThemaCompilerStep {
		/// <summary>
		/// 	creates new instance of step
		/// </summary>
		/// <remarks>
		/// </remarks>
		public ReadSourceXmlContentsStep() {
			_bxl = Application.Current.Bxl.GetParser();
		}

		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var file in Context.SourceFileData) {
				var ext = Path.GetExtension(file.Key);
				if (null == ext) {
					throw new Exception("invalid null extension");
				}
				var n = Context.LocalFileNames[file.Key];
				if (ext == ".xml") {
					Context.SourceFileXml[file.Key] = XElement.Parse(file.Value);
					continue;
				}
				if (ext.Contains(".bxl")) {
					try {
						Context.SourceFileXml[file.Key] = _bxl.Parse(file.Value, Context.LocalFileNames[file.Key]);
					}
					catch (BxlException e) {
						AddError(ErrorLevel.Error, e.Message, "TE0210", e, e.LexInfo.File, e.LexInfo.Line, e.LexInfo.Column);
					}
					catch (Exception e) {
						AddError(ErrorLevel.Error, e.Message, "TE0212", e);
					}
					continue;
				}
				AddError(ErrorLevel.Warning, "unknown extension " + ext, "TW0201", null, n);
			}
		}

		/// <summary>
		/// </summary>
		private readonly IBxlParser _bxl;
	}
}