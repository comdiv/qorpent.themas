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
// Original file : EmbedSubsetsStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	resolves addcols elements in thema items
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class EmbedSubsetsStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var t in Context.Themas.Values) {
				foreach (var item in t.Items) {
					var i = item.Value;
					var c = item.Key;
					ResolveUseSets(t, i, c);
				}
			}
			if (!Context.Project.AnalyzeSubsetUsage) {
				return;
			}
			foreach (var subset in Context.SubsetIndex.Values) {
				if (null != subset.Annotation<UsedInWorkingThemaAnnotation>()) {
					continue;
				}
				var message = "subset " + subset.Describe().Id + " '" + subset.Describe().Name +
				              "' not used in any working thema";
				UserLog.Warn(message);
				AddError(ErrorLevel.Hint,
				         message, "TW2102", null,
				         subset.Describe().File,
				         subset.Describe().Line
					);
			}
		}

		/// <summary>
		/// 	Resolves the use sets.
		/// </summary>
		/// <param name="t"> The t. </param>
		/// <param name="i"> The i. </param>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void ResolveUseSets(ThemaDescriptor t, XElement i, string c) {
			foreach (var ac in i.Elements(Context.Project.ImportSubsetElementName).ToArray()) {
				var code = ac.Id();
				if (null != t) {
					code = t.Substitute(code, c, true);
				}
				if (code.IsEmpty()) {
					continue;
				}
				if (Context.SubsetIndex.ContainsKey(code)) {
					ac.ReplaceWith(ResolveSubset(code));
				}
				else {
					var message = "item " + (null == t ? i.Id() : t.Code) + "/" + c + " references non-existed subset " + code;
					if (Context.Project.NonResolvedSubsetIsError) {
						AddError(
							ErrorLevel.Error,
							message,
							"TE2101",
							null,
							i.Describe().File,
							i.Describe().Line
							);
						UserLog.Error(message);
						return;
					}
					AddError(
						ErrorLevel.Warning,
						message,
						"TW2101",
						null,
						i.Describe().File,
						i.Describe().Line
						);
					UserLog.Warn(message);
					ac.Remove();
				}
			}
		}

		/// <summary>
		/// 	Resolves the subset.
		/// </summary>
		/// <param name="code"> The code. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private object[] ResolveSubset(string code) {
			var subset = Context.SubsetIndex[code];
			ResolveUseSets(null, subset, "definition");
			if (null == subset.Annotation<UsedInWorkingThemaAnnotation>()) {
				subset.AddAnnotation(UsedInWorkingThemaAnnotation.Default);
			}
			return subset.Nodes().ToArray();
		}
	}
}