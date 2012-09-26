namespace Comdiv.ThemaLoader {
	internal class FormLockDepend : IFormLockDepend {
		#region IFormLockDepend Members

		public string SourceCode { get; set; }
		public string TargetCode { get; set; }
		public IFormThemaItem Source { get; set; }
		public IFormThemaItem Target { get; set; }

		#endregion
	}
}