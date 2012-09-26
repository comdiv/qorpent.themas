namespace Comdiv.ThemaLoader.ExtensionPoints {
	public interface IThemaLoaderExtension {
		int Idx { get; set; }
		void Process(IThemaFactory factory);
	}
}