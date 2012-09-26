using Comdiv.ThemaLoader.Wrap;

namespace Comdiv.ThemaLoader.UI {
	public class utThemaRoot {
		private IThemaWrapper _wrap;
		public string Idx { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public IThema Target { get; set; }
		public utTerminal[] Terminals { get; set; }
		public string Usr { get; set; }

		public utThemaGroup Group { get; set; }

		public IThemaWrapper GetWrap(WrapContext context = null) {
			return _wrap ??
			       (_wrap = new ThemaWrapperFactory(Target.Factory, Usr).WrapThema(Target.Code, context ?? Group.Tree.Context));
		}
	}
}