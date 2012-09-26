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
// Original file : RemoveUnUsedThemasStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	removes all non-working themas
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class RemoveUnUsedThemasStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			if (Context.Project.KeepAbstractThemas) {
				return;
			}
			foreach (var t in Context.Themas.Values.Where(x => !x.IsWorking).ToArray()) {
				Context.Themas.Remove(t.Code);
				UserLog.Trace("thema " + t.Code + " removed due to it's not working");
			}
		}
	}
}