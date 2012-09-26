namespace Comdiv.ThemaLoader.Wrap {
	public interface IThemaWrapper {
		WrapContext Context { get; }
		IThemaWrapperFactory Factory { get; }
		IThema Thema { get; }
		IThemaItemWrapper GetItem(string code);
		IReportThemaItemWrapper GetReport(string code);
		IFormThemaItemWrapper GetForm(string code);
	}
}