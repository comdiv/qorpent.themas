using System.Collections.Generic;
using System.Xml.Linq;
using Comdiv.QWeb.Serialization.BxlParser;

namespace Comdiv.ThemaLoader {
	public class BxlThemaSource : IThemaSource {
		private readonly string bxl;

		public BxlThemaSource(string bxl) {
			this.bxl = bxl;
		}

		#region IThemaSource Members

		public IEnumerable<XElement> GetXmlSources(IThemaFactory factory) {
			yield return new BxlXmlParser().Parse(bxl, "direct");
		}

		#endregion
	}
}