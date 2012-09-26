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
// Original file : EmbedGlobalsStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using System.Text.RegularExpressions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Process globals in XML data
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class EmbedGlobalsStep : ThemaCompilerStep {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="EmbedGlobalsStep" /> class.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public EmbedGlobalsStep() {
			_regex = new Regex(@"~(\p{Lu}+)", RegexOptions.Compiled);
		}

		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			_count = 0;
			if (0 == Context.Globals.Count) {
				return;
			}
			foreach (
				var e in Context.SourceFileXml.Where(e => null != e.Value.Annotation<HaveToBeProcessedWithGlobalsAnnotation>())) {
				VisitTextAndAttributes(e.Value, (x, s) =>
					{
						if (-1 == s.IndexOf('~')) {
							return s;
						}
						var result = _regex.Replace(s, m =>
							{
								if (
									Context.Globals.ContainsKey(m.Groups[1].Value)) {
									return
										Context.Globals[
											m.Groups[1].Value];
								}
								return "";
							});
						_count++;
						return result;
					});
				UserLog.Trace("globals embeded : " + _count + " in " + Context.LocalFileNames[e.Key]);
			}
		}

		/// <summary>
		/// </summary>
		private readonly Regex _regex;

		/// <summary>
		/// </summary>
		private int _count;
	}
}