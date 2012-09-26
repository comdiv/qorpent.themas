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
// Original file : CheckEcoProcessIntegrity.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using Qorpent.Themas.Compiler.EcoProcess;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps.EcoProcess {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class CheckEcoProcessIntegrity : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			CheckRecoursiveDependency();
			CheckThatThemaForOneGroupIsFormedInOnePlace();
			CheckStaging();
		}

		/// <summary>
		/// 	Checks the recoursive dependency.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void CheckRecoursiveDependency() {
			foreach (var p in Context.EcoProcessIndex.All.Where(p => FindDepend(p, p))) {
				AddError(ErrorLevel.Error,
				         "Процесс " + p.Code + " имеет рекурсивную ссылку на самого себя , проанализируйте цепочку зависимостей",
				         "ER_EPINTEG_3");
			}
		}

		/// <summary>
		/// 	Finds the depend.
		/// </summary>
		/// <param name="current"> The current. </param>
		/// <param name="whatToFind"> The what to find. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private static bool FindDepend(Process current, Process whatToFind) {
			return current.InDepends.Any(d => d.Process == whatToFind || FindDepend(d.Process, whatToFind));
		}

		/// <summary>
		/// 	Проверяем стадии внутри процесса, стадии должны идти по порядку, без пропусков и пустых ссылок
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void CheckStaging() {
			foreach (var p in Context.EcoProcessIndex.All) {
				var refs = p.ThemaRefs.Where(x => null != x.Thema && 0 != x.Stage).ToArray();
				if (!refs.Any()) {
					continue;
				}
				var maxstage = refs.Select(x => x.Stage).Max();
				if (1 == maxstage) {
					continue;
				}
				for (var i = maxstage - 1; i > 0; i--) {
					var exists = refs.Any(x => x.Stage == i);
					if (!exists) {
						AddError(ErrorLevel.Error,
						         "Процесс " + p.Code + " имеет максимальную стадию " + maxstage + ", при этом среди стадий отсутствует №" +
						         i,
						         "ER_EPINTEG_2");
					}
				}
			}
		}

		/// <summary>
		/// 	Проверяем, что одна и та же тема по write присуствует только в одном экземпляре
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void CheckThatThemaForOneGroupIsFormedInOnePlace() {
			IDictionary<string, string> checkedout = new Dictionary<string, string>();
			foreach (var p in Context.EcoProcessIndex.All) {
				foreach (var t in p.ThemaRefs.Where(x => x.IsWrite)) {
					var keys = new[] {t.Code, t.Code + "A", t.Code + "B"};
					if (t.Group.IsNotEmpty()) {
						keys = new[] {t.Code, t.Code + t.Group};
					}
					var proceed = true;
					foreach (var key in keys.Where(checkedout.ContainsKey)) {
						AddError(ErrorLevel.Error,
						         "Процесс " + p.Code + " дублирует ввод по форме " + key + ", определенный в форме" + checkedout[key],
						         "ER_EPINTEG_1");
						proceed = false;
					}
					if (proceed) {
						checkedout[t.Code + t.Group] = p.Code;
					}
				}
			}
		}
	}
}