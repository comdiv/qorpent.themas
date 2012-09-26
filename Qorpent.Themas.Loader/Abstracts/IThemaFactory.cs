using System.Collections.Generic;
using System.Xml.Linq;
using Comdiv.QWeb.Factory;
using Comdiv.QWeb.Files;
using Comdiv.QWeb.Logging;
using Comdiv.QWeb.Security;
using Comdiv.ThemaLoader.Wrap;

namespace Comdiv.ThemaLoader {
	public interface IThemaFactory {
		IFileNameResolver FileResolver { get; set; }
		IThemaCollection Themas { get; set; }
		IRoleResolver RoleResolver { get; set; }
		IThemaLoader Loader { get; set; }
		ITypeLocator TypeLocator { get; set; }
		IList<IThemaSource> Sources { get; set; }
		ILogListener Log { get; set; }
		XElement ExtraData { get; set; }
		IEntityResolver EntityResolver { get; set; }
		IPeriodProvider PeriodProvider { get; set; }
		void CleanupSecurityCache();
		bool Authorize(string usr, IThema thema);
		bool Authorize(string usr, IThemaItem themaitem);

		void ReLoad();
		IThemaFactory Load(params IThemaSource[] sources);
		IThema GetThema(string code, string usr);
		IThemaWrapperFactory GetWrapper(string usr);
		IUserThemaTreeBuilder GetUIBuilder();
	}
}