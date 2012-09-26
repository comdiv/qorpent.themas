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
// Original file : EmbedExtraDataStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Imports extradata into thema's XML by code
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class EmbedExtraDataStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			if (Context.ExtraData == null || !Context.ExtraData.Elements().Any()) {
				if (null == Context.Project.ExtraEmbedElements || (0 == Context.Project.ExtraEmbedElements.Count)) {
					return;
				}
			}
			foreach (var t in Context.Themas.Values) {
				if (null == t.Xml) {
					continue;
				}
				foreach (var ee in Context.Project.ExtraEmbedElements) {
					var find = ee.Key;
					var resolve = ee.Value;
					var src = Context.GetExtraSubstitutes(resolve);
					foreach (var trg in t.Xml.Descendants(find).ToArray()) {
						var code = trg.Id();
						if (src.ContainsKey(code)) {
							trg.ReplaceWith(src[code]);
						}
						else {
							var message = "cannot find extraembed " + find + ":" + code + " -> " + resolve +
							              " in extra data";
							var errorcode = "TW2601";
							var errorlevel = ErrorLevel.Warning;
							var file = trg.Describe().File;
							var line = trg.Describe().Line;
							if (Context.NonResolvedExtraEmbedIsError) {
								errorlevel = ErrorLevel.Error;
								errorcode = "TE2601";
							}
							AddError(errorlevel, message, errorcode, null, file, line);
							if (Context.NonResolvedExtraEmbedIsError) {
								return;
							}
						}
					}
				}
			}
		}
	}
}