using System.Collections.Generic;

namespace Comdiv.ThemaLoader {
	public interface IThemaCollection {
		ThemaFactory Factory { get; set; }
		IDictionary<string, IThema> Index { get; }
		IThema this[string code] { get; }
		IEnumerable<IThema> GetGroups(string usr = null);
		IEnumerable<IThemaItem> SearchItems(string pattern, string usr = null);
		IThemaItem GetItem(string code, string usr = null);
		IFormThemaItem GetForm(string code, string usr = null);
		IReportThemaItem GetReport(string code, string usr = null);
		void Clear();
	}
}