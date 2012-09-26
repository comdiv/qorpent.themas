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
// Original file : ProcessThemaRef.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.EcoProcess {
	/// <summary>
	/// </summary>
	public class ProcessThemaRef : XmlExtensions.ICustomXmlApplyer {
		/// <summary>
		/// </summary>
		public ProcessThemaRef() {
			Mode = "write";
			OutView = false;
			OutLock = false;
			Group = "";
		}


		/// <summary>
		/// </summary>
		public bool Resolved {
			get { return null != Thema; }
		}

		/// <summary>
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// </summary>
		public ThemaDescriptor Thema { get; set; }

		/// <summary>
		/// </summary>
		public bool IsWrite {
			get { return Mode == "write"; }
		}

		/// <summary>
		/// </summary>
		public Process ContainingProcess { get; set; }

		/// <summary>
		/// </summary>
		public string Mode { get; set; }

		/// <summary>
		/// </summary>
		public bool OutView { get; set; }

		/// <summary>
		/// </summary>
		public bool OutLock { get; set; }

		/// <summary>
		/// </summary>
		public int Stage { get; set; }

		/// <summary>
		/// </summary>
		public string Group { get; set; }


		void XmlExtensions.ICustomXmlApplyer.Apply(XElement element) {
			_name = element.Describe().Name;
			switch (_name) {
				case "read":
					Mode = "read";
					break;
				case "write":
					Mode = "write";
					break;
				case "outview":
					OutView = true;
					break;
				case "outlock":
					OutLock = true;
					break;
				case "A":
					Group = "A";
					break;
				case "B":
					Group = "B";
					break;
			}
		}


		/// <summary>
		/// </summary>
		/// <param name="group"> </param>
		/// <returns> </returns>
		public bool IsMatchGroup(string group) {
			return Group.IsEmpty() || Group.Equals(@group);
		}

		private string _name;
	}
}