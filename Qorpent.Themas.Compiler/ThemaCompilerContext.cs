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
// Original file : ThemaCompilerContext.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Log;
using Qorpent.Themas.Compiler.EcoProcess;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler {
	/// <summary>
	/// 	main context for thema compiler
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ThemaCompilerContext {
		/// <summary>
		/// 	creates new with all defaults
		/// </summary>
		/// <remarks>
		/// </remarks>
		public ThemaCompilerContext() {
			SourceFileData = new Dictionary<string, string>();
			SourceFileXml = new Dictionary<string, XElement>();
			Themas = new Dictionary<string, ThemaDescriptor>();
			SubsetIndex = new Dictionary<string, XElement>();
			ParameterIndex = new Dictionary<string, XElement>();
			Properties = new Dictionary<string, object>();
			Errors = new List<ThemaCompilerError>();
			SourceFiles = new List<string>();
			Globals = new Dictionary<string, string>();
			LocalFileNames = new Dictionary<string, string>();
			Generators = new Dictionary<string, GeneratorDescriptor>();
			LinkTypes = new Dictionary<string, ThemaLinkType>();
			OrgNodeIndex = new OrgNodeCollection();
			RoleMaps = new BindingList<RoleMap>();
			EcoProcessIndex = new EcoProcessCollection();
		}

		/// <summary>
		/// 	Gets the link types.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, ThemaLinkType> LinkTypes { get; private set; }


		/// <summary>
		/// 	Gets the role maps.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<RoleMap> RoleMaps { get; private set; }

		/// <summary>
		/// 	Gets a value indicating whether this instance is error.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public bool IsError {
			get {
				if (0 == Errors.Count) {
					return false;
				}
				return Errors.Select(x => x.Level).Max() >= Project.ErrorLevel;
			}
		}


		/// <summary>
		/// 	Gets the index of the org node.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public OrgNodeCollection OrgNodeIndex { get; private set; }

		/// <summary>
		/// 	Gets the index of the eco process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public EcoProcessCollection EcoProcessIndex { get; private set; }

		/// <summary>
		/// 	Gets or sets the project.
		/// </summary>
		/// <value> The project. </value>
		/// <remarks>
		/// </remarks>
		public ThemaProject Project { get; set; }

		/// <summary>
		/// 	Gets or sets the pipeline.
		/// </summary>
		/// <value> The pipeline. </value>
		/// <remarks>
		/// </remarks>
		public ThemaCompilerPipeline Pipeline { get; set; }

		/// <summary>
		/// 	Gets or sets the index of the step.
		/// </summary>
		/// <value> The index of the step. </value>
		/// <remarks>
		/// </remarks>
		public int StepIndex { get; set; }

		/// <summary>
		/// 	Gets the globals.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, string> Globals { get; private set; }

		/// <summary>
		/// 	Gets the source files.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<string> SourceFiles { get; private set; }

		/// <summary>
		/// 	Gets the source file data.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, string> SourceFileData { get; private set; }

		/// <summary>
		/// 	Gets the local file names.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, string> LocalFileNames { get; private set; }

		/// <summary>
		/// 	Gets the source file XML.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, XElement> SourceFileXml { get; private set; }

		/// <summary>
		/// 	Gets the themas.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, ThemaDescriptor> Themas { get; private set; }

		/// <summary>
		/// 	Gets or sets the extra data.
		/// </summary>
		/// <value> The extra data. </value>
		/// <remarks>
		/// </remarks>
		public XElement ExtraData { get; set; }

		/// <summary>
		/// 	Gets the index of the subset.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, XElement> SubsetIndex { get; private set; }

		/// <summary>
		/// 	Gets the index of the parameter.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, XElement> ParameterIndex { get; private set; }

		/// <summary>
		/// 	Gets the properties.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, object> Properties { get; private set; }

		/// <summary>
		/// 	Gets the errors.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<ThemaCompilerError> Errors { get; private set; }

		/// <summary>
		/// 	Gets or sets a value indicating whether this instance is complete.
		/// </summary>
		/// <value> <c>true</c> if this instance is complete; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool IsComplete { get; set; }

		/// <summary>
		/// 	Gets the UserLog.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IUserLog UserLog {
			get { return _userLog ?? (_userLog = Project.UserLog ?? new StubUserLog()); }
		}

		/// <summary>
		/// 	Gets the generators.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, GeneratorDescriptor> Generators { get; private set; }

		/// <summary>
		/// 	Gets or sets a value indicating whether [non resolved extra embed is error].
		/// </summary>
		/// <value> <c>true</c> if [non resolved extra embed is error]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool NonResolvedExtraEmbedIsError { get; set; }

		/// <summary>
		/// 	Extras the eco process role map.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public XElement ExtraEcoProcessRoleMap() {
			return ExtraData.Element("ecoprocess_rolemap");
		}

		/// <summary>
		/// 	Gets the global by local.
		/// </summary>
		/// <param name="file"> The file. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public string GetGlobalByLocal(string file) {
			var result = LocalFileNames.FirstOrDefault(x => x.Value == file);
			return result.Key;
		}

		/// <summary>
		/// 	Gets the extra substitutes.
		/// </summary>
		/// <param name="resolve"> The resolve. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, XElement> GetExtraSubstitutes(string resolve) {
			if (null == ExtraData) {
				return null;
			}
			if (_substs.ContainsKey(resolve)) {
				return _substs[resolve];
			}
			var dict = new Dictionary<string, XElement>();
			_substs[resolve] = dict;
			foreach (var e in ExtraData.Elements(resolve)) {
				var code = e.Id();
				dict[code] = e;
			}
			return dict;
		}

		/// <summary>
		/// </summary>
		private readonly IDictionary<string, IDictionary<string, XElement>> _substs =
			new Dictionary<string, IDictionary<string, XElement>>();

		/// <summary>
		/// </summary>
		private IUserLog _userLog;
	}
}