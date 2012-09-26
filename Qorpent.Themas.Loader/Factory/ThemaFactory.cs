using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Xml.Linq;
using Comdiv.QWeb.Factory;
using Comdiv.QWeb.Files;
using Comdiv.QWeb.Logging;
using Comdiv.QWeb.Security;
using Comdiv.ThemaLoader.ExtensionPoints;
using Comdiv.ThemaLoader.UI;
using Comdiv.ThemaLoader.Wrap;

namespace Comdiv.ThemaLoader {
	public class ThemaFactory : IThemaFactory {
		private readonly IDictionary<string, bool> _security_authorize_cache = new Dictionary<string, bool>();
		private readonly object refresh_lock = new object();

		public ThemaFactory(
			IThemaLoader loader = null,
			IRoleResolver roleresolver = null,
			IFileNameResolver fileresolver = null,
			ITypeLocator locator = null,
			IEntityResolver entityResolver = null,
			ILogListener log = null,
			IPeriodProvider periodprovider = null) {
			Themas = new ThemaCollection {Factory = this};
			RoleResolver = roleresolver ?? QWebServiceRegistry.Default.RoleResolver;
			FileResolver = fileresolver ?? QWebServiceRegistry.Default.FileNameResolver;
			TypeLocator = locator ?? QWebServiceRegistry.Default.TypeLocator;
			PeriodProvider = periodprovider ?? TypeLocator.Get<IPeriodProvider>().create<IPeriodProvider>();
			EntityResolver = entityResolver ?? TypeLocator.Get<IEntityResolver>().create<IEntityResolver>();
			Sources = new List<IThemaSource>();
			Log = log ?? QWebServiceRegistry.Default.Log;
			Loader = loader ?? new Loader(this);
		}

		#region IThemaFactory Members

		public IPeriodProvider PeriodProvider { get; set; }

		public IThemaFactory Load(params IThemaSource[] sources) {
			return Loader.Load(sources).Factory;
		}

		public IUserThemaTreeBuilder GetUIBuilder() {
			return new UserThemaTreeBuilder {Factory = this};
		}

		public IThema GetThema(string code, string usr) {
			var result = Themas[code];
			if (null == result) return null;
			if (!result.Authorized(usr)) {
				throw new SecurityException("User " + usr + " attempt to access not owned " + code + " thema");
			}
			return result;
		}

		public XElement ExtraData { get; set; }

		public IList<IThemaSource> Sources { get; set; }
		public ITypeLocator TypeLocator { get; set; }

		public IThemaWrapperFactory GetWrapper(string usr) {
			return new ThemaWrapperFactory(this, usr);
		}

		public IThemaCollection Themas { get; set; }
		public IRoleResolver RoleResolver { get; set; }
		public IFileNameResolver FileResolver { get; set; }
		public IThemaLoader Loader { get; set; }

		public void ReLoad() {
			lock (refresh_lock) {
				CleanupSecurityCache();
				Themas.Clear();
				Loader.Load();
				foreach (var loaderextension in TypeLocator
					.GetAll<IThemaLoaderExtension>()
					.Select(x => x.create<IThemaLoaderExtension>())
					.OrderBy(x => x.Idx)) {
					loaderextension.Process(this);
				}
			}
		}


		public void CleanupSecurityCache() {
			lock (refresh_lock) {
				_security_authorize_cache.Clear();
			}
		}

		public IEntityResolver EntityResolver { get; set; }
		public ILogListener Log { get; set; }


		public bool Authorize(string usr, IThema thema) {
			lock (refresh_lock) {
			}
			return Authorize(usr, thema, usr + "_" + thema);
		}

		public bool Authorize(string usr, IThemaItem themaitem) {
			lock (refresh_lock) {
			}
			return Authorize(usr, themaitem, usr + "_" + themaitem.Thema.Code + "." + themaitem.Code);
		}

		#endregion

		public IUserThemaTreeBuilder GetTreeBuilder() {
			return new UserThemaTreeBuilder {Factory = this};
		}

		private bool Authorize(string usr, IElementBasis element, string key) {
			lock (this) {
				if (element.Role.noContent()) return true;
				if (usr.noContent()) return true;
				if (_security_authorize_cache.ContainsKey(key)) return _security_authorize_cache[key];
				var roles = element.Role.SmartSplit();
				var result = false;
				foreach (var role in roles) {
					if (RoleResolver.IsInRole(new GenericPrincipal(new GenericIdentity(usr), new string[] {}), role, false, null)) {
						result = true;
						break;
					}
				}
				_security_authorize_cache[key] = result;
				return result;
			}
		}
	}
}