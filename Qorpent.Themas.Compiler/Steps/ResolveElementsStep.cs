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
// Original file : ResolveElementsStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using System.Xml.Linq;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	resolves self and imported elements and their extensions to single XElement
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ResolveElementsStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var t in Context.Themas.Values) {
				foreach (var item in t.ImportedThemaItems.Union(t.SelfThemaItems)) {
					var key = item.Key;
					var x = new XElement(item.Value);
					XElement s = null;
					if (t.SelfThemaItemsSets.ContainsKey(key)) {
						s = t.SelfThemaItemsSets[key];
					}
					else if (t.ImportedThemaItemsSets.ContainsKey(key)) {
						s = t.ImportedThemaItemsSets[key];
					}
					if (null != s) {
						x.RemoveNodes();
						x.Add(s.Nodes());
						foreach (var a in s.Attributes()) {
							x.SetAttributeValue(a.Name, a.Value);
						}
					}
					if (t.ImportedThemaItemsExtensions.ContainsKey(key)) {
						foreach (var e in t.ImportedThemaItemsExtensions[key]) {
							x.Add(e.Nodes());
							foreach (var a in e.Attributes()) {
								x.SetAttributeValue(a.Name, a.Value);
							}
						}
					}
					if (t.SelfThemaItemsExtensions.ContainsKey(key)) {
						x.Add(t.SelfThemaItemsExtensions[key].Nodes());
						foreach (var a in t.SelfThemaItemsExtensions[key].Attributes()) {
							x.SetAttributeValue(a.Name, a.Value);
						}
					}
					t.Items[key] = x;
				}
			}
		}
	}
}