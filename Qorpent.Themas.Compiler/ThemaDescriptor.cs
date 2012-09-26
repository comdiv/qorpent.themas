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
// Original file : ThemaDescriptor.cs
// Project: Qorpent.Themas.Compiler
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Themas.Compiler {
	/// <summary>
	/// 	Xml based thema descriptor which is thema definition at compile time
	/// </summary>
	public class ThemaDescriptor {
		/// <summary>
		/// 	mask for finding param references in @name mode
		/// </summary>
		internal static readonly Regex Atregex = new Regex(@"^@(\w[\w\d_\.]*)$", RegexOptions.Compiled);

		/// <summary>
		/// 	mask to find parameters injections in ${name} mode
		/// </summary>
		internal static readonly Regex Inregex = new Regex(@"\$\{([\w@][\w\d_\.]*)\}", RegexOptions.Compiled);

		/// <summary>
		/// 	creates default descriptor
		/// </summary>
		public ThemaDescriptor() {
			Imports = new List<string>();
			SelfParameters = new Dictionary<string, string>();
			SelfThemaItems = new Dictionary<string, XElement>();
			Items = new Dictionary<string, XElement>();
			ImportedThemaItems = new Dictionary<string, XElement>();
			SelfThemaItemsSets = new Dictionary<string, XElement>();
			ImportedThemaItemsSets = new Dictionary<string, XElement>();
			SelfThemaItemsExtensions = new Dictionary<string, XElement>();
			ImportedThemaItemsExtensions = new Dictionary<string, IList<XElement>>();
			ResolvedParameters = new Dictionary<string, string>();
			Links = new List<ThemaLink>();
			BackLinks = new List<ThemaLink>();
			SelfLinks = new List<ThemaLink>();
		}

		/// <summary>
		/// 	flag that this them ruled by ecoprocess (so security settings will be injected
		/// </summary>
		public bool IsUnderEcoProcess {
			get { return GetParam("ecoprocess").IsNotEmpty(); }
		}

		/// <summary>
		/// 	code of eco process that holds this thema (both A and B)
		/// </summary>
		public string EcoProcess {
			get { return GetParam("ecoprocess") ?? ""; }
			set { ResolvedParameters["ecoprocess"] = value; }
		}

		/// <summary>
		/// 	process that holds input of form group A
		/// </summary>
		public string OwnerProcessA {
			get { return GetParam("ownerprocessA") ?? ""; }
			set { ResolvedParameters["ownerprocessA"] = value; }
		}

		/// <summary>
		/// 	process that holds input of form group B
		/// </summary>
		public string OwnerProcessB {
			get { return GetParam("ownerprocessB") ?? ""; }
			set { ResolvedParameters["ownerprocessB"] = value; }
		}

		/// <summary>
		/// 	flag that A group will be visible for depended processes
		/// </summary>
		public bool OutViewA {
			get { return GetParam("outviewA").ToBool(); }
			set { ResolvedParameters["outviewA"] = value.ToString(CultureInfo.InvariantCulture).ToLower(); }
		}

		/// <summary>
		/// 	flag that B group will be visible for depended processes
		/// </summary>
		public bool OutViewB {
			get { return GetParam("outviewB").ToBool(); }
			set { ResolvedParameters["outviewB"] = value.ToString(CultureInfo.InvariantCulture).ToLower(); }
		}

		/// <summary>
		/// 	Role (role list) to access thema
		/// </summary>
		public string Role {
			get { return GetParam("role") ?? ""; }
			set { ResolvedParameters["role"] = value; }
		}

		/// <summary>
		/// 	Calculated index for ordering
		/// </summary>
		public int EffectiveIndex {
			get {
				var idx = GetParam("idx").ToInt();
				if (0 == idx) {
					idx = 9999;
				}
				idx = idx*1000;
				var fileidx = GetParam("fileidx").ToInt();
				return idx + fileidx;
			}
		}

		/// <summary>
		/// 	Flag that thema processed with link resolution algorithm to prevent circular void
		/// </summary>
		public bool LinksResolved { get; set; }

		/// <summary>
		/// 	Collection of outgoing links
		/// </summary>
		public IList<ThemaLink> Links { get; private set; }

		/// <summary>
		/// 	Collection of incoming links
		/// </summary>
		public IList<ThemaLink> BackLinks { get; private set; }

		/// <summary>
		/// 	unique code of thema
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 	visible name of thema
		/// </summary>
		public string Name {
			get { return GetParam("name"); }
		}

		/// <summary>
		/// 	source file of thema
		/// </summary>
		public string File {
			get { return SelfParameters.ContainsKey("_file") ? SelfParameters["_file"] : ""; }
		}

		/// <summary>
		/// 	start line of thema
		/// </summary>
		public int Line {
			get { return SelfParameters.ContainsKey("_line") ? Convert.ToInt32(SelfParameters["_line"]) : 0; }
		}

		/// <summary>
		/// 	indicates that this thema is real output thema which can be loaded and used itself
		/// </summary>
		public bool IsWorking {
			get { return (IsLibrary) || !IsAbstract && !IsGeneric && !IsExtension && !GetParam("inactive").ToBool(); }
		}

		/// <summary>
		/// 	Indicate that thema processed with parameter resolution algorithm to avoid circular void
		/// </summary>
		public bool ParametersResolved { get; set; }

		/// <summary>
		/// 	indicate that thema is hiiden shared library for other themas
		/// </summary>
		public bool IsLibrary {
			get { return SelfParameters.ContainsKey("library"); }
		}

		/// <summary>
		/// 	raw XML source of thema
		/// </summary>
		public XElement Fullsource { get; set; }

		/// <summary>
		/// 	indicates that thema is generic (special parameter resolution)
		/// </summary>
		public bool IsGeneric {
			get { return SelfParameters.ContainsKey("generic"); }
		}

		/// <summary>
		/// 	indicates that thema is abstract (used just for importing)
		/// </summary>
		public bool IsAbstract {
			get { return SelfParameters.ContainsKey("abst"); }
		}

		/// <summary>
		/// 	list of imported thema codes
		/// </summary>
		public IList<string> Imports { get; private set; }

		/// <summary>
		/// 	parameters, defined in thema itself
		/// </summary>
		public IDictionary<string, string> SelfParameters { get; private set; }

		/// <summary>
		/// 	parameters, resolved over all imports
		/// </summary>
		public IDictionary<string, string> ResolvedParameters { get; private set; }

		/// <summary>
		/// 	refined extra data of source xml
		/// </summary>
		public XElement ExtraData { get; set; }

		/// <summary>
		/// 	items, defined in thema itself
		/// </summary>
		public IDictionary<string, XElement> SelfThemaItems { get; private set; }

		/// <summary>
		/// 	resolved item set
		/// </summary>
		public IDictionary<string, XElement> Items { get; private set; }

		/// <summary>
		/// 	items, imported from ancesors
		/// </summary>
		public IDictionary<string, XElement> ImportedThemaItems { get; private set; }

		/// <summary>
		/// 	item sets, defined in thema
		/// </summary>
		public IDictionary<string, XElement> SelfThemaItemsSets { get; private set; }

		/// <summary>
		/// 	imported item sets
		/// </summary>
		public IDictionary<string, XElement> ImportedThemaItemsSets { get; private set; }

		/// <summary>
		/// 	item set extensions, defined in thema
		/// </summary>
		public IDictionary<string, XElement> SelfThemaItemsExtensions { get; private set; }

		/// <summary>
		/// 	imported item extensions
		/// </summary>
		public IDictionary<string, IList<XElement>> ImportedThemaItemsExtensions { get; private set; }

		/// <summary>
		/// 	indicates that all elements of items resolved
		/// </summary>
		public bool ElementsResolved { get; set; }

		/// <summary>
		/// 	indicates that thema is extentor for other themas (aspect-like import)
		/// </summary>
		public bool IsExtension { get; set; }

		/// <summary>
		/// 	Xml generated for output
		/// </summary>
		public XElement Xml { get; set; }

		/// <summary>
		/// 	explicit index attribute from xml file
		/// </summary>
		public string Index {
			get { return SelfParameters.ContainsKey("index") ? SelfParameters["index"] : Code; }
		}

		/// <summary>
		/// 	links, defined in thema
		/// </summary>
		public IList<ThemaLink> SelfLinks { get; private set; }

		/// <summary>
		/// 	indicates that thema marks with "isgroup" attribute
		/// </summary>
		public bool IsGroup {
			get { return GetParam("isgroup").ToBool(); }
		}

		/// <summary>
		/// 	shortcut for group link to find code of target
		/// </summary>
		public string GroupCode {
			get {
				var grplink = Links.FirstOrDefault(x => x.Type.Code == "group");
				return null != grplink ? grplink.TargetCode : "";
			}
		}

		/// <summary>
		/// 	shortcut of parent link to quick acess parent thema
		/// </summary>
		public string ParentCode {
			get {
				var parentlink = Links.FirstOrDefault(x => x.Type.Code == "parent");
				return null != parentlink ? parentlink.TargetCode : "";
			}
		}

		/// <summary>
		/// 	resolved Parent thema
		/// </summary>
		public ThemaDescriptor Parent {
			get {
				var parentlink = Links.FirstOrDefault(x => x.Type.Code == "parent");
				return null != parentlink ? parentlink.Target : null;
			}
		}

		/// <summary>
		/// 	resolved Group thema
		/// </summary>
		public ThemaDescriptor Group {
			get {
				var parentlink = Links.FirstOrDefault(x => x.Type.Code == "group");
				return null != parentlink ? parentlink.Target : null;
			}
		}

		/// <summary>
		/// 	Cluster/ cluster list of thema
		/// </summary>
		public string Cluster {
			get { return GetParam("cluster"); }
		}

		/// <summary>
		/// 	quick acess to usual groupped zeta forms
		/// </summary>
		/// <param name="group"> </param>
		/// <returns> </returns>
		public XElement GetForm(string group) {
			return Items.FirstOrDefault(x => x.Key == group + ".in").Value;
		}

		/// <summary>
		/// 	embeds thema parameter into string with @name or ${name} notation
		/// </summary>
		/// <param name="val"> </param>
		/// <param name="src"> </param>
		/// <param name="full"> </param>
		/// <returns> </returns>
		public string Substitute(string val, string src = null, bool full = false) {
			if (0 != val.IndexOf("@", StringComparison.InvariantCulture) && !val.Contains("${")) {
				return val;
			}
			Match m;

			if (0 == val.IndexOf('@') && (m = Atregex.Match(val)).Success) {
				val = GetParam(m.Groups[1].Value, src, full);
			}
			else {
				val = Inregex.Replace(val, i => GetParam(i.Groups[1].Value, src, full));
			}
			return val;
		}

		/// <summary>
		/// 	indicates is item group active
		/// </summary>
		/// <param name="key"> </param>
		/// <returns> </returns>
		public bool IsGroupActive(string key) {
			var val = GetParam("active" + key);
			return val.IsNotEmpty() && "false" != val;
		}

		/// <summary>
		/// 	resoles param reference string to parameter value with generic resolution
		/// </summary>
		/// <param name="code"> </param>
		/// <param name="srcitemid"> </param>
		/// <param name="full"> </param>
		/// <returns> </returns>
		public string GetParam(string code, string srcitemid = null, bool full = false) {
			if (code == "@@") {
				return srcitemid;
			}
			var suffix = "";
			if (null != srcitemid) {
				suffix = srcitemid.Split('.')[0].Last().ToString(CultureInfo.InvariantCulture);
			}
			if (code == "@") {
				return suffix;
			}

			if (code.EndsWith(".")) {
				code = code.Substring(0, code.Length - 1) + suffix;
			}

			if (ResolvedParameters.ContainsKey(code)) {
				var result = ResolvedParameters[code];
				if (full) {
					result = result.Replace("${@}", suffix);
				}
				return result;
			}
			return "";
		}
	}
}