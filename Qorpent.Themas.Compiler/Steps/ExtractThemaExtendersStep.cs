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
// Original file : ExtractThemaExtendersStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Extracts extenders of thema
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ExtractThemaExtendersStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var t in Context.Themas) {
				var td = t.Value;
				foreach (var e in t.Value.Fullsource.Elements("extend").ToArray()) {
					td.IsExtension = true;
					var code = e.Id();
					if (Context.Themas.ContainsKey(code)) {
						Context.Themas[code].Imports.Add(td.Code);
					}
					else {
						if (Context.Project.NonResolvedImportIsError) {
							UserLog.Error("thema " + td.Code + " extends non existed thema " + code);
							AddError(ErrorLevel.Error, "thema " + td.Code + " extends non existed thema " + code, "TE1301", null,
							         td.File,
							         td.Line);
							return;
						}
						UserLog.Warn("thema " + td.Code + " extends non existed thema " + code);
						AddError(ErrorLevel.Warning, "thema " + td.Code + " extends non existed thema " + code, "TW1301", null,
						         td.File,
						         td.Line);
					}

					e.Remove();
				}
			}
		}
	}
}