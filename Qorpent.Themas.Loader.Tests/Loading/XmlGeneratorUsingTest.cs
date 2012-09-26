using System.Linq;
using System.Xml.Linq;
using Comdiv.QWeb.Utils;
using NUnit.Framework;

namespace Comdiv.ThemaLoader.Test.Loading {
	[TestFixture]
	public class XmlGeneratorUsingTest {
		public class TestGenerator:ILoadXmlGenerator {
			public XElement Generate(XElement sourceElement, IThemaLoader loader) {
				return new XElement("hello",new XAttribute("code",sourceElement.describe().Name));
			}
		}
		private string code = @"
generator test, 'Comdiv.ThemaLoader.Test.Loading.XmlGeneratorUsingTest+TestGenerator,Comdiv.ThemaLoader.Test', xmlload
thema test
	out testreport.out
		call test 1
		call test 2
";
		[Test]
		public void generators_applyed() {
			var result = new ThemaFactory().Load(new BxlThemaSource(code));
			var report = result.Themas.GetReport("test.testreport.out");
			Assert.NotNull(report.GetElements("hello").FirstOrDefault(x=>x.Code=="1"));
			Assert.NotNull(report.GetElements("hello").FirstOrDefault(x => x.Code == "2"));
			Assert.NotNull(report.GetElements("hello").FirstOrDefault(x => x.XmlSource.attr("code") == "1"));
			Assert.NotNull(report.GetElements("hello").FirstOrDefault(x => x.XmlSource.attr("code") == "2"));
		}
	}
}