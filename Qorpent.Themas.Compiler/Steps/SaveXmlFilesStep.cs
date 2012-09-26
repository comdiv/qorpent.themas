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
// Original file : SaveXmlFilesStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.IO;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	Saves generated XML and Extra data to TargetFolder of project
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class SaveXmlFilesStep : ThemaCompilerStep {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="SaveXmlFilesStep" /> class.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public SaveXmlFilesStep() {
			_resolver = Application.Current.Files.GetResolver();
			_xmlsettings = new XmlWriterSettings
				{
					NewLineOnAttributes = true,
					NewLineChars = Environment.NewLine,
					Indent = true,
					IndentChars = "\t",
					OmitXmlDeclaration = true
				};
		}

		/// <summary>
		/// 	Flag that step will cleanup output folders before generation
		/// </summary>
		/// <value> <c>true</c> if cleanup; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool Cleanup { get; set; }

		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			if (Context.Project.OutputFolder.IsEmpty()) {
				AddError(ErrorLevel.Error, "no target folder is given", "TE3001");
				UserLog.Error("no target folder is given");
				return;
			}
			_currentresolver = Context.Project.Resolver ?? _resolver;
			var folder = _currentresolver.Resolve(Context.Project.OutputFolder, false);
			Directory.CreateDirectory(folder);

			if (Cleanup) {
				foreach (var f in Directory.GetFiles(folder)) {
					//cleanup directory from existed self-xml
					File.Delete(f);
				}
			}


			UserLog.Debug("target directory cleaned");
			foreach (var t in Context.Themas.Values) {
				var fileidx = t.ResolvedParameters["fileidx"].ToInt();
				var filename = t.Code + ".thema";
				if (Context.Project.UseFileIndexInFileName) {
					filename = string.Format("{0:0000}_{1}.thema", fileidx, t.Code);
				}
				var path = Path.Combine(folder, filename);

				WriteFile(path, t.Xml);

				UserLog.Trace("file " + filename + " saved to " + path);
			}
			var extraname = Path.Combine(folder, "EXTRADATA.thema");
			var e = Context.ExtraData;
			WriteFile(extraname, e);
		}

		/// <summary>
		/// 	Writes the file.
		/// </summary>
		/// <param name="filename"> The filename. </param>
		/// <param name="e"> The e. </param>
		/// <remarks>
		/// </remarks>
		protected virtual void WriteFile(string filename, XElement e) {
			filename += ".xml";
			using (var xmlwriter = XmlWriter.Create(filename, _xmlsettings)) {
				e.WriteTo(xmlwriter);
			}
		}

		/// <summary>
		/// </summary>
		private readonly IFileNameResolver _resolver;

		/// <summary>
		/// </summary>
		private readonly XmlWriterSettings _xmlsettings;

		/// <summary>
		/// </summary>
		private IFileNameResolver _currentresolver;
	}
}