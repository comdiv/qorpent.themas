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
// Original file : CheckLibraryResolutionStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// 	verify step to check that all uselib resolved
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class CheckLibraryResolutionStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			IDictionary<string, string> nativelibraryindex = new Dictionary<string, string>();
			foreach (var t in Context.Themas.Values.Where(x => x.IsLibrary)) {
				foreach (var e in t.Items) {
					var c = e.Key.Split('.')[0];
					nativelibraryindex[c] = t.Code + "." + e.Key;
				}
			}
			foreach (var t in Context.Themas.Values) {
				foreach (var i in t.Items) {
					var ic = i.Key;
					var libref = i.Value.Elements("uselib").ToArray();
					foreach (var r in libref) {
						var refid = r.Id();
						var desc = r.Describe();
						if (refid.IsEmpty()) {
							r.Remove();
							continue;
						}
						var path = refid.SmartSplit(false, true, '.');
						if (path.Count == 1) {
							if (nativelibraryindex.ContainsKey(path[0])) {
								r.ReplaceWith(new XElement("uselib", new XAttribute("code", nativelibraryindex[path[0]])));
								continue;
							}
							path.Add(i.Key.Split('.')[0]);
						}
						if (path.Count == 2) {
							path.Add(i.Key.Split('.')[1]);
						}

						if (3 != path.Count) {
							if (Makeerror(r, t, ic, desc, "неправильный путь к библиотеке", 2)) {
								return;
							}
							continue;
						}
						if (!Context.Themas.ContainsKey(path[0])) {
							if (Makeerror(r, t, ic, desc, "отсутствует целевая тема " + path[0], 3)) {
								return;
							}
							continue;
						}
						var rt = Context.Themas[path[0]];
						if (!rt.IsWorking) {
							if (Makeerror(r, t, ic, desc, "целевая тема должна быть рабочей " + path[0], 4)) {
								return;
							}
							continue;
						}
						var ik = path[1] + "." + path[2];
						if (!rt.Items.ContainsKey(ik)) {
							if (Makeerror(r, t, ic, desc, "отсутствует целевой элемент темы " + ik, 5)) {
								return;
							}
							continue;
						}
						var fullpath = string.Join(".", path.ToArray());
						r.ReplaceWith(new XElement("uselib", new XAttribute("code", fullpath)));
					}
				}
			}
		}

		/// <summary>
		/// 	Makeerrors the specified r.
		/// </summary>
		/// <param name="r"> The r. </param>
		/// <param name="t"> The t. </param>
		/// <param name="ic"> The ic. </param>
		/// <param name="desc"> The desc. </param>
		/// <param name="error"> The error. </param>
		/// <param name="code"> The code. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private bool Makeerror(XNode r, ThemaDescriptor t, string ic, XmlExtensions.XmlElementDescriptor desc, string error,
		                       int code) {
			r.Remove();
			var message = string.Format("error in uselib resolution in {0}/{1} {2}:{3} - {4}", t.Code, ic, desc.File,
			                            desc.Line,
			                            error);
			var ecprefix = "WRLIBR0";
			var level = ErrorLevel.Warning;
			if (Context.Project.NonResolvedLibraryIsError) {
				ecprefix = "ERLIBR0";
				level = ErrorLevel.Error;
			}
			var ecode = ecprefix + code;
			AddError(level, message, ecode, null, desc.File, desc.Line);
			if (ErrorLevel.Error == level) {
				UserLog.Error(message);
			}
			else {
				UserLog.Warn(message);
			}
			return ErrorLevel.Error == level;
		}
	}
}