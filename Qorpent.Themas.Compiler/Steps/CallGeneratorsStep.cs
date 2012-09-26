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
// Original file : CallGeneratorsStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	generates dynamic content with custom generators
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class CallGeneratorsStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var td in Context.Themas.Values) {
				var tc = td.Code;
				foreach (var i in td.Items) {
					var ic = i.Key;
					var ie = i.Value;
					ChecksCallsInElement(ie, tc, ic);
				}
				ChecksCallsInElement(td.Fullsource, tc, "src");
			}
		}

		/// <summary>
		/// 	Checkses the calls in element.
		/// </summary>
		/// <param name="ie"> The ie. </param>
		/// <param name="tc"> The tc. </param>
		/// <param name="ic"> The ic. </param>
		/// <remarks>
		/// </remarks>
		private void ChecksCallsInElement(XContainer ie, string tc, string ic) {
			foreach (var e in ie.Elements("call").ToArray()) {
				var code = e.Id();
				if (!Context.Generators.ContainsKey(code)) {
					continue;
				}

				var gen = Context.Generators[code];
				if (gen.IsValid) {
					gen.Execute(Context, e);
					UserLog.Trace("generator " + code + " called in " + tc + "/" + ic + " " + e.Describe().File + ":" +
					              e.Describe().Line);
				}
				else {
					var message = "try call not valid generator with code " + code + " in " + tc + "/" + ic;
					AddError(
						ErrorLevel.Warning,
						message,
						"TW2501",
						null, e.Describe().File, e.Describe().Line
						);
					UserLog.Warn(message);
				}
			}
		}
	}
}