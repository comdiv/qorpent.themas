using System.Xml.Linq;

namespace Comdiv.ThemaLoader {
	public class ThemaLink : IThemaLink {
		#region IThemaLink Members

		public string Type { get; set; }
		public string SourceCode { get; set; }
		public string TargetCode { get; set; }
		public IThema Source { get; set; }
		public IThema Target { get; set; }

		public XElement XmlSource { get; set; }

		#endregion
	}
}