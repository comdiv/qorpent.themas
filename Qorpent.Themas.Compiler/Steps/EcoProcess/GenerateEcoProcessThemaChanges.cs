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
// Original file : GenerateEcoProcessThemaChanges.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Themas.Compiler.EcoProcess;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps.EcoProcess {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class GenerateEcoProcessThemaChanges : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			PrepareThemaRolesAndProcessReferences();
			EmbedDependencyToForms();
			RewriteXml();
		}

		/// <summary>
		/// 	Embeds the dependency to forms.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void EmbedDependencyToForms() {
			foreach (var p in Context.EcoProcessIndex.All) {
				IList<ProcessThemaRef> startdependency = p.InDepends.SelectMany(d => d.Process.GetOutLockers()).ToList();
				foreach (var r in p.GetStage(0).Union(p.GetStage(1))) {
					//для 0-й и первой очереди тем ставим зависимость от входных
					EmbedDependency("A", r, startdependency);
					EmbedDependency("B", r, startdependency);
				}
				for (var i = 2; i <= p.GetMaxStage(); i++) {
					var targets = p.GetStage(i);
					var innerdepends = p.GetStage(i - 1).ToArray();
					foreach (var target in targets) {
						EmbedDependency("A", target, innerdepends);
						EmbedDependency("B", target, innerdepends);
					}
				}
			}
		}

		/// <summary>
		/// 	Embeds the dependency.
		/// </summary>
		/// <param name="group"> The group. </param>
		/// <param name="target"> The target. </param>
		/// <param name="sources"> The sources. </param>
		/// <remarks>
		/// </remarks>
		private static void EmbedDependency(string group, ProcessThemaRef target, IEnumerable<ProcessThemaRef> sources) {
			if (target.Group.IsNotEmpty() && target.Group != group) {
				return;
			}
			var targetform = target.Thema.GetForm(group);
			if (null == targetform) {
				return;
			}
			foreach (var s in from s in sources
			                  where s.IsMatchGroup(@group)
			                  let sourceform = s.Thema.GetForm(@group)
			                  where null != sourceform
			                  select s) {
				targetform.Add(new XElement("lockdepend", new XAttribute("code", s.Thema.Code + "." + @group + ".in")));
			}
		}

		/// <summary>
		/// 	Rewrites the XML.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void RewriteXml() {
			foreach (var t in Context.Themas.Values.Where(x => x.IsUnderEcoProcess)) {
				t.Xml = new GenerateXmlStep(Context).Generate(t);
			}
		}

		/// <summary>
		/// 	Prepares the thema roles and process references.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void PrepareThemaRolesAndProcessReferences() {
			foreach (var p in Context.EcoProcessIndex.All) {
				foreach (var r in p.ThemaRefs) {
					if (r.Thema == null) {
						continue;
					}
					var current = r.Thema.EcoProcess.SmartSplit();
					current.Add(p.Code);
					r.Thema.EcoProcess = string.Join("; ", current.ToArray());
					// заносим процесс в список параметров, переводим тему в режим процесса


					//we have to regenerate XML to avoid some strange effects
				}
			}
			foreach (var t in Context.Themas.Values.Where(x => x.IsUnderEcoProcess)) {
				var roles = new List<string>();
				Action<string> setrole = r =>
					{
						if (!roles.Contains(r)) {
							roles.Add(r);
						}
					};
				if (t.OwnerProcessA.IsNotEmpty()) {
					setrole(t.OwnerProcessA + "_OWN");
					if (t.OutViewA) {
						setrole(t.OwnerProcessA + "_VIEW");
					}
				}
				if (t.OwnerProcessB.IsNotEmpty()) {
					setrole(t.OwnerProcessB + "_OWN");
					if (t.OutViewA) {
						setrole(t.OwnerProcessB + "_VIEW");
					}
				}
				foreach (var vp in t.EcoProcess.SmartSplit()) {
					setrole(vp + "_VIEW");
				}

				if (null != t.Parent) {
					var parentroles = t.Parent.Role.SmartSplit();
					foreach (var role in roles.Except(parentroles)) {
						parentroles.Add(role);
					}
					t.Parent.Role = string.Join("; ", parentroles.ToArray());
				}
				var grp = t.Group;
				if (grp == null && t.Parent != null) {
					grp = t.Parent.Group;
				}
				if (grp != null) {
					var grproles = grp.Role.SmartSplit();
					foreach (var role in roles.Except(grproles)) {
						grproles.Add(role);
					}
					grp.Role = string.Join("; ", grproles.ToArray());
				}

				setrole(t.Code + "_OWN");
				setrole(t.Code + "_VIEW");
				t.Role = string.Join("; ", roles.ToArray());


				roles.Clear();
				if (t.OwnerProcessA.IsNotEmpty()) {
					setrole(t.OwnerProcessA + "_OWN");
				}
				setrole(t.Code + "_FORM_A_OWN");
				var aforms = t.Items.Keys.Where(x => x.EndsWith("A.in"));
				foreach (var x in aforms) {
					t.Items[x].SetAttributeValue("role", string.Join("; ", roles.ToArray()));
				}

				roles.Clear();
				if (t.OwnerProcessB.IsNotEmpty()) {
					setrole(t.OwnerProcessB + "_OWN");
				}
				setrole(t.Code + "_FORM_B_OWN");
				var bforms = t.Items.Keys.Where(x => x.EndsWith("B.in"));
				foreach (var x in bforms) {
					t.Items[x].SetAttributeValue("role", string.Join("; ", roles.ToArray()));
				}


				roles.Clear();

				foreach (var vp in t.EcoProcess.SmartSplit()) {
					if (t.OutViewA || t.OutViewB) {
						setrole(vp + "_VIEW");
					}
					setrole(vp + "_OWN");
				}
				setrole(t.Code + "_REPORT_VIEW");

				var areports = t.Items.Keys.Where(x => x.EndsWith(".out"));
				foreach (var x in areports) {
					t.Items[x].SetAttributeValue("role", string.Join("; ", roles.ToArray()));
				}
			}
		}
	}
}