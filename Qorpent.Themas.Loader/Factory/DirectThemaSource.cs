using System.Collections.Generic;
using System.Xml.Linq;

namespace Comdiv.ThemaLoader {
	public class DirectThemaSource : IThemaSource {
		private readonly XElement[] _xmlSources;

		public DirectThemaSource(params XElement[] xmls) {
			_xmlSources = xmls ?? new XElement[] {};
		}

		#region IThemaSource Members

		public IEnumerable<XElement> GetXmlSources(IThemaFactory factory) {
			return _xmlSources;
		}

		#endregion
	}
}