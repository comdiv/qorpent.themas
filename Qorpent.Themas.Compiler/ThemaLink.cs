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
// Original file : ThemaLink.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Xml.Linq;

namespace Qorpent.Themas.Compiler {
	/// <summary>
	/// </summary>
	public class ThemaLink {
		/// <summary>
		/// </summary>
		public ThemaLinkType Type { get; set; }

		/// <summary>
		/// </summary>
		public string SourceCode { get; set; }

		/// <summary>
		/// </summary>
		public string TargetCode { get; set; }

		/// <summary>
		/// </summary>
		public ThemaDescriptor Source { get; set; }

		/// <summary>
		/// </summary>
		public ThemaDescriptor Target { get; set; }

		/// <summary>
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// </summary>
		public int Line { get; set; }

		/// <summary>
		/// </summary>
		public string File { get; set; }

		/// <summary>
		/// </summary>
		public XElement Xml { get; set; }

		/// <summary>
		/// </summary>
		public bool Deleted { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="source"> </param>
		/// <returns> </returns>
		public ThemaLink Import(ThemaDescriptor source) {
			var copy = (ThemaLink) MemberwiseClone();
			copy.Source = source;
			copy.SourceCode = source.Code;
			return copy;
		}
	}
}