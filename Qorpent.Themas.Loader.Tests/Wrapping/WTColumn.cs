using System;
using Comdiv.ThemaLoader.ZetaIntegration;

namespace Comdiv.ThemaLoader.Test.Wrapping {
	public class WTColumn : IZetaColumnIntermediate {
		public int Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string Tag { get; set; }
		public string Comment { get; set; }
		public DateTime Start { get; set; }
		public DateTime Finish { get; set; }
		public bool Active { get; set; }
		public bool IsFormula { get; set; }
		public string Formula { get; set; }
		public string FormulaType { get; set; }
		public string Marks { get; set; }
		public string Valuta { get; set; }
		public string DataType { get; set; }
	}
}