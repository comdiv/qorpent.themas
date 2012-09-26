using System.Xml.Linq;

namespace Comdiv.ThemaLoader {
	public interface ILoadXmlGenerator {
		XElement Generate(XElement sourceElement, IThemaLoader loader);
	}
}