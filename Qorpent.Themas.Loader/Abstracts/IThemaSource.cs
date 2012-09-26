using System.Collections.Generic;
using System.Xml.Linq;

namespace Comdiv.ThemaLoader {
	public interface IThemaSource {
		IEnumerable<XElement> GetXmlSources(IThemaFactory factory);
	}
}