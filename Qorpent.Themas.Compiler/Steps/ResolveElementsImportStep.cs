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
// Original file : ResolveElementsImportStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Resolves all elements,sets and extensions to 'imported' dictionaries (not in resolved)
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ResolveElementsImportStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var thema in Context.Themas.Values) {
				ResolveElements(thema);
			}
		}

		/// <summary>
		/// 	Resolves the elements.
		/// </summary>
		/// <param name="thema"> The thema. </param>
		/// <remarks>
		/// </remarks>
		private void ResolveElements(ThemaDescriptor thema) {
			if (thema.ElementsResolved) {
				return;
			}
			foreach (var importthema in thema.Imports.Select(import => Context.Themas[import])) {
				ResolveElements(importthema);
				foreach (var rp in importthema.ImportedThemaItems
					.Where(rp => !thema.SelfThemaItems.ContainsKey(rp.Key))) {
					thema.ImportedThemaItems[rp.Key] = rp.Value;
				}
				foreach (var rp in importthema.SelfThemaItems
					.Where(rp => !thema.SelfThemaItems.ContainsKey(rp.Key))) {
					thema.ImportedThemaItems[rp.Key] = rp.Value;
				}
				foreach (var rp in importthema.ImportedThemaItemsSets
					.Where(rp => !thema.SelfThemaItems.ContainsKey(rp.Key))
					.Where(rp => !thema.SelfThemaItemsSets.ContainsKey(rp.Key))) {
					thema.ImportedThemaItemsSets[rp.Key] = rp.Value;
				}
				foreach (var rp in importthema.SelfThemaItemsSets
					.Where(rp => !thema.SelfThemaItems.ContainsKey(rp.Key))
					.Where(rp => !thema.SelfThemaItemsSets.ContainsKey(rp.Key))) {
					thema.ImportedThemaItemsSets[rp.Key] = rp.Value;
				}
				foreach (var rp in importthema.ImportedThemaItemsExtensions
					.Where(rp => !thema.SelfThemaItems.ContainsKey(rp.Key))
					.Where(rp => !thema.SelfThemaItemsSets.ContainsKey(rp.Key))) {
					if (!thema.ImportedThemaItemsExtensions.ContainsKey(rp.Key)) {
						thema.ImportedThemaItemsExtensions[rp.Key] = new List<XElement>();
					}
					foreach (var e in rp.Value) {
						thema.ImportedThemaItemsExtensions[rp.Key].Add(e);
					}
				}
				foreach (var rp in importthema.SelfThemaItemsExtensions
					.Where(rp => !thema.SelfThemaItems.ContainsKey(rp.Key))
					.Where(rp => !thema.SelfThemaItemsSets.ContainsKey(rp.Key))) {
					if (!thema.ImportedThemaItemsExtensions.ContainsKey(rp.Key)) {
						thema.ImportedThemaItemsExtensions[rp.Key] = new List<XElement>();
					}
					thema.ImportedThemaItemsExtensions[rp.Key].Add(rp.Value);
				}
			}
			if (thema.IsGeneric) {
				Genericdict(thema, thema.ImportedThemaItems);
				Genericdict(thema, thema.ImportedThemaItemsSets);
				Genericdict(thema, thema.ImportedThemaItemsExtensions);
				Genericdict(thema, thema.SelfThemaItems);
				Genericdict(thema, thema.SelfThemaItemsSets);
				Genericdict(thema, thema.SelfThemaItemsExtensions);
			}
			thema.ElementsResolved = true;
		}

		/// <summary>
		/// 	Genericdicts the specified thema.
		/// </summary>
		/// <typeparam name="TV"> The type of the V. </typeparam>
		/// <param name="thema"> The thema. </param>
		/// <param name="dict"> The dict. </param>
		/// <remarks>
		/// </remarks>
		private static void Genericdict<TV>(ThemaDescriptor thema, IDictionary<string, TV> dict) {
			foreach (var elementCode in dict.Keys.ToArray()) {
				if (!elementCode.Contains("..")) {
					continue;
				}
				var convertedCode = elementCode;
				if (convertedCode.StartsWith("_.")) {
					convertedCode = convertedCode.Substring(1);
				}
				var v = dict[elementCode];
				dict.Remove(elementCode);
				var index = thema.Code;
				if (thema.ResolvedParameters.ContainsKey("index")) {
					index = thema.ResolvedParameters["index"];
				}
				dict[convertedCode.Replace("..", index + ".")] = v;
			}
		}
	}
}