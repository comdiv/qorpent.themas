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
// Original file : GenerateEcoProcessRoleMap.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Xml.Linq;
using Qorpent.Themas.Compiler.EcoProcess;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps.EcoProcess {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class GenerateEcoProcessRoleMap : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			BuildRoleMapIndex();
			UserLog.Info("Идентифицировано " + Context.RoleMaps.Count + " сопоставлений ролей");
			if (Context.RoleMaps.Count <= 0) {
				return;
			}
			var x = new XElement("ecoprocess_rolemap");
			foreach (var map in Context.RoleMaps) {
				x.Add(new XElement("map"
				                   , new XAttribute("from", map.From)
				                   , new XAttribute("to", map.To)
				                   , new XAttribute("cause", map.Cause)
					      ));
			}
			Context.ExtraData.Add(x);
		}

		/// <summary>
		/// 	Builds the index of the role map.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void BuildRoleMapIndex() {
			foreach (var from in Context.OrgNodeIndex.All) {
				foreach (var to in from.Children) {
					Context.RoleMaps.Add(new RoleMap {From = from.Code, To = to.Code, Cause = "orgnode"});
				}
				foreach (var p in from.Processes) {
					Context.RoleMaps.Add(new RoleMap {From = from.Code, To = p.Code + "_OWN", Cause = "process_own"});
					foreach (var pi in p.InDepends) {
						Context.RoleMaps.Add(new RoleMap {From = p.Code + "_OWN", To = pi.Code + "_VIEW", Cause = "process_view"});
					}
					foreach (var r in p.ThemaRefs) {
						var suffix = r.Group;
						if (r.OutView) {
							if (suffix.IsEmpty() || suffix == "A") {
								r.Thema.OutViewA = true;
							}
							if (suffix.IsEmpty() || suffix == "B") {
								r.Thema.OutViewB = true;
							}
						}

						if (r.Mode != "write") {
							continue;
						}
						if (suffix.IsEmpty() || suffix == "A") {
							r.Thema.OwnerProcessA = p.Code;
						}
						if (suffix.IsEmpty() || suffix == "B") {
							r.Thema.OwnerProcessB = p.Code;
						}
					}
				}
			}
		}
	}
}