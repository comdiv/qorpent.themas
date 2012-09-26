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
// Original file : ExtractSubsetsStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Extracts well-known elements subset to context.SubsetIndex dictionary
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ExtractSubsetsStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var file in Context.SourceFiles) {
				if (!Context.SourceFileXml.ContainsKey(file)) {
					continue;
				}
				var xml = Context.SourceFileXml[file];
				foreach (var e in xml.Elements(Context.Project.SubsetElementName).ToArray()) {
					var code = e.Id();

					if (Context.Project.UserLog.Level <= LogLevel.Trace) {
						var existed = Context.SubsetIndex.ContainsKey(code);
						if (existed) {
							UserLog.Trace("subset " + code + " RECREATED from (" + Context.LocalFileNames[file] + ")");
						}
						else {
							UserLog.Debug("subset " + code + " created from (" + Context.LocalFileNames[file] + ")");
						}
					}
					foreach (var subsetElement in e.Elements()) {
						subsetElement.SetAttributeValue("evidence", "colset: " + code + " " + e.Describe().ToWhereString());
					}
					Context.SubsetIndex[code] = e;


					e.Remove();
				}
			}
		}
	}
}