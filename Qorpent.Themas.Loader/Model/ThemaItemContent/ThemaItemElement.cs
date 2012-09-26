using System.Xml.Linq;
using Comdiv.ThemaLoader.Wrap;

namespace Comdiv.ThemaLoader {
	public class ThemaItemElement : IThemaItemElement {
		public ThemaItemElement(XElement e = null) {
			if (null != e) e.Apply(this);
			XmlSource = e;
			if (Type.noContent()) Type = e.Name.LocalName;
		}

		#region IThemaItemElement Members

		public string Type { get; set; }
		public string Role { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public XElement XmlSource { get; set; }

		public bool Authorized(string usr = null) {
			//NOTE: may be we must provide valud authorize method here
			return true;
		}

		public bool Immutable { get; set; }
		public string Group { get; set; }
		public string CustomCode { get; set; }
		public string Condition { get; set; }
		public string Comment { get; set; }
		public string Evidence { get; set; }
		public IThemaItem ContainingItem { get; set; }
		public bool IsFormula { get; set; }
		public string Formula { get; set; }
		public string FormulaType { get; set; }
		public string CssClass { get; set; }
		public string CssStyle { get; set; }
		public string CustomView { get; set; }
		public string Tag { get; set; }
		public string NumberFormat { get; set; }

		public virtual IThemaItemElementWrapper GetWrapper(IThemaItemWrapper itemwrapper) {
			return new ThemaItemElementWrapper(this, itemwrapper);
		}

		#endregion
	}
}