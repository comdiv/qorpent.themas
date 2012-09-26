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
// Original file : ThemaCompilerResultForQweb.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.Serialization;

namespace Qorpent.Themas.Compiler.Tests {
	[Serialize]
	public class ThemaCompilerResultForQweb {
		public ThemaCompilerResultForQweb(ThemaCompilerContext context) {
			IsComplete = context.IsComplete;
			Errors = context.Errors.ToArray();
			if (!IsComplete) {
				return;
			}
			var r = new XElement("result");
			foreach (var t in context.Themas.Values.Where(t => null != t.Xml)) {
				r.Add(t.Xml);
			}
			if (null != context.ExtraData) {
				r.Add(context.ExtraData);
			}
			Result = Application.Current.Bxl.GetParser().Generate(r);
			Log = context.Project.GetLog();
		}

		[Serialize] public string Log { get; set; }

		[Serialize] public string Result { get; set; }

		[Serialize] public ThemaCompilerError[] Errors { get; set; }

		[Serialize] public bool IsComplete { get; set; }
	}
}