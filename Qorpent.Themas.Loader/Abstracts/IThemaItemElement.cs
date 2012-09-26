using Comdiv.ThemaLoader.Wrap;

namespace Comdiv.ThemaLoader {
	public interface IThemaItemElement : IElementBasis {
		string Group { get; set; }
		string CustomCode { get; set; }
		string Condition { get; set; }
		string Comment { get; set; }
		string Evidence { get; set; }
		IThemaItem ContainingItem { get; set; }
		string Type { get; set; }
		bool IsFormula { get; set; }
		string Formula { get; set; }
		string FormulaType { get; set; }
		string CssClass { get; set; }
		string CssStyle { get; set; }
		string CustomView { get; set; }
		string Tag { get; set; }
		bool Immutable { get; set; }
		string NumberFormat { get; set; }
		IThemaItemElementWrapper GetWrapper(IThemaItemWrapper itemwrapper);
	}
}