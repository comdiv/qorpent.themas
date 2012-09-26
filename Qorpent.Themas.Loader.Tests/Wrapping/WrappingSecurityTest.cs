using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using Comdiv.ThemaLoader.Wrap;
using NUnit.Framework;

namespace Comdiv.ThemaLoader.Test.Wrapping
{
	public class WTBaseTest {
		protected string code;

		protected IThemaWrapperFactory load(string  usr) {
			return new ThemaFactory(roleresolver:new WTRoleResolver(), entityResolver:new WTEntityResolver())
				.Load(new BxlThemaSource(code)).GetWrapper(usr);
		}
	}

	[TestFixture]
	public class WrappingSecurityTest : WTBaseTest {
		public WrappingSecurityTest() {
			this.code = @"
thema A A isgroup
thema Aa Aa role=ADMIN
	group A
thema Aa1 Aa1 role=ADMIN
	parent Aa
thema Aa2 Aa2 role=STRICT
	parent Aa
	in X.in, role=ADMIN
	in Y.in, role=STRICT 

thema As As role=STRICT
	group A
thema As1 As1 role=ADMIN
	parent As
thema As2 As2 role=STRICT
	parent As
";
		}

		[Test]
		public void available_thema_will_be_wrapped() {
			var admin_result = load("test\\admin");
			var strict_result = load("test\\strict");
			Assert.NotNull(admin_result.WrapThema("Aa1"));
			Assert.NotNull(admin_result.WrapThema("Aa2"));
			Assert.Throws<SecurityException>(()=>strict_result.WrapThema("Aa1"));
			Assert.NotNull(admin_result.WrapThema("Aa2"));
			Assert.NotNull(admin_result.WrapItem<IFormThemaItemWrapper>("Aa2.X.in"));
			Assert.NotNull(admin_result.WrapItem<IFormThemaItemWrapper>("Aa2.Y.in"));
			Assert.Null( strict_result.WrapItem<IFormThemaItemWrapper>("Aa2.X.in"));
			Assert.NotNull(strict_result.WrapItem<IFormThemaItemWrapper>("Aa2.Y.in"));
		}
	}
}
