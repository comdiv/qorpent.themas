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
// Original file : OrgNodeCollection.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.EcoProcess {
	/// <summary>
	/// </summary>
	public class OrgNodeCollection {
		/// <summary>
		/// </summary>
		/// <param name="code"> </param>
		public OrgNode this[string code] {
			get { return Index.ContainsKey(code) ? Index[code] : null; }
		}

		/// <summary>
		/// </summary>
		public IEnumerable<OrgNode> Roots {
			get { return Index.Values.Where(x => null == x.Parent).ToArray(); }
		}

		/// <summary>
		/// </summary>
		public IDictionary<string, OrgNode> Index {
			get { return _index; }
		}


		/// <summary>
		/// </summary>
		public IEnumerable<OrgNode> All {
			get { return _index.Values.ToArray(); }
		}


		/// <summary>
		/// </summary>
		/// <param name="src"> </param>
		public void LoadFromXml(XElement src) {
			var elements = src.Elements("orgnode");
			foreach (var e in elements) {
				AddElementFromXml(e, null);
			}
		}

		private void AddElementFromXml(XElement e, OrgNode parent) {
			var orgnode = new OrgNode();
			e.Apply(orgnode);
			if (orgnode.Code.IsEmpty()) {
				throw new EcoProcessException("Узлу не сопоставлен код :" + e.Describe().ToWhereString());
			}
			Add(orgnode, parent);
			foreach (var c in e.Elements("orgnode")) {
				AddElementFromXml(c, orgnode);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="node"> </param>
		/// <param name="parent"> </param>
		/// <exception cref="EcoProcessException"></exception>
		public void Add(OrgNode node, OrgNode parent = null) {
			if (Index.ContainsKey(node.Code)) {
				throw new EcoProcessException("Организационная структура включет в себя дублирующийся код узла :" + node.Code);
			}
			node.Level = 1;
			if (null != parent) {
				node.Parent = parent;
				node.Level = parent.Level + 1;
				parent.Children.Add(node);
			}
			Index[node.Code] = node;
		}

		private readonly IDictionary<string, OrgNode> _index = new Dictionary<string, OrgNode>();
	}
}