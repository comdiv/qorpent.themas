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
// Original file : SaveBxlFilesStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.Bxl;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	writes BXL equvalent of compiled themas
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class SaveBxlFilesStep : SaveXmlFilesStep {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="SaveBxlFilesStep" /> class.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public SaveBxlFilesStep() {
			_bxl = Application.Current.Bxl.GetParser();
		}

		/// <summary>
		/// 	Writes the file.
		/// </summary>
		/// <param name="filename"> The filename. </param>
		/// <param name="e"> The e. </param>
		/// <remarks>
		/// </remarks>
		protected override void WriteFile(string filename, XElement e) {
			filename += ".bxl";
			var opts = new BxlGeneratorOptions();

			opts.InlineAlwaysAttributes =
				new[] {"id", "code", "name", "_file", "_line", "role", "ecoprocess", "idx"}.Union(Context.Project.InlineAttributes).
					ToArray();
			var content = _bxl.Generate(e, opts);
			File.WriteAllText(filename, content);
		}

		/// <summary>
		/// </summary>
		private readonly IBxlParser _bxl;
	}
}