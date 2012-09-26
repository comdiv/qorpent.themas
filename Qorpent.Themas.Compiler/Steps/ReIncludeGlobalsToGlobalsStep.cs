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
// Original file : ReIncludeGlobalsToGlobalsStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using System.Text.RegularExpressions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Includes Globals into Globals
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ReIncludeGlobalsToGlobalsStep : ThemaCompilerStep {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="ReIncludeGlobalsToGlobalsStep" /> class.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public ReIncludeGlobalsToGlobalsStep() {
			_regex = new Regex(@"~\p{Lu}+", RegexOptions.Compiled);
		}

		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			if (Context.Project.Options.ContainsKey("no-reinclude-globals-to-globals")) {
				return;
			}
			foreach (var k in Context.Globals.Keys.ToArray()) {
				var val = Context.Globals[k];
				if (-1 == val.IndexOf('~')) {
					continue;
				}
				val = _regex.Replace(val, m =>
					{
						var k2 = m.Value.Substring(1);
						return Context.Globals.ContainsKey(k2) ? Context.Globals[k2] : "";
					});
				Context.Globals[k] = val;
				UserLog.Debug("global " + k + " reincluded");
			}
		}

		/// <summary>
		/// </summary>
		private readonly Regex _regex;
	}
}