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
// Original file : EmbedParametersStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using Qorpent.Utils;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Embeds all thema parameters in thema's elements
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class EmbedParametersStep : CalculateParametersStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var t in Context.Themas.Values) {
				foreach (var e in t.Items) {
					var t1 = t;
					var e1 = e;
					VisitTextAndAttributes(e.Value, (x, v) => Doreplace(t1, v, e1.Key));
					foreach (var p in e.Value.Elements("var").Union(e.Value.Elements("param")).ToArray()) {
						var list = p.Attribute("list");
						var pluslist = p.Attribute("__PLUS__list");
						var minuslist = p.Attribute("__MINUS__list");
						if (null != pluslist && null != list) {
							list.Value = ComplexStringHelper.Set(list.Value, pluslist.Value);
						}
						if (null != minuslist && null != list) {
							list.Value = ComplexStringHelper.Remove(list.Value, minuslist.Value);
						}
						if (null != pluslist) {
							pluslist.Remove();
						}
						if (null != minuslist) {
							minuslist.Remove();
						}
					}
				}
			}
		}

		/// <summary>
		/// 	Resolves the specified td.
		/// </summary>
		/// <param name="td"> The td. </param>
		/// <param name="key"> The key. </param>
		/// <param name="src"> The SRC. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected override string Resolve(ThemaDescriptor td, string key, string src = null) {
			return td.GetParam(key, src);
		}
	}
}