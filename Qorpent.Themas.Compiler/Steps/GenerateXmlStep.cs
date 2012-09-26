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
// Original file : GenerateXmlStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Generates XML representation of each thema
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class GenerateXmlStep : ThemaCompilerStep {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="GenerateXmlStep" /> class.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public GenerateXmlStep() {}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="GenerateXmlStep" /> class.
		/// </summary>
		/// <param name="ctx"> The CTX. </param>
		/// <remarks>
		/// </remarks>
		public GenerateXmlStep(ThemaCompilerContext ctx) {
			Context = ctx;
		}

		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var t in Context.Themas.Values) {
				t.Xml = Generate(t);
				if (t.Fullsource.Elements().Any()) {
					t.ExtraData = new XElement(t.Fullsource) {Name = "extra"};
				}
			}
			var extra = new XElement("extra");
			foreach (var e in Context.SourceFileXml.Values) {
				extra.Add(e.Elements());
			}
			Context.ExtraData = extra;
		}

		/// <summary>
		/// 	Generates the specified t.
		/// </summary>
		/// <param name="t"> The t. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public XElement Generate(ThemaDescriptor t) {
			var result = new XElement("thema");
			foreach (var p in SaveAbleParameterKeys(t)) {
				result.Add(new XAttribute(p, t.ResolvedParameters[p]));
			}
			var imports = t.Imports.ConcatString();
			if (imports.IsNotEmpty()) {
				result.Add(new XElement("imports", t.Imports.ConcatString()));
			}
			XElement tmp;
			foreach (var link in t.Links) {
				result.Add(tmp = new XElement("link",
				                              new XAttribute("type", link.Type.Code),
				                              new XAttribute("target", link.TargetCode)
					                 ));
				if (link.Value.IsNotEmpty()) {
					tmp.Add(new XAttribute("value", link.Value));
				}
			}
			foreach (var link in t.BackLinks) {
				result.Add(tmp = new XElement("backlink",
				                              new XAttribute("type", link.Type.Code),
				                              new XAttribute("source", link.SourceCode)
					                 ));
				if (link.Value.IsNotEmpty()) {
					tmp.Add(new XAttribute("value", link.Value));
				}
			}
			foreach (var i in t.Items) {
				i.Value.SetAttributeValue("code", i.Key);
				i.Value.SetAttributeValue("id", i.Key);
				result.Add(i.Value);
			}
			result.Add(t.Fullsource.Elements());
			return result;
		}

		/// <summary>
		/// 	Saves the able parameter keys.
		/// </summary>
		/// <param name="t"> The t. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private IEnumerable<string> SaveAbleParameterKeys(ThemaDescriptor t) {
			return t.ResolvedParameters.Keys
				.OrderBy(x =>
					{
						if (Context.Project.AttributeOrder.Contains(x)) {
							return
								Context.Project.AttributeOrder.IndexOf(x).ToString("0000");
						}
						return "ZZZ_" + x;
					}).ToArray()
				.Where(p => null == Context.Project.NonSaveParameters ||
				            null == Context.Project.NonSaveParameters.FirstOrDefault(
					            x => Regex.IsMatch(p, x, RegexOptions.Compiled)));
		}
	}
}