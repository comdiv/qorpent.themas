using System;

namespace Comdiv.ThemaLoader.ZetaIntegration {
	public interface IZetaEntityIntermediate {
		int Id { get; }
		string Code { get; }
		string Name { get; }
		string Tag { get; }
		string Comment { get; }
		DateTime Start { get; }
		DateTime Finish { get; }
		bool Active { get; }
		bool IsFormula { get; }
		string Formula { get; }
		string FormulaType { get; }
		string Marks { get; }
		string Valuta { get; }
	}
}