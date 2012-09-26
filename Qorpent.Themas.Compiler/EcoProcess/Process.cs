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
// Original file : Process.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.Themas.Compiler.EcoProcess {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class Process {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="Process" /> class.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public Process() {
			InDepends = new List<ProcessInRef>();
			OutDepends = new List<ProcessInRef>();
			ThemaRefs = new List<ProcessThemaRef>();
		}

		/// <summary>
		/// 	Gets or sets the org node code.
		/// </summary>
		/// <value> The org node code. </value>
		/// <remarks>
		/// </remarks>
		public string OrgNodeCode { get; set; }

		/// <summary>
		/// 	Gets or sets the org node.
		/// </summary>
		/// <value> The org node. </value>
		/// <remarks>
		/// </remarks>
		public OrgNode OrgNode { get; set; }

		/// <summary>
		/// 	Gets or sets the code.
		/// </summary>
		/// <value> The code. </value>
		/// <remarks>
		/// </remarks>
		public string Code { get; set; }

		/// <summary>
		/// 	Gets or sets the name.
		/// </summary>
		/// <value> The name. </value>
		/// <remarks>
		/// </remarks>
		public string Name { get; set; }

		/// <summary>
		/// 	Gets the in depends.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public List<ProcessInRef> InDepends { get; private set; }

		/// <summary>
		/// 	Gets the out depends.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public List<ProcessInRef> OutDepends { get; private set; }

		/// <summary>
		/// 	Gets the thema refs.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public List<ProcessThemaRef> ThemaRefs { get; private set; }

		/// <summary>
		/// 	Gets or sets the XML.
		/// </summary>
		/// <value> The XML. </value>
		/// <remarks>
		/// </remarks>
		public XElement Xml { get; set; }

		/// <summary>
		/// 	Gets the out lockers.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<ProcessThemaRef> GetOutLockers() {
			return ThemaRefs.Where(x => x.Resolved && x.OutLock);
		}

		/// <summary>
		/// 	Gets the stage.
		/// </summary>
		/// <param name="stage"> The stage. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<ProcessThemaRef> GetStage(int stage) {
			return ThemaRefs.Where(x => x.Resolved && x.IsWrite && x.Stage == stage).ToArray();
		}

		/// <summary>
		/// 	Gets the max stage.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public int GetMaxStage() {
			var staged = ThemaRefs.Where(x => x.Resolved && x.Stage > 1).ToArray();
			return staged.Any() ? staged.Select(x => x.Stage).Max() : 1;
		}
	}
}