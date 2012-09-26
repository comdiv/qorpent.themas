using System;
using Comdiv.QWeb.Utils;
using Comdiv.ThemaLoader.ZetaIntegration;

namespace Comdiv.ThemaLoader.Test.Wrapping {
	public class WTEntityResolver: IEntityResolver {
		public T Get<T>(object key) {
			return (T)Get(typeof (T), key) ;
		}

		public object Get(Type type, object key) {
			if (typeof(WTObject) == type) {



				if (1.Equals(key.toInt())) {
					return new WTObject() {Id = 1, GroupCache = "/G1/G2/"};
				}
				else if (2.Equals(key.toInt())) {
					return new WTObject() {Id = 2, GroupCache = "/G1/KVART/"};
				}
				return new WTObject() {Id = key.toInt(), GroupCache = "/G1/"};
			}
			else if (typeof(IZetaColumnIntermediate) == type) {
				if ("ZB1".Equals(key)) {
					return new WTColumn {Code = "ZB1", Name = "Z B 1"};
				}
				else if ("ZF1".Equals(key)) {
					return new WTColumn {Code = "ZF1", Name = "Z F 1", IsFormula = true, Formula = "@ZB1?", FormulaType = "boo"};
				}
				else {
					return null;
				}
			}
			return null;
		}
	}
}