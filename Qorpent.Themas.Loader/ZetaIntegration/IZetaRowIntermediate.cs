namespace Comdiv.ThemaLoader.ZetaIntegration {
	public interface IZetaRowIntermediate : IZetaEntityIntermediate {
		string Path { get; }
		string ParentCode { get; }
	}
}