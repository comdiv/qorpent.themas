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
// Original file : ExtractThemaSelfItemsStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Extracts well-known thema parts/sets and extensions
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ExtractThemaSelfItemsStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			SetupElementIndexes();
			foreach (var t in Context.Themas) {
				var td = t.Value;
				foreach (var e in t.Value.Fullsource.Elements().ToArray()) {
					var n = e.Name.LocalName;
					if (!(_selfItems.ContainsKey(n) || _selfItemSets.ContainsKey(n) || _selfItemExtensions.ContainsKey(n))) {
						continue;
					}
					var code = e.Id();
					if (code.IsEmpty()) {
						continue;
					}

					if (_selfItems.ContainsKey(n)) {
						code = code + "." + _selfItems[n];
						td.SelfThemaItems[code] = e;
					}
					else if (_selfItemSets.ContainsKey(n)) {
						code = code + "." + _selfItemSets[n];
						td.SelfThemaItemsSets[code] = e;
					}
					else if (_selfItemExtensions.ContainsKey(n)) {
						code = code + "." + _selfItemExtensions[n];
						td.SelfThemaItemsExtensions[code] = e;
					}
					UserLog.Debug("main item in " + td.Code + " of type " + n + " with code " + code + " added");
					e.Remove();
				}
			}
		}

		/// <summary>
		/// 	Setups the element indexes.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void SetupElementIndexes() {
			_selfItems = new Dictionary<string, string>();
			_selfItemSets = new Dictionary<string, string>();
			_selfItemExtensions = new Dictionary<string, string>();
			foreach (var s in Context.Project.ItemElements.Select(ie => ie.Split(':'))) {
				_selfItems[s[0]] = s[1];
			}
			foreach (var s in Context.Project.ItemSetElements.Select(ie => ie.Split(':'))) {
				_selfItemSets[s[0]] = s[1];
			}
			foreach (var s in Context.Project.ItemExtensionElements.Select(ie => ie.Split(':'))) {
				_selfItemExtensions[s[0]] = s[1];
			}
		}

		/// <summary>
		/// </summary>
		private Dictionary<string, string> _selfItemExtensions;

		/// <summary>
		/// </summary>
		private Dictionary<string, string> _selfItemSets;

		/// <summary>
		/// </summary>
		private Dictionary<string, string> _selfItems;
	}
}