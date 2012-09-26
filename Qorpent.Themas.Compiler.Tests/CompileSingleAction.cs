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
// Original file : CompileSingleAction.cs
// Project: Qorpent.Themas.Compiler.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using Qorpent.Applications;
using Qorpent.Mvc;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Tests {
	[Action("thema.compilesingle")]
	public class CompileSingleAction {
		public CompileSingleAction(string text, string projbxl) {
			Text = text;
			ProjBxl = projbxl;
		}

		public object Process() {
			var project = new SingleContentProject(Text);
			if (ProjBxl.IsNotEmpty()) {
				project.ConfigureFromXml(Application.Current.Bxl.Parse(ProjBxl));
			}
			return new ThemaCompilerResultForQweb(
				LastContext = new ThemaCompiler().Compile(
					project));
		}

		public ThemaCompilerContext LastContext;
		[Bind] public string ProjBxl;
		[Bind] public string Text;
	}
}