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
// Original file : ExtractThemaLinksStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ExtractThemaLinksStep : IThemaCompilerStep {
		/// <summary>
		/// 	Processes the specified CTX.
		/// </summary>
		/// <param name="ctx"> The CTX. </param>
		/// <remarks>
		/// </remarks>
		public void Process(ThemaCompilerContext ctx) {
			foreach (var t in ctx.Themas.Values) {
				foreach (var e in t.Fullsource.Elements().ToArray()) {
					if (!ctx.LinkTypes.ContainsKey(e.Name.LocalName)) {
						continue;
					}
					var d = e.Describe();
					t.SelfLinks.Add(new ThemaLink
						{
							Type = ctx.LinkTypes[e.Name.LocalName],
							Source = t,
							SourceCode = t.Code,
							TargetCode = e.Id(),
							Value = e.Attr("name"),
							Line = d.Line,
							File = d.File,
							Xml = e
						});
					e.Remove();
				}
			}
		}
	}
}