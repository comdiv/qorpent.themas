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
// Original file : RegisterGeneratorsStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Extract "register" elements with "compile" attribute and set them into context.Generators dictionary
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class RegisterGeneratorsStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var sf in Context.SourceFileXml.Values) {
				foreach (var e in sf.Elements("register").ToArray()) {
					if (null == e.Attribute("compile")) {
						continue;
					}

					var code = e.Id();
					var type = e.Describe().Name;

					var gendesc = new GeneratorDescriptor(code, type);
					Context.Generators[code] = gendesc;
					try {
						gendesc.PrepareType();
					}
					catch (Exception ex) {
						var message = "error processing generator regestering of " + code + ":" + type;
						if (Context.Project.NonResolvedGeneratorIsError) {
							AddError(ErrorLevel.Error, message, "TE0201", ex, e.Describe().File, e.Describe().Line);
							UserLog.Error(message);
							return;
						}
						AddError(ErrorLevel.Warning, message, "TW0201", ex, e.Describe().File, e.Describe().Line);
						UserLog.Warn(message);
					}
				}
			}
		}
	}
}