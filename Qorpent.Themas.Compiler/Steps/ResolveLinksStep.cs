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
// Original file : ResolveLinksStep.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler.Steps {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ResolveLinksStep : ThemaCompilerStep {
		/// <summary>
		/// 	Internals the process.
		/// </summary>
		/// <remarks>
		/// </remarks>
		protected override void InternalProcess() {
			ResolveLinkImport();
			ResolveLinkContent();
			CheckLinkTarget();
			CheckIntegrity();
		}

		/// <summary>
		/// 	Checks the integrity.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void CheckIntegrity() {
			foreach (var t in Context.Themas.Values.ToArray()) {
				foreach (var l in t.Links.ToArray()) {
					var error = l.Type.CustomValidation(l, t);
					if (null == error) {
						continue;
					}
					Context.Errors.Add(error);
					t.Links.Remove(l);
				}
			}
		}

		/// <summary>
		/// 	Checks the link target.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void CheckLinkTarget() {
			foreach (var t in Context.Themas.Values) {
				foreach (var l in t.Links.ToArray()) {
					if (null == l.Target) {
						l.Deleted = true;
						if (l.Type.RequireResolution) {
							var message =
								string.Format("thema {0} has link to target thema {1}, but this thema not exist, and that is not allowed",
								              t.Code, l.TargetCode);
							AddError(ErrorLevel.Error, message, "ERLINK01", null, l.File, l.Line);
							t.Links.Remove(l);
						}
					}
					else if (! l.Target.IsWorking) {
						l.Deleted = true;
						if (l.Type.RequireResolution) {
							var message =
								string.Format(
									"thema {0} has link to target thema {1}, but this thema is not working thema, that is not allowed",
									t.Code, l.TargetCode);
							AddError(ErrorLevel.Error, message, "ERLINK02", null, l.File, l.Line);
							t.Links.Remove(l);
						}
					}
					else if (l.Type.RequireAttribute.IsNotEmpty()) {
						var attribute = l.Type.RequireAttribute;
						var targetattr = l.Target.GetParam(attribute);
						if (!targetattr.ToBool()) {
							var message =
								string.Format("thema {0} has link to target thema {1}, but this thema is not marked with attribute {2}",
								              t.Code, l.TargetCode, attribute);
							AddError(ErrorLevel.Error, message, "ERLINK03", null, l.File, l.Line);
							t.Links.Remove(l);
						}
					}
				}

				foreach (var l in t.Links.ToArray()) {
					if (!l.Deleted) {
						continue;
					}
					var message =
						string.Format("thema {0} has link to target thema {1}, but this thema not exist",
						              t.Code, l.TargetCode);
					AddError(ErrorLevel.Warning, message, "WRLINK01", null, l.File, l.Line);
					t.Links.Remove(l);
				}
			}
		}

		/// <summary>
		/// 	Resolves the link import.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void ResolveLinkImport() {
			foreach (var t in Context.Themas.Values) {
				ResolveLinks(t.Code);
			}
		}

		/// <summary>
		/// 	Resolves the links.
		/// </summary>
		/// <param name="t"> The t. </param>
		/// <remarks>
		/// </remarks>
		private void ResolveLinks(string t) {
			if (!Context.Themas.ContainsKey(t)) {
				return;
			}
			var th = Context.Themas[t];
			if (null == th) {
				return;
			}
			if (th.LinksResolved) {
				return;
			}
			foreach (var i in th.Imports) {
				if (!Context.Themas.ContainsKey(i)) {
					continue;
				}
				var it = Context.Themas[i];
				ResolveLinks(i);
				foreach (var il in it.Links.Where(il => il.Type.Inheritable || it.IsAbstract)) {
					th.Links.Add(il.Import(th));
				}
			}
			foreach (var l in th.SelfLinks) {
				th.Links.Add(l.Import(th));
			}
			var idx = 1;
			foreach (var link in th.Links) {
				if (link.Type.Singleton) {
					if (null != th.Links.Skip(idx).FirstOrDefault(x => x.Type.Code == link.Type.Code)) {
						link.Deleted = true;
					}
				}
				idx++;
			}
			foreach (var l in th.Links.ToArray().Where(l => l.Deleted)) {
				th.Links.Remove(l);
			}
			th.LinksResolved = true;
		}

		/// <summary>
		/// 	Resolves the content of the link.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void ResolveLinkContent() {
			foreach (var t in Context.Themas.Values) {
				foreach (var l in t.Links) {
					l.TargetCode = t.Substitute(l.TargetCode);
					if (l.TargetCode.IsEmpty()) {
						l.Deleted = true; // here we can delete EMPTY links - it means safe operation
						continue;
					}
					l.Value = t.Substitute(l.Value);
					if (!Context.Themas.ContainsKey(l.TargetCode)) {
						continue;
					}
					l.Target = Context.Themas[l.TargetCode];
					l.Target.BackLinks.Add(l);
				}
				foreach (var l in t.Links.ToArray().Where(l => l.Deleted)) {
					t.Links.Remove(l);
				}
			}
		}
	}
}