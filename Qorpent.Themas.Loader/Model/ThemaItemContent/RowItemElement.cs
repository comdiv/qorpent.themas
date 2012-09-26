using System.Xml.Linq;
using Comdiv.ThemaLoader.Wrap;

namespace Comdiv.ThemaLoader {
	public class RowItemElement : ThemaItemElement, IRowItemElement {
		public RowItemElement(XElement e = null) : base(e) {
			Type = "row";
		}

		#region IRowItemElement Members

		public override IThemaItemElementWrapper GetWrapper(IThemaItemWrapper itemwrapper) {
			return new RowItemElementWrapper(this, itemwrapper);
		}

		#endregion
	}
}