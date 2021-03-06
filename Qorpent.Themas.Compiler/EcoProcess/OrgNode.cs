﻿#region LICENSE

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
// Original file : OrgNode.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;

namespace Qorpent.Themas.Compiler.EcoProcess {
	/// <summary>
	/// </summary>
	public class OrgNode {
		/// <summary>
		/// </summary>
		public OrgNode() {
			Children = new List<OrgNode>();
			Processes = new List<Process>();
		}

		/// <summary>
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// </summary>
		public int Level { get; set; }

		/// <summary>
		/// </summary>
		public OrgNode Parent { get; set; }

		/// <summary>
		/// </summary>
		public IList<OrgNode> Children { get; private set; }

		/// <summary>
		/// </summary>
		public IList<Process> Processes { get; private set; }
	}
}