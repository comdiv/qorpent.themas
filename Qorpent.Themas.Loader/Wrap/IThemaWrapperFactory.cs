namespace Comdiv.ThemaLoader.Wrap {
	public interface IThemaWrapperFactory {
		IThemaFactory Factory { get; }
		string Usr { get; }
		IThemaWrapper WrapThema(string code, WrapContext context = null);
		IFormThemaItemWrapper WrapForm(string code, WrapContext context = null);
		IReportThemaItemWrapper WrapReport(string code, WrapContext context = null);
		T WrapItem<T>(string code, WrapContext context = null) where T : IThemaItemWrapper;
		bool IsInRole(string role, bool exact = false);
	}
}