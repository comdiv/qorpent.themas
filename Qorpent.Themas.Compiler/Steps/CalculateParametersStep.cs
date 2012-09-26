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
// Original file : CalculateParametersStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Calculates parameters in themas with patterns "@name" and "...${name}..."
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class CalculateParametersStep : ThemaCompilerStep {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="CalculateParametersStep" /> class.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public CalculateParametersStep() {
			_atregex = new Regex(@"^@(\w[\w\d_\.]*)$", RegexOptions.Compiled);
			_inregex = new Regex(@"\$\{(\w[\w\d_\.]*)\}", RegexOptions.Compiled);
		}

		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var t in Context.Themas.Values) {
				if (!t.IsWorking && !Context.Project.KeepAbstractThemas) {
					continue; //no any needs to resolve parameters of abstract themas
				}
				foreach (var key in t.ResolvedParameters.Keys.ToArray()) {
					Resolve(t, key);
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
		protected virtual string Resolve(ThemaDescriptor td, string key, string src = null) {
			var val = td.GetParam(key, src);
			if (0 != val.IndexOf('@') && -1 == val.IndexOf("${", StringComparison.InvariantCulture)) {
				return val;
			}
			val = Doreplace(td, val, src);
			td.ResolvedParameters[key] = val;
			return val;
		}

		/// <summary>
		/// 	Doreplaces the specified td.
		/// </summary>
		/// <param name="td"> The td. </param>
		/// <param name="val"> The val. </param>
		/// <param name="src"> The SRC. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected string Doreplace(ThemaDescriptor td, string val, string src) {
			Match m;
			if (0 == val.IndexOf('@') && (m = _atregex.Match(val)).Success) {
				val = Resolve(td, m.Groups[1].Value, src);
			}
			else {
				val = _inregex.Replace(val, i => Resolve(td, i.Groups[1].Value));
			}
			return val;
		}

		/// <summary>
		/// </summary>
		private readonly Regex _atregex;

		/// <summary>
		/// </summary>
		private readonly Regex _inregex;
	}
}