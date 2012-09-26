namespace Comdiv.ThemaLoader {
	public interface IColumnItemElement : IThemaItemElement {
		string ForPeriods { get; set; }
		string ForGroup { get; set; }
		string Valuta { get; set; }
		string MatrixId { get; set; }
		string MatrixFormula { get; set; }
		string MatrixFormulaType { get; set; }
		string MatrixTotalFormula { get; set; }
		bool Fixed { get; set; }
		int Year { get; set; }
		int Period { get; set; }
		bool DoSum { get; set; }
	}
}