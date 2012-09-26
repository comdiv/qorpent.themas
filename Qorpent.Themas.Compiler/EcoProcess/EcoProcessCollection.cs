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
// Original file : EcoProcessCollection.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.EcoProcess {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class EcoProcessCollection {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="EcoProcessCollection" /> class.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public EcoProcessCollection() {
			EmptyReferences = new List<ProcessThemaRef>();
			Errors = new List<Exception>();
		}

		/// <summary>
		/// 	Gets the <see cref="Qorpent.Themas.Compiler.EcoProcess.Process" /> with the specified code.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public Process this[string code] {
			get { return Index.ContainsKey(code) ? Index[code] : null; }
		}

		/// <summary>
		/// 	Gets the index.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, Process> Index {
			get { return _index; }
		}

		/// <summary>
		/// 	Gets all.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IEnumerable<Process> All {
			get { return _index.Values.ToArray(); }
		}

		/// <summary>
		/// 	Gets the empty references.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public List<ProcessThemaRef> EmptyReferences { get; private set; }

		/// <summary>
		/// 	Gets the errors.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public List<Exception> Errors { get; private set; }

		/// <summary>
		/// 	Resolves the structure and org nodes.
		/// </summary>
		/// <param name="orgnodes"> The orgnodes. </param>
		/// <param name="themas"> The themas. </param>
		/// <remarks>
		/// </remarks>
		public void ResolveStructureAndOrgNodes(OrgNodeCollection orgnodes, ThemaDescriptor[] themas) {
			BindProcessIns();
			if (null != orgnodes) {
				BindOrgNodes(orgnodes);
			}
			if (null != themas) {
				BindThemas(themas);
			}
		}

		/// <summary>
		/// 	Binds the process ins.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void BindProcessIns() {
			foreach (var process in Index.Values) {
				foreach (var pi in process.InDepends) {
					if (pi.Code == process.Code) {
						Errors.Add(
							new EcoProcessException("Процесс " + process.Code + " ссылается на самого себя ", pi.Xml)
							);
						continue;
					}
					var dp = Index.Values.FirstOrDefault(x => x.Code == pi.Code);

					if (null == dp) {
						Errors.Add(new EcoProcessException("Процесс " + process.Code + " ссылается на несуществующий процесс " +
						                                   pi.Code, pi.Xml));
						continue;
					}
					pi.Process = dp;
					dp.OutDepends.Add(new ProcessInRef {Code = process.Code, Process = process, Xml = pi.Xml});
				}
			}
		}

		/// <summary>
		/// 	Binds the themas.
		/// </summary>
		/// <param name="themas"> The themas. </param>
		/// <remarks>
		/// </remarks>
		private void BindThemas(ThemaDescriptor[] themas) {
			foreach (var process in Index.Values) {
				foreach (var tr in process.ThemaRefs) {
					tr.ContainingProcess = process;
					var thema = themas.FirstOrDefault(x => x.Code == tr.Code && x.IsWorking);
					if (null == thema) {
						EmptyReferences.Add(tr);
					}
					else {
						tr.Thema = thema;
					}
				}
			}
		}

		/// <summary>
		/// 	Binds the org nodes.
		/// </summary>
		/// <param name="orgnodes"> The orgnodes. </param>
		/// <remarks>
		/// </remarks>
		private void BindOrgNodes(OrgNodeCollection orgnodes) {
			foreach (var process in Index.Values) {
				var orgnode = orgnodes[process.OrgNodeCode];
				if (null == orgnode) {
					Errors.Add(
						new EcoProcessException("Процесс " + process.Code + " ссылается на несуществующий исполнительный узел " +
						                        process.OrgNodeCode, process.Xml));
					continue;
				}
				process.OrgNode = orgnode;
				orgnode.Processes.Add(process);
			}
		}


		/// <summary>
		/// 	Loads from XML.
		/// </summary>
		/// <param name="src"> The SRC. </param>
		/// <remarks>
		/// </remarks>
		public void LoadFromXml(XElement src) {
			var elements = src.Descendants("process");
			foreach (var e in elements) {
				AddElementFromXml(e);
			}
		}


		/// <summary>
		/// 	Adds the element from XML.
		/// </summary>
		/// <param name="e"> The e. </param>
		/// <remarks>
		/// </remarks>
		private void AddElementFromXml(XElement e) {
			var process = new Process();
			e.Apply(process);
			process.Xml = e;
			if (process.Code.IsEmpty()) {
				Errors.Add(
					new EcoProcessException("Процессу " + e.Describe().ToWhereString() + " не сопоставлен исполнительный узел",
					                        process.Xml));
				return;
			}
			process.OrgNodeCode = e.Attr("orgnode");
			if (e.Parent != null && e.Parent.Name.LocalName == "orgnode") {
				process.OrgNodeCode = e.Parent.Id();
			}
			if (process.OrgNodeCode.IsEmpty()) {
				Errors.Add(
					new EcoProcessException("Процессу " + process.Code + " не сопоставлен исполнительный узел", process.Xml));
				return;
			}
			foreach (var t in e.Elements("thema")) {
				var themaref = new ProcessThemaRef();
				t.Apply(themaref);
				if (themaref.Code.IsEmpty()) {
					Errors.Add(
						new EcoProcessException("Процесс " + process.Code + " имеет путсую ссылку на тему ", process.Xml)
						);
					continue;
				}
				if (null != process.ThemaRefs.FirstOrDefault(x => x.Code == themaref.Code)) {
					Errors.Add(
						new EcoProcessException("Процесс " + process.Code + " имеет двойную ссылку на тему " + themaref.Code, process.Xml));
					continue;
				}
				if (themaref.OutLock && !themaref.OutView) {
					Errors.Add(
						new EcoProcessException("Процесс " + process.Code + " имеет неверную ссылку на тему " + themaref.Code +
						                        " - тема помечена к блокировке, но к экспорту", process.Xml));
					continue;
				}
				process.ThemaRefs.Add(themaref);
			}
			foreach (var p in e.Elements("in")) {
				var processin = new ProcessInRef();
				p.Apply(processin);
				processin.Xml = p;
				if (processin.Code.IsEmpty()) {
					Errors.Add(
						new EcoProcessException("Процесс " + process.Code + " имеет путсую ссылку процесс", process.Xml));
					continue;
				}
				if (null != process.InDepends.FirstOrDefault(x => x.Code == processin.Code)) {
					Errors.Add(
						new EcoProcessException("Процесс " + process.Code + " имеет двойную ссылку на процесс " + processin.Code,
						                        process.Xml));
					continue;
				}
				process.InDepends.Add(processin);
			}
			Add(process);
		}

		/// <summary>
		/// 	Adds the specified process.
		/// </summary>
		/// <param name="process"> The process. </param>
		/// <remarks>
		/// </remarks>
		public void Add(Process process) {
			if (Index.ContainsKey(process.Code)) {
				Errors.Add(
					new EcoProcessException("Структура процессов включет в себя дублирующийся код процесса :" + process.Code,
					                        process.Xml));
				return;
			}
			Index[process.Code] = process;
		}

		/// <summary>
		/// </summary>
		private readonly IDictionary<string, Process> _index = new Dictionary<string, Process>();
	}
}