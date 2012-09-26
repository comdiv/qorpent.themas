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
// Original file : EcoCleanupElementsStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Cleanup elements due to ECO logic ( bool(active.)==false drops all parameters with same generic)
	/// 	worked only if EcoOptimization of project is true (by default)
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class EcoCleanupElementsStep : ThemaCompilerStep {
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
			Cleanup(thema.SelfThemaItems, key);
			Cleanup(thema.SelfThemaItemsSets, key);
			Cleanup(thema.SelfThemaItemsExtensions, key);
			Cleanup(thema.ImportedThemaItems, key);
			Cleanup(thema.ImportedThemaItemsSets, key);
			Cleanup(thema.ImportedThemaItemsExtensions, key);
		}

		/// <summary>
		/// 	Cleanups the specified d.
		/// </summary>
		/// <typeparam name="TV"> The type of the V. </typeparam>
		/// <param name="d"> The d. </param>
		/// <param name="key"> The key. </param>
		/// <remarks>
		/// </remarks>
		private static void Cleanup<TV>(IDictionary<string, TV> d, string key) {
			foreach (var s in d.Keys.ToArray().Where(s => s.Split('.')[0].EndsWith(key))) {
				d.Remove(s);
			}
		}
	}
}