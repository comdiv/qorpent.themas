using System.Xml.Linq;
using Comdiv.ThemaLoader.Wrap;

namespace Comdiv.ThemaLoader {
	public class ObjItemElement : ThemaItemElement, IObjItemElement {
		public ObjItemElement(XElement e = null) : base(e) {
			Type = "obj";
		}

		#region IObjItemElement Members

		public override IThemaItemElementWrapper GetWrapper(IThemaItemWrapper itemwrapper) {
			return new ObjItemElementWrapper(this, itemwrapper);
		}

		#endregion
	}
}