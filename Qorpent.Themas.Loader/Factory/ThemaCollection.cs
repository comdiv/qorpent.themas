using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Comdiv.ThemaLoader {
	public class ThemaCollection : IThemaCollection {
		public ThemaCollection() {
			Index = new Dictionary<string, IThema>();
		}

		#region IThemaCollection Members

		public ThemaFactory Factory { get; set; }

		public IDictionary<string, IThema> Index { get; private set; }

		public IEnumerable<IThema> GetGroups(string usr = null) {
			return Index.Values.Where(x => x.Authorized(usr)).Where(x => x.IsGroup).OrderBy(x => x.EffectiveIndex);
		}

		public IEnumerable<IThemaItem> SearchItems(string pattern, string usr = null) {
			var keys = pattern.Split('.');
			var thema_pattern = keys[0];
			if (thema_pattern == "*") thema_pattern = "^.+$";
			if (keys[1] == "*") keys[1] = "^.+";
			if (keys[2] == "*") keys[2] = ".+$";
			var item_pattern = keys[1] + "\\." + keys[2];
			var themas =
				Index.Values.Where(x => x.Authorized(usr)).Where(
					x => Regex.IsMatch(x.Code, thema_pattern, RegexOptions.Compiled)).ToArray();
			foreach (var thema in themas) {
				foreach (var item in thema.Items.Where(x => usr.noContent() || Factory.Authorize(usr, x))) {
					if (Regex.IsMatch(item.Code, item_pattern, RegexOptions.Compiled)) {
						yield return item;
					}
				}
			}
		}

		public IThema this[string code] {
			get {
				if (Index.ContainsKey(code)) return Index[code];
				return null;
			}
		}

		public IThemaItem GetItem(string code, string usr = null) {
			return SearchItems(code, usr).FirstOrDefault();
		}

		public IFormThemaItem GetForm(string code, string usr = null) {
			if (!code.EndsWith(".in")) code += ".in";
			return GetItem(code, usr) as IFormThemaItem;
		}

		public IReportThemaItem GetReport(string code, string usr = null) {
			if (!code.EndsWith(".out")) code += ".out";
			return GetItem(code, usr) as IReportThemaItem;
		}

		public void Clear() {
			Index.Clear();
		}

		#endregion
	}
}