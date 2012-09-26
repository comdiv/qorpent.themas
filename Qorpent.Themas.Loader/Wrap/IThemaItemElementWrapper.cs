using Comdiv.ThemaLoader.ZetaIntegration;

namespace Comdiv.ThemaLoader.Wrap {
	public interface IThemaItemElementWrapper : IThemaItemElement {
		IThemaItemWrapper ItemWrap { get; set; }
		IThemaItemElement Target { get; set; }
		IThemaWrapperFactory Factory { get; }
		IZetaEntityIntermediate ZetaObject { get; }
		bool IsValid();
	}
}