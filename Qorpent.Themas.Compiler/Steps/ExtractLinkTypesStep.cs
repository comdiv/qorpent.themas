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
// Original file : ExtractLinkTypesStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Extract linktype elements and creates dictionary from them
	/// </summary>
	public class ExtractLinkTypesStep : IThemaCompilerStep {
		/// <summary>
		/// </summary>
		/// <param name="ctx"> </param>
		public void Process(ThemaCompilerContext ctx) {
			//firstly - fill links from ptoject default set
			foreach (var type in ctx.Project.LinkTypes) {
				ctx.LinkTypes[type.Key] = type.Value;
				ctx.UserLog.Info("link type " + type.Key + " regestered from project");
			}
			//secondary - oversee all files and get linktype elements
			foreach (var sf in ctx.SourceFileXml.Values) {
				foreach (var e in sf.Elements("linktype").ToArray()) {
					var ltype = e.Apply(new ThemaLinkType());
					ctx.LinkTypes[ltype.Code] = ltype;
					ctx.UserLog.Info("link type " + ltype.Code + " extracted from " + e.Describe().ToWhereString());
					e.Remove();
				}
			}
		}
	}
}