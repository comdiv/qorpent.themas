using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Comdiv.ThemaLoader {
	public class Thema : IThema {
		private string[] _forgroup;

		public Thema() {
			InLinks = new List<IThemaLink>();
			OutLinks = new List<IThemaLink>();
			Parameters = new Dictionary<string, string>();
			Items = new List<IThemaItem>();
		}

		public IEnumerable<IFormThemaItem> Forms {
			get { return Items.OfType<IFormThemaItem>(); }
		}

		public IEnumerable<IReportThemaItem> Reports {
			get { return Items.OfType<IReportThemaItem>(); }
		}

		#region IThema Members

		public int Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string ForGroup { get; set; }
		public bool UseHistory { get; set; }
		public int FirstYear { get; set; }
		public int LastYear { get; set; }

		public string[] ListForGroup {
			get {
				if (null == _forgroup) {
					lock (this) {
						_forgroup = ForGroup.SmartSplit(false, true, '/', ';').ToArray();
					}
				}
				return _forgroup;
			}
		}

		public string FixedObject { get; set; }

		public int Idx { get; set; }
		public int FileIdx { get; set; }
		public string Role { get; set; }
		public string Cluster { get; set; }
		public string EcoProcess { get; set; }
		public XElement XmlSource { get; set; }

		public IThemaItem GetItem(string code) {
			return Items.FirstOrDefault(x => x.Code == code);
		}

		public IFormThemaItem GetForm(string code) {
			if (!code.EndsWith(".in")) code = code + ".in";
			return Items.OfType<IFormThemaItem>().FirstOrDefault(x => x.Code == code);
		}

		public IReportThemaItem GetReport(string code) {
			if (!code.EndsWith(".out")) code = code + ".out";
			return Items.OfType<IReportThemaItem>().FirstOrDefault(x => x.Code == code);
		}

		public string EffectiveIndex {
			get { return string.Format("{0:00000000}{1}", (Idx == 0 ? 9999 : Idx)*10000 + FileIdx, Code); }
		}

		public bool Authorized(string usr = null) {
			return usr.noContent() || Role.noContent() || Factory.Authorize(usr, this);
		}


		public IThemaFactory Factory { get; set; }
		public IList<IThemaLink> OutLinks { get; private set; }
		public IList<IThemaLink> InLinks { get; private set; }
		public IList<IThemaItem> Items { get; private set; }

		public IDictionary<string, string> Parameters { get; private set; }

		public T GetParam<T>(string name, T def) {
			if (Parameters.ContainsKey(name)) return Parameters[name].To<T>();
			return default(T);
		}


		public IThema Group {
			get { return GetTargetThema("group"); }
		}

		public IEnumerable<IThema> GroupMembers {
			get { return GetSourceThemas("group"); }
		}

		public IThema Parent {
			get { return GetTargetThema("parent"); }
		}

		public IEnumerable<IThema> Children {
			get { return GetSourceThemas("parent"); }
		}

		public bool IsGroup { get; set; }

		public IThema GetTargetThema(string code) {
			var grplink = OutLinks.FirstOrDefault(x => x.Type == code);
			if (null == grplink) return null;
			return grplink.Target;
		}

		public IEnumerable<IThema> GetSourceThemas(string code) {
			return InLinks.Where(x => x.Type == code).Select(x => x.Source);
		}

		public void SetupFromSourceXml() {
			setupParameters();
			setupLinks();
			setupForms();
			setupReports();
		}

		#endregion

		private void setupReports() {
			foreach (var re in XmlSource.Elements("out")) {
				var r = new ReportThemaItem();
				r.XmlSource = re;
				r.Thema = this;
				re.Apply(r);
				r.SetupFromSourceXml();
				Items.Add(r);
			}
		}

		private void setupForms() {
			foreach (var re in XmlSource.Elements("in")) {
				var r = new FormThemaItem();
				r.XmlSource = re;
				r.Thema = this;
				re.Apply(r);
				r.SetupFromSourceXml();
				Items.Add(r);
			}
		}

		private void setupLinks() {
			foreach (var le in XmlSource.Elements("link")) {
				var tl = new ThemaLink();
				tl.Type = le.Attr("type");
				tl.SourceCode = Code;
				tl.Source = this;
				tl.TargetCode = le.Attr("target");
				tl.XmlSource = le;
				OutLinks.Add(tl);
			}
		}

		private void setupParameters() {
			foreach (var a in XmlSource.Attributes()) {
				Parameters[a.Name.LocalName] = a.Value;
			}
		}
	}
}