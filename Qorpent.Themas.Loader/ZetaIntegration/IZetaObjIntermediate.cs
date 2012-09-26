namespace Comdiv.ThemaLoader.ZetaIntegration {
	public interface IZetaObjIntermediate : IZetaEntityIntermediate {
		string Path { get; }
		string ParentCode { get; }
		string RoleCode { get; }
		string GroupCode { get; }
		string LocationCode { get; }
		string TypeCode { get; }
	}
}