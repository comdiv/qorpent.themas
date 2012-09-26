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
// Original file : ExtractGlobalsStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using System.Xml.Linq;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Find and stores in context.Globals , excluding from source files
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ExtractGlobalsStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			if (Context.Project.Options.ContainsKey("no-extract-globals")) {
				return;
			}
			foreach (var file in Context.SourceFiles) {
				if (!Context.SourceFileXml.ContainsKey(file)) {
					continue;
				}
				var xml = Context.SourceFileXml[file];
				ExtractGlobalElements(file, xml);
				if (-1 == Context.SourceFileData[file].IndexOf('~')) {
					continue;
				}
				UserLog.Trace("file " + Context.LocalFileNames[file] + " marked to proceed with globals");
				xml.AddAnnotation(HaveToBeProcessedWithGlobalsAnnotation.Default);
			}
		}

		/// <summary>
		/// 	Extracts the global elements.
		/// </summary>
		/// <param name="file"> The file. </param>
		/// <param name="xml"> The XML. </param>
		/// <remarks>
		/// </remarks>
		private void ExtractGlobalElements(string file, XContainer xml) {
			foreach (var e in xml.Elements("global").ToArray()) {
				var code = e.Id().ToUpper();
				var nameattr = e.Attribute("name");
				var value = null == nameattr ? e.Value : nameattr.Value;
				if (Context.Project.UserLog.Level <= LogLevel.Trace) {
					var existed = Context.Globals.ContainsKey(code);
					if (existed) {
						UserLog.Trace("global " + code + " RECREATED from (" + Context.LocalFileNames[file] + ")");
					}
					else {
						UserLog.Debug("global " + code + " created from (" + Context.LocalFileNames[file] + ")");
					}
				}
				Context.Globals[code] = value;


				e.Remove();
			}
		}
	}
}