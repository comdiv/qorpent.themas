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
// Original file : EmbedItemParametersStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Embeds item parameters into element
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class EmbedItemParametersStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var t in Context.Themas) {
				var tc = t.Key;
				var td = t.Value;
				foreach (var i in td.Items) {
					var ic = i.Key;
					var iv = i.Value;
					foreach (var pr in iv.Elements("ask").Union(iv.Elements("use")).ToArray()) {
						EmbedParameter(ic, pr, tc, t.Value);
					}
				}
			}
			if (!Context.Project.AnalyzeParamUsage) {
				return;
			}
			foreach (var parameter in Context.ParameterIndex.Values) {
				if (null != parameter.Annotation<UsedInWorkingThemaAnnotation>()) {
					continue;
				}
				var message = "parameter " + parameter.Describe().Id + " '" + parameter.Describe().Name +
				              "' not used in any working thema";
				UserLog.Warn(message);
				AddError(ErrorLevel.Hint,
				         message, "TW2202", null,
				         parameter.Describe().File,
				         parameter.Describe().Line
					);
			}
		}

		/// <summary>
		/// 	Embeds the parameter.
		/// </summary>
		/// <param name="ic"> The ic. </param>
		/// <param name="pr"> The pr. </param>
		/// <param name="tc"> The tc. </param>
		/// <param name="t"> The t. </param>
		/// <remarks>
		/// </remarks>
		private void EmbedParameter(string ic, XElement pr, string tc, ThemaDescriptor t) {
			var code = t.Substitute(pr.Id());
			if (Context.ParameterIndex.ContainsKey(code)) {
				ReplaceParameter(pr, code);
			}
			else {
				var message = "item " + tc + "/" + ic + " references non-existed parameter " + code;
				if (Context.Project.NonResolvedParameterIsError) {
					AddError(
						ErrorLevel.Error,
						message,
						"TE2201",
						null,
						pr.Describe().File,
						pr.Describe().Line
						);
					UserLog.Error(message);
					return;
				}
				AddError(
					ErrorLevel.Warning,
					message,
					"TW2201",
					null,
					pr.Describe().File,
					pr.Describe().Line
					);
				UserLog.Warn(message);
				pr.Remove();
			}
		}

		/// <summary>
		/// 	Replaces the parameter.
		/// </summary>
		/// <param name="pr"> The pr. </param>
		/// <param name="code"> The code. </param>
		/// <remarks>
		/// </remarks>
		private void ReplaceParameter(XElement pr, string code) {
			var p = Context.ParameterIndex[code];
			if (null == p.Annotation<UsedInWorkingThemaAnnotation>()) {
				p.AddAnnotation(UsedInWorkingThemaAnnotation.Default);
			}
			p = new XElement(p);
			if (pr.Name.LocalName == "use") {
				var list = p.Attribute("list");
				if (null != list) {
					list.Remove();
				}
			}
			foreach (var a in pr.Attributes()
				.Where(a =>
				       !a.Name.LocalName.EndsWith("list")
				       || pr.Name.LocalName != "use")) {
				p.SetAttributeValue(a.Name, a.Value);
			}
			if (!string.IsNullOrEmpty(pr.Value)) {
				p.Value = pr.Value;
			}
			if (null != p.Attribute("clear")) {
				p.Value = "";
			}
			switch (pr.Name.LocalName) {
				case "ask":
					p.Name = "var";
					break;
				case "use":
					p.Name = "param";
					break;
			}
			pr.ReplaceWith(p);
		}
	}
}