using System.Xml.Linq;

namespace Comdiv.ThemaLoader {
	public interface IThemaLink {
		string Type { get; set; }
		string SourceCode { get; set; }
		string TargetCode { get; set; }
		IThema Source { get; set; }
		IThema Target { get; set; }
		XElement XmlSource { get; set; }
	}
}