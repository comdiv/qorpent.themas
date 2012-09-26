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
// Original file : EcoCleanupParametersStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Cleanup parameters due to ECO logic ( bool(active.)==false drops all parameters with same generic)
	/// 	worked only if EcoOptimization of project is true (by default)
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class EcoCleanupParametersStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			if (!Context.Project.UseEcoOptimization) {
				return;
			}
			foreach (var thema in Context.Themas.Values) {
				Cleanup(thema, "A");
				Cleanup(thema, "B");
				Cleanup(thema, "C");
				/*foreach (var pair in thema.ResolvedParameters.ToArray()) {
					if (string.IsNullOrEmpty(val) || "false" == val) {
						thema.ResolvedParameters.Remove(key);
					}
				}*/
			}
		}

		/// <summary>
		/// 	Cleanups the specified thema.
		/// </summary>
		/// <param name="thema"> The thema. </param>
		/// <param name="key"> The key. </param>
		/// <remarks>
		/// </remarks>
		private static void Cleanup(ThemaDescriptor thema, string key) {
			if (thema.IsGroupActive(key)) {
				return;
			}
			foreach (var p in thema.ResolvedParameters.Keys.ToArray().Where(p => p.EndsWith(key))) {
				thema.ResolvedParameters.Remove(p);
			}
		}
	}
}