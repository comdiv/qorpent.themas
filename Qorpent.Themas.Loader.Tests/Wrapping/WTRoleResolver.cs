using System.Security.Principal;
using Comdiv.QWeb.Security;

namespace Comdiv.ThemaLoader.Test.Wrapping {
	public class WTRoleResolver: IRoleResolver {
		public bool IsInRole(IPrincipal principal, string role, bool exact, QWebContext context) {
			if(principal.Identity.Name=="test\\admin") return true;
			if (role == "DEFAULT" || role=="STRICT") return true;
			return principal.IsInRole(role);

		}
	}
}