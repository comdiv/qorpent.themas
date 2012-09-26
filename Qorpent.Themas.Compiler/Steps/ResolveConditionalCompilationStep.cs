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
// Original file : ResolveConditionalCompilationStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	checks that xml block match conditions of project
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ResolveConditionalCompilationStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			_src = LogicTermSource.Create(Context.Project.Conditions);
			_evaluator = Application.Current.Container.Get<ILogicalExpressionEvaluator>();
			foreach (var x in Context.SourceFileXml.Values) {
				ResolveConditions(x);
			}
		}

		/// <summary>
		/// 	Resolves the conditions.
		/// </summary>
		/// <param name="e"> The e. </param>
		/// <remarks>
		/// </remarks>
		private void ResolveConditions(XContainer e) {
			foreach (var d in e.Descendants("if").ToArray()) {
				ResolveCondition(d, d.Id());
			}
		}

		/// <summary>
		/// 	Resolves the condition.
		/// </summary>
		/// <param name="e"> The e. </param>
		/// <param name="id"> The id. </param>
		/// <remarks>
		/// </remarks>
		private void ResolveCondition(XElement e, string id) {
			if (!Match(id)) {
				e.Remove();
				return;
			}
			foreach (var a in e.Attributes()) {
				SetParentAttribute(e, a);
			}
			e.ReplaceWith(e.Elements());
		}

		/// <summary>
		/// 	Sets the parent attribute.
		/// </summary>
		/// <param name="e"> The e. </param>
		/// <param name="a"> A. </param>
		/// <remarks>
		/// </remarks>
		private static void SetParentAttribute(XObject e, XAttribute a) {
			if (e.Parent != null) {
				e.Parent.SetAttributeValue(a.Name, a.Value);
			}
		}

		/// <summary>
		/// 	Matches the specified id.
		/// </summary>
		/// <param name="id"> The id. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private bool Match(string id) {
			return _evaluator.Eval(id, _src);
		}

		/// <summary>
		/// </summary>
		private ILogicalExpressionEvaluator _evaluator;

		/// <summary>
		/// </summary>
		private LogicTermSource _src;
	}
}