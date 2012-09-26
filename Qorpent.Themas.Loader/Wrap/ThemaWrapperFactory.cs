using System.Security.Principal;

namespace Comdiv.ThemaLoader.Wrap {
	public class ThemaWrapperFactory : IThemaWrapperFactory {
		private readonly IThemaFactory factory;
		private readonly string usr;
		private IPrincipal __usr;

		public ThemaWrapperFactory(IThemaFactory factory, string usr) {
			this.factory = factory;
			this.usr = usr;
		}

		protected IPrincipal _usr {
			get {
				if (null == __usr) {
					__usr = new GenericPrincipal(new GenericIdentity(Usr), null);
				}
				return __usr;
			}
		}

		#region IThemaWrapperFactory Members

		public IThemaFactory Factory {
			get { return factory; }
		}

		public string Usr {
			get { return usr; }
		}

		public bool IsInRole(string role, bool exact = false) {
			return Factory.RoleResolver.IsInRole(_usr, role, exact, null);
		}

		public IThemaWrapper WrapThema(string code, WrapContext context = null) {
			context = context ?? new WrapContext();
			var thema = Factory.GetThema(code, Usr);
			return new ThemaWrapper(thema, this, context);
		}

		public IFormThemaItemWrapper WrapForm(string code, WrapContext context) {
			if (!code.EndsWith(".in")) code += ".in";
			return WrapItem<IFormThemaItemWrapper>(code, context);
		}

		public IReportThemaItemWrapper WrapReport(string code, WrapContext context) {
			if (!code.EndsWith(".out")) code += ".out";
			return WrapItem<IReportThemaItemWrapper>(code, context);
		}

		public T WrapItem<T>(string code, WrapContext context = null) where T : IThemaItemWrapper {
			context = context ?? new WrapContext();
			var item = Factory.Themas.GetItem(code);
			var tw = new ThemaWrapper(item.Thema, this, context);
			return (T) tw.GetItem(item.Code);
		}

		#endregion
	}
}