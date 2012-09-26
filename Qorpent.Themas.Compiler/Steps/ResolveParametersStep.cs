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
// Original file : ResolveParametersStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Resolves all parameters in all themas with generic support
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ResolveParametersStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			foreach (var thema in Context.Themas.Values) {
				ResolveParameters(thema);
			}
		}

		/// <summary>
		/// 	Resolves the parameters.
		/// </summary>
		/// <param name="thema"> The thema. </param>
		/// <remarks>
		/// </remarks>
		private void ResolveParameters(ThemaDescriptor thema) {
			if (thema.ParametersResolved) {
				return;
			}
			// cluster is must-be property, if no imports and self setted, set it as DEFAULT
			if (thema.Imports.Count == 0) {
				thema.ResolvedParameters["cluster"] = "DEFAULT";
			}
			foreach (var importthema in thema.Imports.Select(import => Context.Themas[import])) {
				ResolveParameters(importthema);
				foreach (var rp in importthema.ResolvedParameters.Where(rp => !NonInheritableParameter(thema, rp))) {
					thema.ResolvedParameters[rp.Key] = rp.Value;
				}
			}
			foreach (
				var sp in
					thema.SelfParameters.OrderBy(
						x => x.Key.StartsWith("__PLUS__") ? "ZZ0" + x.Key : (x.Key.StartsWith("__MINUS__") ? "ZZ1" + x.Key : x.Key))) {
				var complex = new Complex(sp.Key);
				if ("" == complex.Type) {
					thema.ResolvedParameters[sp.Key] = sp.Value;
				}
				else {
					var current = "";
					if (thema.ResolvedParameters.ContainsKey(complex.Name)) {
						current = thema.ResolvedParameters[complex.Name];
					}
					current = complex.Type == "+"
						          ? ComplexStringHelper.Set(current, sp.Value)
						          : ComplexStringHelper.Remove(current, sp.Value);
					thema.ResolvedParameters[complex.Name] = current;
				}
			}
			if (thema.IsGeneric) {
				foreach (var p in thema.ResolvedParameters.Keys.ToArray()) {
					if (!p.EndsWith(".")) {
						continue;
					}
					var v = thema.ResolvedParameters[p];
					thema.ResolvedParameters.Remove(p);
					thema.ResolvedParameters[p.Substring(0, p.Length - 1) + thema.Index] = v;
				}

				foreach (var p in thema.ResolvedParameters.Keys.ToArray()) {
					var val = thema.ResolvedParameters[p];
					if (ThemaDescriptor.Atregex.IsMatch(val) && val.EndsWith(".")) {
						thema.ResolvedParameters[p] = val.Substring(0, val.Length - 1) + thema.Index;
					}
					else if (val.Contains("${")) {
						thema.ResolvedParameters[p] = ThemaDescriptor.Inregex.Replace(val, m =>
							{
								var v = m.Groups[1].Value;

								if (v == "@") {
									return thema.Index;
								}
								if (!v.EndsWith(".")) {
									return m.Value;
								}
								return
									"${" + v.Substring(0, v.Length - 1) +
									thema.Index + "}";
							});
					}
				}
			}

			thema.ParametersResolved = true;
		}

		/// <summary>
		/// 	Nons the inheritable parameter.
		/// </summary>
		/// <param name="thema"> The thema. </param>
		/// <param name="rp"> The rp. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private bool NonInheritableParameter(ThemaDescriptor thema, KeyValuePair<string, string> rp) {
			if (-1 != Context.Project.NonInheritableParameters.IndexOf(rp.Key)) {
				return true;
			}
			if (!thema.IsGeneric && rp.Key.EndsWith(".")) {
				return true;
			}
			return rp.Key.StartsWith("__PLUS__") || rp.Key.StartsWith("__MUNIS__");
		}

		#region Nested type: Complex

		/// <summary>
		/// </summary>
		/// <remarks>
		/// </remarks>
		private class Complex {
			/// <summary>
			/// 	Initializes a new instance of the <see cref="Complex" /> class.
			/// </summary>
			/// <param name="key"> The key. </param>
			/// <remarks>
			/// </remarks>
			public Complex(string key) {
				if (key.StartsWith("__PLUS__")) {
					Type = "+";
					Name = key.Substring(8);
				}
				else if (key.StartsWith("__MINUS__")) {
					Type = "-";
					Name = key.Substring(9);
				}
				else {
					Type = "";
					Name = key;
				}
			}

			/// <summary>
			/// 	Gets or sets the name.
			/// </summary>
			/// <value> The name. </value>
			/// <remarks>
			/// </remarks>
			public string Name { get; private set; }

			/// <summary>
			/// 	Gets or sets the type.
			/// </summary>
			/// <value> The type. </value>
			/// <remarks>
			/// </remarks>
			public string Type { get; private set; }
		}

		#endregion
	}
}