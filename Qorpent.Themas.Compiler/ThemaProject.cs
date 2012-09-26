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
// Original file : ThemaProject.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.IO;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler {
	/// <summary>
	/// 	Project definition for thema compiler
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ThemaProject {
		/// <summary>
		/// 	generates default project
		/// </summary>
		/// <remarks>
		/// </remarks>
		public ThemaProject() {
			Conditions = new List<string>();
			DirectSource = new Dictionary<string, string>();
			UseEcoOptimization = false;
			Options = new Dictionary<string, object>();
			ErrorLevel = ErrorLevel.Error;
			SubsetElementName = "subset";
			ImportSubsetElementName = "useset";
			Clusters = new List<string>();
			ItemElements = new List<string> {"form:in", "out:out", "report:out", "in:in"};
			ItemSetElements = new List<string> {"outset:out", "inset:in", "formset:in", "reportset:out"};
			ItemExtensionElements = new List<string> {"outext:out", "inext:in", "formsetex:in", "reportsetex:in"};
			NonInheritableParameters =
				new List<string>
					{"abst", "generic", "library", "code", "id", "name", "_file", "_line", "inactive", "fileidx", "index", "idx"};
			AttributeOrder = new List<string>
				{
					"id",
					"code",
					"name",
					"cluster",
					"_file",
					"_line",
					"inactive",
					"fileidx",
					"idx",
					"abst",
					"generic"
				};
			ExtraEmbedElements = new Dictionary<string, string>
				{
					{"useext", "extension"}
				};
			InlineAttributes = new List<string>();
			LinkTypes = new Dictionary<string, ThemaLinkType>
				{
					{
						"parent", new ThemaLinkType
							{
								Code = "parent",
								Name = "Родитель",
								BackName = "Дочерние",
								Singleton = true,
								Inheritable = true,
								RequireResolution = true,
								OnCustomValidation =
									(l, t) =>
										{
											if (null == l.Target) {
												return null;
											}
											if (l.Target.GetParam("isgroup").ToBool() || l.Target.IsLibrary) {
												return new ThemaCompilerError
													{
														Level = ErrorLevel.Error,
														ErrorCode = "LP01",
														Message = "parent link cannot target GROUP or LIBRARY thema " + l.TargetCode,
														Line = l.Line,
														SourceFile = l.File,
													};
											}
											var grp = t.Links.FirstOrDefault(x => x.Type.Code == "group");
											if (grp != null) {
												var prntgrp = l.Target.Links.FirstOrDefault(x => x.Type.Code == "group");
												if (null == prntgrp || prntgrp.TargetCode != grp.TargetCode) {
													return new ThemaCompilerError
														{
															Level = ErrorLevel.Error,
															ErrorCode = "LP02",
															Message =
																"if thema, containing parent reference has a group, group must match parent's group ",
															Line = l.Line,
															SourceFile = l.File,
														};
												}
											}
											return null;
										},
							}
					},
					{
						"group", new ThemaLinkType
							{
								Code = "group",
								Name = "Группа",
								BackName = "Состав группы",
								Singleton = true,
								Inheritable = true,
								RequireResolution = true,
								RequireAttribute = "isgroup",
							}
					},
				};
			NonSaveParameters = new List<string>();
			SourceFiles = new List<string>();
		}

		/// <summary>
		/// 	attributes forced to generate inline
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<string> InlineAttributes { get; private set; }

		/// <summary>
		/// 	error if emport/extent operator cannot be resolved
		/// </summary>
		/// <value> <c>true</c> if [non resolved import is error]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool NonResolvedImportIsError { get; set; }

		/// <summary>
		/// 	error if useset was not resolved
		/// </summary>
		/// <value> <c>true</c> if [non resolved subset is error]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool NonResolvedSubsetIsError { get; set; }

		/// <summary>
		/// 	error if thema reference from process was not resolved
		/// </summary>
		/// <value> <c>true</c> if [non resolved process thema ref is error]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool NonResolvedProcessThemaRefIsError { get; set; }

		/// <summary>
		/// 	conditions for conditional compilation
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<string> Conditions { get; private set; }

		/// <summary>
		/// 	parameters forced not to be saved in output
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<string> NonSaveParameters { get; private set; }

		/// <summary>
		/// 	typed link types
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, ThemaLinkType> LinkTypes { get; private set; }

		/// <summary>
		/// 	names for extra embeded elements
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, string> ExtraEmbedElements { get; private set; }

		/// <summary>
		/// 	directly defined "files" of content
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, string> DirectSource { get; private set; }

		/// <summary>
		/// 	define custom attribute order in output
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<string> AttributeOrder { get; private set; }

		/// <summary>
		/// 	override import subset element name (default useset)
		/// </summary>
		/// <remarks>
		/// </remarks>
		public string ImportSubsetElementName { get; private set; }

		/// <summary>
		/// 	forces to render order of file in name of generated thema file
		/// </summary>
		/// <remarks>
		/// </remarks>
		public bool UseFileIndexInFileName { get; private set; }

		/// <summary>
		/// 	override element name for defining subsets
		/// </summary>
		/// <remarks>
		/// </remarks>
		public string SubsetElementName { get; private set; }

		/// <summary>
		/// 	swithch on ECO optimization mode
		/// </summary>
		/// <value> <c>true</c> if [use eco optimization]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool UseEcoOptimization { get; set; }

		/// <summary>
		/// 	prepares statistics about subset reference
		/// </summary>
		/// <value> <c>true</c> if [analyze subset usage]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool AnalyzeSubsetUsage { get; set; }

		/// <summary>
		/// 	prepares statistics about parameter reference
		/// </summary>
		/// <value> <c>true</c> if [analyze param usage]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool AnalyzeParamUsage { get; set; }

		/// <summary>
		/// 	parameters that not inherited bethween themas
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<string> NonInheritableParameters { get; private set; }

		/// <summary>
		/// 	list of file masks to search (paths for Resolver)
		/// </summary>
		/// <value> The source files. </value>
		/// <remarks>
		/// </remarks>
		public IList<string> SourceFiles { get; set; }

		//NOTE: not ever used property TemporaryFolder
		//public string TemporaryFolder { get; set; }

		/// <summary>
		/// 	Folder for output compiled files
		/// </summary>
		/// <value> The output folder. </value>
		/// <remarks>
		/// </remarks>
		public string OutputFolder { get; set; }

		/// <summary>
		/// 	max error level with which compilation can still proceed
		/// </summary>
		/// <value> The error level. </value>
		/// <remarks>
		/// </remarks>
		public ErrorLevel ErrorLevel { get; set; }

		/// <summary>
		/// 	custom thema compiler step
		/// </summary>
		/// <value> The custom compiler. </value>
		/// <remarks>
		/// </remarks>
		public IThemaCompilerSetup CustomCompiler { get; set; }

		/// <summary>
		/// 	custom options (dictionary)
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, object> Options { get; private set; }

		/// <summary>
		/// 	compile process UserLog
		/// </summary>
		/// <value> The UserLog. </value>
		/// <remarks>
		/// </remarks>
		public IUserLog UserLog { get; set; }

		/// <summary>
		/// 	Filename resolver to use
		/// </summary>
		/// <value> The resolver. </value>
		/// <remarks>
		/// </remarks>
		public IFileNameResolver Resolver { get; protected set; }

		/// <summary>
		/// 	list of thema item's names
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<string> ItemElements { get; private set; }

		/// <summary>
		/// 	list of thema itemset's names
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<string> ItemSetElements { get; private set; }

		/// <summary>
		/// 	list of thema itemext's names
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<string> ItemExtensionElements { get; private set; }

		/// <summary>
		/// 	throw error if ask,use not resolved
		/// </summary>
		/// <value> <c>true</c> if [non resolved parameter is error]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool NonResolvedParameterIsError { get; set; }

		/// <summary>
		/// 	abstract themas will be generated in directory
		/// </summary>
		/// <value> <c>true</c> if [keep abstract themas]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool KeepAbstractThemas { get; set; }

		/// <summary>
		/// 	unresolved call is error
		/// </summary>
		/// <value> <c>true</c> if [non resolved generator is error]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool NonResolvedGeneratorIsError { get; set; }

		/// <summary>
		/// 	unresolved uselib is error
		/// </summary>
		/// <value> <c>true</c> if [non resolved library is error]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool NonResolvedLibraryIsError { get; set; }

		/// <summary>
		/// 	target clusters of themas
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<string> Clusters { get; private set; }

		/// <summary>
		/// 	EcoProcess module included
		/// </summary>
		/// <value> <c>true</c> if [use eco process]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool UseEcoProcess { get; set; }

		/// <summary>
		/// 	loads project from xml
		/// </summary>
		/// <param name="x"> The x. </param>
		/// <param name="target"> The target. </param>
		/// <remarks>
		/// </remarks>
		public void ConfigureFromXml(XElement x, string target = "DEFAULT") {
			//Contract.Requires<ArgumentNullException>(x!=null);
			//Contract.Requires<ArgumentNullException>(target.IsNotEmpty());
			UserLog = ConsoleLogWriter.CreateLog(GetType().ToStr(), LogLevel.Warning);
			//Application.Current.Principal.GetLog(this); // new TextWriterLogListener(Console.Out, null, LogLevel.Warning);
			var targets = target.SmartSplit();
			if (!targets.Contains("DEFAULT")) {
				targets.Add("DEFAULT");
			}
			foreach (var e in x.Elements()) {
				var etargets = e.Attr("target").SmartSplit();
				if (etargets.Count == 0) {
					etargets.Add("DEFAULT");
				}
				if (!targets.Intersect(etargets).Any()) {
					continue;
				}
				switch (e.Name.LocalName.ToLower()) {
					case "condition":
						Conditions.Add(e.Id());
						break;
					case "target":
						if (target.IsNotEmpty() && target != "DEFAULT") {
							break;
						}
						var ts = e.Id().SmartSplit();
						foreach (var t in ts.Except(targets)) {
							targets.Add(t);
						}
						break;
					case "__MINUS__target":
						ts = e.Id().SmartSplit();
						foreach (var t in ts) {
							targets.Remove(t);
						}
						break;
					case "directsource":
						DirectSource[e.Id()] = e.Describe().Name;
						break;
					case "option":
						Options[e.Id()] = e.Describe().Name;
						break;
					case "useecooptimization":
						UseEcoOptimization = e.Id() != "false";
						break;
					case "useecoprocess":
						UseEcoProcess = e.Id() != "false";
						break;
					case "analyzesubsetusage":
						AnalyzeSubsetUsage = e.Id() != "false";
						break;
					case "nonresolvedparameteriserror":
						NonResolvedParameterIsError = e.Id() != "false";
						break;
					case "nonresolvedgeneratoriserror":
						NonResolvedGeneratorIsError = e.Id() != "false";
						break;

					case "nonresolvedthemaimportiserror":
						NonResolvedImportIsError = e.Id() != "false";
						break;
					case "nonresolvedprocessthemarefiserror":
						NonResolvedProcessThemaRefIsError = e.Id() != "false";
						break;


					case "nonresolvedlibraryiserror":
						NonResolvedLibraryIsError = e.Id() != "false";
						break;
					case "nonresolvedsubsetiserror":
						NonResolvedSubsetIsError = e.Id() != "false";
						break;
					case "analyzeparamusage":
						AnalyzeParamUsage = e.Id() != "false";
						break;
						/*case "temporaryfolder":
						TemporaryFolder = e.Id();
						break;*/
					case "outputfolder":
						OutputFolder = e.Id();
						break;
					case "UserLog":
						var loglevel = e.Describe().Name.To<LogLevel>();
						if (LogLevel.None == loglevel) {
							loglevel = LogLevel.Warning;
						}
						var writer = e.Id() == "console"
							             ? Console.Out
							             : new StreamWriter(Resolver.Resolve(e.Id()), true, Encoding.UTF8);
						if (e.Id() != "console") {
							UserLog = BaseTextWriterLogWriter.CreateLog(GetType().ToString(), writer, loglevel);
						}
						else {
							UserLog = ConsoleLogWriter.CreateLog(GetType().ToString(), loglevel);
						}
						break;
					case "customcompiler":
						CustomCompiler = Type.GetType(e.Id()).Create<IThemaCompilerSetup>();
						break;

					case "errorlevel":
						ErrorLevel = e.Id().To<ErrorLevel>();
						if (ErrorLevel == ErrorLevel.None) {
							ErrorLevel = ErrorLevel.Error;
						}
						break;
					case "subsetelement":
						SubsetElementName = e.Id();
						break;
					case "importsubsetelement":
						ImportSubsetElementName = e.Id();
						break;
					case "cluster":
						Clusters.Add(e.Id());
						break;
					case "itemelement":
						if (!ItemElements.Contains(e.Id())) {
							ItemElements.Add(e.Id());
						}
						break;
					case "itemsetelement":
						if (!ItemSetElements.Contains(e.Id())) {
							ItemSetElements.Add(e.Id());
						}
						break;
					case "itemextelement":
						if (!ItemExtensionElements.Contains(e.Id())) {
							ItemExtensionElements.Add(e.Id());
						}
						break;


					case "__MINUS__itemelement":
						ItemElements.Remove(e.Id());
						break;
					case "__MINUS__itemsetelement":
						ItemSetElements.Remove(e.Id());
						break;
					case "__MINUS__itemextelement":
						ItemSetElements.Remove(e.Id());
						break;
					case "noninheritableparameter":
						if (!NonInheritableParameters.Contains(e.Id())) {
							NonInheritableParameters.Add(e.Id());
						}
						break;
					case "__MINUS__noninheritableparameter":
						NonInheritableParameters.Remove(e.Id());
						break;
					case "extraembed":
						ExtraEmbedElements[e.Id()] = e.Describe().Name;
						break;
					case "notsaveparameter":
						if (!NonSaveParameters.Contains(e.Id())) {
							NonSaveParameters.Add(e.Id());
						}
						break;
					case "inlineattribute":
						if (!InlineAttributes.Contains(e.Id())) {
							InlineAttributes.Add(e.Id());
						}
						break;
					case "usefileindexinfilename":
						UseFileIndexInFileName = e.Id() != "false";
						break;
					case "source":
						if (!SourceFiles.Contains(e.Id())) {
							SourceFiles.Add(e.Id());
						}
						break;
				}
			}
		}

		/// <summary>
		/// 	prepares prooject to keep self made simple text logger for compilation
		/// </summary>
		/// <param name="level"> The level. </param>
		/// <remarks>
		/// </remarks>
		public void SetSelfLog(LogLevel level = LogLevel.Info) {
			_sw = new StringWriter();
			UserLog = BaseTextWriterLogWriter.CreateLog(GetType().ToString(), _sw);
			// new LoggerBasedUserLog();//new TextWriterLogListener(_sw, null, level);
		}

		/// <summary>
		/// 	Gets the UserLog.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public string GetLog() {
			return null == _sw ? "Self UserLog was not writed" : _sw.ToString();
		}

		/// <summary>
		/// </summary>
		private StringWriter _sw;
	}
}