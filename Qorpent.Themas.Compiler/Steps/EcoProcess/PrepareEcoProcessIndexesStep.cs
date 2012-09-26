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
// Original file : PrepareEcoProcessIndexesStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using Qorpent.Themas.Compiler.EcoProcess;

namespace Qorpent.Themas.Compiler.Steps.EcoProcess {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class PrepareEcoProcessIndexesStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			if (Context.ExtraData == null) {
				AddError(ErrorLevel.Error, "Попытка вызвать конструктор процессов без подготовленного раздела ExtraData", "ER_EPI01");
				return;
			}
			try {
				Context.OrgNodeIndex.LoadFromXml(Context.ExtraData);
				UserLog.Info("Загружено " + Context.OrgNodeIndex.Index.Count + " элементов структуры");
			}
			catch (EcoProcessException e) {
				AddError(ErrorLevel.Error, "Ошибка при посроении индекса OrgNode", "ER_EPI02", e, e.File, e.Line);
			}

			try {
				Context.EcoProcessIndex.LoadFromXml(Context.ExtraData);
				UserLog.Info("Загружено " + Context.EcoProcessIndex.Index.Count + " процессов");
				if (Context.EcoProcessIndex.Errors.Count > 0) {
					foreach (var e in Context.EcoProcessIndex.Errors) {
						AddError(ErrorLevel.Error, "Ошибка при посроении индекса Process", "ER_EPI03", e);
					}
				}
				Context.EcoProcessIndex.Errors.Clear();
			}
			catch (EcoProcessException e) {
				AddError(ErrorLevel.Error, "Ошибка при посроении индекса Process", "ER_EPI03", e, e.File, e.Line);
			}


			try {
				Context.EcoProcessIndex.ResolveStructureAndOrgNodes(Context.OrgNodeIndex, Context.Themas.Values.ToArray());
				if (Context.EcoProcessIndex.Errors.Count > 0) {
					foreach (var e in Context.EcoProcessIndex.Errors) {
						AddError(ErrorLevel.Error, "Ошибка при разрешении ссылок в структуре процессов", "ER_EPI04", e);
					}
				}
				Context.EcoProcessIndex.Errors.Clear();
			}
			catch (EcoProcessException e) {
				AddError(ErrorLevel.Error, "Ошибка при разрешении ссылок в структуре процессов", "ER_EPI04", e, e.File, e.Line);
			}

			if (Context.EcoProcessIndex.EmptyReferences.Count == 0) {
				return;
			}
			var level = ErrorLevel.Warning;
			var code = "WR_EPI05";
			if (Context.Project.NonResolvedProcessThemaRefIsError) {
				level = ErrorLevel.Error;
				code = "ER_EPI05";
			}
			foreach (var emptyref in Context.EcoProcessIndex.EmptyReferences.ToArray()) {
				AddError(level,
				         "Процесс " + emptyref.ContainingProcess.Code + " ссылается на отсутствующую или абстрактную тему " +
				         emptyref.Code, code);
				emptyref.ContainingProcess.ThemaRefs.Remove(emptyref);
			}
		}
	}
}