using System.Collections.Generic;
using System.Linq;

namespace Comdiv.ThemaLoader.Wrap {
	public class ThemaWrapper : IThemaWrapper {
		private IList<IThemaItemWrapper> _itemwrappers;

		public ThemaWrapper(IThema target, IThemaWrapperFactory factory, WrapContext context) {
			Thema = target;
			Factory = factory;
			Context = context;
		}

		#region IThemaWrapper Members

		public WrapContext Context { get; private set; }

		public IThemaWrapperFactory Factory { get; private set; }

		public IThema Thema { get; private set; }

		public IThemaItemWrapper GetItem(string code) {
			if (null == _itemwrappers) {
				fillItemWrappers();
			}
			return _itemwrappers.FirstOrDefault(x => x.Item.Code == code);
		}

		public IReportThemaItemWrapper GetReport(string code) {
			if (!code.EndsWith(".out")) code += ".out";
			return GetItem(code) as IReportThemaItemWrapper;
		}

		public IFormThemaItemWrapper GetForm(string code) {
			if (!code.EndsWith(".in")) code += ".in";
			return GetItem(code) as IFormThemaItemWrapper;
		}

		#endregion

		private void fillItemWrappers() {
			_itemwrappers = new List<IThemaItemWrapper>();
			foreach (var item in Thema.Items) {
				if (item.Authorized(Factory.Usr)) {
					var wrapper = ThemaItemWrapper.Wrap(item, this);
					_itemwrappers.Add(wrapper);
				}
			}
		}
	}
}