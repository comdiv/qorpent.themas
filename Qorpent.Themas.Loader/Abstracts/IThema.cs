using System.Collections.Generic;
using System.Xml.Linq;

namespace Comdiv.ThemaLoader {
	public interface IThema : IElementBasis {
		int Id { get; set; }
		int Idx { get; set; }
		int FileIdx { get; set; }
		string Cluster { get; set; }
		string EcoProcess { get; set; }
		XElement XmlSource { get; set; }
		IThemaFactory Factory { get; set; }
		IList<IThemaLink> OutLinks { get; }
		IList<IThemaLink> InLinks { get; }
		IList<IThemaItem> Items { get; }
		IDictionary<string, string> Parameters { get; }
		IThema Group { get; }
		IEnumerable<IThema> GroupMembers { get; }
		IThema Parent { get; }
		IEnumerable<IThema> Children { get; }
		bool IsGroup { get; set; }
		string EffectiveIndex { get; }
		string ForGroup { get; set; }
		bool UseHistory { get; set; }
		int FirstYear { get; set; }
		int LastYear { get; set; }
		string[] ListForGroup { get; }
		string FixedObject { get; set; }
		IThema GetTargetThema(string code);
		IEnumerable<IThema> GetSourceThemas(string code);
		void SetupFromSourceXml();
		T GetParam<T>(string name, T def);
		IThemaItem GetItem(string code);
		IFormThemaItem GetForm(string code);
		IReportThemaItem GetReport(string code);
	}
}