using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Comdiv.ThemaLoader.ZetaIntegration;

namespace Comdiv.ThemaLoader {
	public class ThemaItem : IThemaItem {
		private readonly IDictionary<string, string> _nativeparameters = new Dictionary<string, string>();
		private int[] _listforperiods;
		private PeriodRedirectDefinition[] _listperiodredirect;

		public ThemaItem() {
			LibraryLinks = new List<ILibraryLink>();
			InLibraryUsage = new List<ILibraryLink>();
			Parameters = new List<ThemaItemParameter>();
			Elements = new List<IThemaItemElement>();
		}

		public List<IThemaItemElement> Elements { get; private set; }

		#region IThemaItem Members

		public IThema Thema { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string Role { get; set; }
		public string ForPeriods { get; set; }
		public string PeriodRedirect { get; set; }

		public string FullCode {
			get { return Thema.Code + "." + Code; }
		}

		public string ForGroup { get; set; }


		public int[] ListForPeriods {
			get {
				if (null == _listforperiods) {
					lock (this) {
						_listforperiods = ForPeriods.SmartSplit().Select(x => x.ToInt()).ToArray();
					}
				}
				return _listforperiods;
			}
		}

		public PeriodRedirectDefinition[] ListPeriodRedirect {
			get {
				if (null == _listperiodredirect) {
					lock (this) {
						IList<PeriodRedirectDefinition> tmp = new List<PeriodRedirectDefinition>();
						var objgroups = PeriodRedirect.SmartSplit(false, true, '|');
						foreach (var objgroup in objgroups) {
							var objgroupsplit = objgroup.Split(':');
							var key = "";
							var rules = objgroupsplit[0];
							if (objgroupsplit.Length > 1) {
								rules = objgroupsplit[1];
								key = objgroupsplit[0];
							}
							var _rules = rules.SmartSplit();
							foreach (var rule in _rules) {
								var rulesplit = rule.Split('=');
								var r = new PeriodRedirectDefinition();
								r.ForGroup = key;
								r.Source = rulesplit[0].ToInt();
								r.Target = rulesplit[1].ToInt();
								tmp.Add(r);
							}
						}

						_listperiodredirect = tmp.ToArray();
					}
				}
				return _listperiodredirect;
			}
		}

		public string FixedObject { get; set; }

		public XElement XmlSource { get; set; }

		public bool Authorized(string usr = null) {
			return usr.noContent() || Role.noContent() || Thema.Factory.Authorize(usr, this);
		}

		public IList<ILibraryLink> LibraryLinks { get; private set; }
		public IList<ILibraryLink> InLibraryUsage { get; private set; }
		public IList<ThemaItemParameter> Parameters { get; private set; }

		public IDictionary<string, string> NativeXmlParameters {
			get { return _nativeparameters; }
		}

		public IZetaEntityIntermediate ZetaObject { get; set; }

		public IEnumerable<T> GetElements<T>() where T : IThemaItemElement {
			return Elements.OfType<T>();
		}

		public IEnumerable<IThemaItemElement> GetElements(string type) {
			return Elements.Where(x => x.Type == type);
		}

		public IEnumerable<IColumnItemElement> GetColumns() {
			return GetElements<IColumnItemElement>();
		}

		public IEnumerable<IRowItemElement> GetRows() {
			return GetElements<IRowItemElement>();
		}

		public IEnumerable<IObjItemElement> GetObjs() {
			return GetElements<IObjItemElement>();
		}

		#endregion

		public virtual void SetupFromSourceXml() {
			foreach (var attribute in XmlSource.Attributes()) {
				NativeXmlParameters[attribute.Name.LocalName] = attribute.Value;
			}
			setupLibraryLinks();
			setupParameters();
			setupElements();
		}

		private void setupElements() {
			foreach (var e in XmlSource.Elements()) {
				switch (e.Name.LocalName) {
					case "param":
						break;
					case "var":
						break;
					case "lockdepend":
						break;
					case "uselib":
						break;

					case "col":
						goto case "column";
					case "column":
						Elements.Add(new ColumnItemElement(e));
						break;
					case "row":
						Elements.Add(new RowItemElement(e));
						break;
					case "obj":
						goto case "object";
						break;
					case "object":
						Elements.Add(new ObjItemElement(e));
						break;
					default:
						Elements.Add(new ThemaItemElement(e));
						break;
				}
			}
			foreach (var e in Elements) {
				e.ContainingItem = this;
			}
		}

		private void setupParameters() {
			foreach (var e in XmlSource.Elements("param")) {
				var p = loadparameter(e);

				p.Visible = false;
				p.Readonly = true;
			}
			foreach (var e in XmlSource.Elements("var")) {
				var p = loadparameter(e);
				p.Visible = true;
				p.Readonly = false;
			}
		}

		private ThemaItemParameter loadparameter(XElement e) {
			var p = new ThemaItemParameter();
			p.XmlSource = e;

			e.Apply(p);
			p.ContainingItem = this;
			if (p.DefaultValue.noContent()) {
				p.DefaultValue = e.Value;
			}
			Parameters.Add(p);
			return p;
		}

		private void setupLibraryLinks() {
			foreach (var e in XmlSource.Elements("uselib")) {
				var ll = new LibraryLink();
				ll.SourceCode = Thema.Code + "." + Code;
				ll.Source = this;
				ll.TargetCode = e.Attr("code");
				LibraryLinks.Add(ll);
			}
		}
	}
}