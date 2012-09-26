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
// Original file : GenerateHtmlReportStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.IO;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	step for generating html report about compiled instance
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class GenerateHtmlReportStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			if (Context.Project.OutputFolder.IsEmpty()) {
				return;
			}
			_resolver = Application.Current.Files.GetResolver();
			_currentresolver = Context.Project.Resolver ?? _resolver;
			_folder = _currentresolver.Resolve(Context.Project.OutputFolder, false);
			Directory.CreateDirectory(_folder);
			_filename = Path.Combine(_folder, "themas_report.html");
			var content = GenerateContent();
			File.WriteAllText(_filename, "<!DOCTYPE html>\r\n" + content);
		}

		/// <summary>
		/// 	Generates the content.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private string GenerateContent() {
			_root = new XElement("html");
			_current = _root;
			_rowcount = 0;
			RenderHead();
			var groups =
				Context.Themas.Values.Where(x => x.IsGroup).OrderBy(x => string.Format("{0:000000} {1}", x.EffectiveIndex, x.Code)).
					ToArray();
			var visited = new List<ThemaDescriptor>();
			_current.Add(Context.IsError
				             ? new XElement("p", new XAttribute("class", "error"), "Компиляция не завершена, смотрите ошибки!")
				             : new XElement("p", "Компиляция завершена успешно"));
			RenderErrorTable();
			if (!Context.IsError) {
				StartThemaTable();
				foreach (var g in groups.Except(visited)) {
					RenderGroup(g, visited);
				}
				RenderEmptyGroup(visited);

				EndThemaTable();
			}
			return _root.ToString();
		}

		/// <summary>
		/// 	Ends the thema table.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void EndThemaTable() {
			_current = _root.Element("body");
		}

		/// <summary>
		/// 	Starts the thema table.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void StartThemaTable() {
			_current.Add(new XElement("h1", "Откомпилированные темы"));
			_current.Add(new XElement("table", new XAttribute("class", "themas"),
			                          new XElement("thead",
			                                       new XElement("tr",
			                                                    new XElement("th", "№"),
			                                                    new XElement("th", "Инд."),
			                                                    new XElement("th", "Код"),
			                                                    new XElement("th", "Название"),
			                                                    new XElement("th", "Кластер"),
			                                                    new XElement("th", "Процессы"),
			                                                    new XElement("th", "Роли"),
			                                                    new XElement("th", "Файл"),
			                                                    new XElement("th", "Строка"),
			                                                    new XElement("th", "Спец."),
			                                                    new XElement("th", "Гр."),
			                                                    new XElement("th", "Род."),
			                                                    new XElement("th", "Состав")
				                                       )
				                          ),
			                          _current = new XElement("tbody")
				             ));
		}

		/// <summary>
		/// 	Renders the error table.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void RenderErrorTable() {
			if (0 == Context.Errors.Count) {
				return;
			}
			_current.Add(new XElement("h1", "Ошибки и предупреждения"));
			_current.Add(new XElement("table", new XAttribute("class", "erros"),
			                          new XElement("thead",
			                                       new XElement("tr",
			                                                    new XElement("th", "№"),
			                                                    new XElement("th", "Тип"),
			                                                    new XElement("th", "Код"),
			                                                    new XElement("th", "Сообщение"),
			                                                    new XElement("th", "Файл"),
			                                                    new XElement("th", "Строка"),
			                                                    new XElement("th", "Исключение")
				                                       )
				                          ),
			                          _current = new XElement("tbody")
				             ));
			foreach (var e in Context.Errors) {
				_current.Add(
					new XElement("tr",
					             new XAttribute("class", "elevel_" + e.Level),
					             new XElement("td", ++_erroridx),
					             new XElement("td", e.Level),
					             new XElement("td", e.ErrorCode),
					             new XElement("td", e.Message),
					             new XElement("td", new XElement("a",
					                                             new XAttribute("href",
					                                                            "view-source:file:///" + _folder + "/.." + e.SourceFile),
					                                             new XAttribute("target", "_blank"),
					                                             e.SourceFile)),
					             new XElement("td", e.Line),
					             XElement.Parse("<td class='exception'>" +
					                            (e.Exception == null
						                             ? ""
						                             : e.Exception.ToString().Replace("<", "&lt;").Replace("&", "&amp;").Replace(
							                             Environment.NewLine, "<br/>"))
					                            + "</td>"
						             )
						)
					);
			}
			_current = _root.Element("body");
		}

		/// <summary>
		/// 	Renders the head.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void RenderHead() {
			_root.Add(new XElement("head",
			                       new XElement("meta", new XAttribute("content", "text/html; charset=utf-8"),
			                                    new XAttribute("http-equiv", "Content-Type")),
			                       new XElement("style", new XAttribute("type", "text/css"),
			                                    new XText(
				                                    @"
body { 
	font-family : Arial;
	font-size : 8pt;
}
table {
	border-collapse : collapse;
}
td, th {
	border : solid 1px gray;
	padding : 3px;
}
tr.level_0 {
	font-weight : bold;
	font-size : 12pt;
	background-color : #cccccc;
}
tr.level_1 {
	font-size : 10pt;
	background-color : #eeeeee;
}
tr.level_1 td.name {
	padding-left : 40px;
}

tr.level_2 td.name {
	padding-left : 80px;
}
td.exception {
	font-size:8pt;
	font-style:italic;
}
tr.elevel_Error, tr.elevel_Fatal {
	background-color: #ffcccc;
}
tr.elevel_Warning {
	background-color: #ccffff;
}
.Error {
 color:red;
}

"))));
			_root.Add(_current = new XElement("body"));
		}

		/// <summary>
		/// 	Renders the group.
		/// </summary>
		/// <param name="t"> The t. </param>
		/// <param name="visited"> The visited. </param>
		/// <remarks>
		/// </remarks>
		private void RenderGroup(ThemaDescriptor t, ICollection<ThemaDescriptor> visited) {
			visited.Add(t);
			_current.Add(
				new XElement("tr",
				             new XAttribute("class", "group  level_0"),
				             new XElement("td", ++_rowcount),
				             new XElement("td", t.EffectiveIndex),
				             new XElement("td", new XElement("a",
				                                             new XAttribute("href",
				                                                            "view-source:file:///" + _folder + "/" + t.Code +
				                                                            ".thema.bxl"),
				                                             new XAttribute("target", "_blank"),
				                                             t.Code)),
				             new XElement("td", new XAttribute("class", "name"), t.Name),
				             new XElement("td", new XAttribute("class", "cluster"), t.Cluster),
				             new XElement("td", new XAttribute("class", "processes"), t.EcoProcess),
				             new XElement("td", new XAttribute("class", "roles"), t.Role),
				             new XElement("td", t.File),
				             new XElement("td", t.Line),
				             new XElement("td", new XAttribute("colspan", 10))
					)
				);
			var roots =
				Context.Themas.Values.Where(x => x.GroupCode == t.Code && x.ParentCode.IsEmpty()).OrderBy(
					x => string.Format("{0:000000} {1}", x.EffectiveIndex, x.Code)).ToArray();
			foreach (var r in roots) {
				RenderThema(r, visited, 1);
			}
		}

		/// <summary>
		/// 	Renders the thema.
		/// </summary>
		/// <param name="t"> The t. </param>
		/// <param name="visited"> The visited. </param>
		/// <param name="level"> The level. </param>
		/// <remarks>
		/// </remarks>
		private void RenderThema(ThemaDescriptor t, ICollection<ThemaDescriptor> visited, int level) {
			if (visited.Contains(t)) {
				return;
			}
			visited.Add(t);
			_current.Add(
				new XElement("tr",
				             new XAttribute("class", "thema level_" + level),
				             new XElement("td", ++_rowcount),
				             new XElement("td", t.EffectiveIndex),
				             new XElement("td", new XElement("a",
				                                             new XAttribute("href",
				                                                            "view-source:file:///" + _folder + "/" + t.Code +
				                                                            ".thema.bxl"),
				                                             new XAttribute("target", "_blank"),
				                                             t.Code)),
				             new XElement("td", new XAttribute("class", "name"), t.Name),
				             new XElement("td", new XAttribute("class", "cluster"), t.Cluster),
				             new XElement("td", new XAttribute("class", "processes"), t.EcoProcess),
				             new XElement("td", new XAttribute("class", "roles"), t.Role),
				             new XElement("td", t.File),
				             new XElement("td", t.Line),
				             new XElement("td", BuildSpec(t)),
				             new XElement("td", t.GroupCode),
				             new XElement("td", t.ParentCode),
				             new XElement("td", GetContent(t))
					)
				);
			var child =
				Context.Themas.Values.Where(x => x.ParentCode == t.Code).OrderBy(
					x => string.Format("{0:000000} {1}", x.EffectiveIndex, x.Code)).ToArray();
			foreach (var r in child) {
				RenderThema(r, visited, level + 1);
			}
		}

		/// <summary>
		/// 	Gets the content.
		/// </summary>
		/// <param name="t"> The t. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private static IEnumerable<XNode> GetContent(ThemaDescriptor t) {
			return t.Items.Select(e => new XText(e.Key + "; ")).OfType<XNode>();
		}

		/// <summary>
		/// 	Builds the spec.
		/// </summary>
		/// <param name="t"> The t. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private static IEnumerable<XNode> BuildSpec(ThemaDescriptor t) {
			if (!t.IsLibrary) {
				yield break;
			}
			yield return new XText("Библиотека");
			yield return new XElement("br");
		}

		/// <summary>
		/// 	Renders the empty group.
		/// </summary>
		/// <param name="visited"> The visited. </param>
		/// <remarks>
		/// </remarks>
		private void RenderEmptyGroup(ICollection<ThemaDescriptor> visited) {
			_current.Add(
				new XElement("tr",
				             new XAttribute("class", "group level_0"),
				             new XElement("td", ++_rowcount),
				             new XElement("td", 0),
				             new XElement("td", "EMPTY_STUB"),
				             new XElement("td", new XAttribute("class", "name"), "Пустая группа"),
				             new XElement("td", new XAttribute("class", "cluster"), ""),
				             new XElement("td", new XAttribute("class", "processes"), ""),
				             new XElement("td", new XAttribute("class", "roles"), ""),
				             new XElement("td", ""),
				             new XElement("td", ""),
				             new XElement("td", new XAttribute("colspan", 10))
					)
				);
			var roots =
				Context.Themas.Values.Where(x => x.GroupCode == "" && x.ParentCode.IsEmpty()).OrderBy(
					x => string.Format("{0:000000} {1}", x.EffectiveIndex, x.Code)).ToArray();
			foreach (var r in roots) {
				RenderThema(r, visited, 1);
			}
		}

		/// <summary>
		/// </summary>
		private XElement _current;

		/// <summary>
		/// </summary>
		private IFileNameResolver _currentresolver;

		/// <summary>
		/// </summary>
		private int _erroridx;

		/// <summary>
		/// </summary>
		private string _filename;

		/// <summary>
		/// </summary>
		private string _folder;

		/// <summary>
		/// </summary>
		private IFileNameResolver _resolver;

		/// <summary>
		/// </summary>
		private XElement _root;

		/// <summary>
		/// </summary>
		private int _rowcount;
	}
}