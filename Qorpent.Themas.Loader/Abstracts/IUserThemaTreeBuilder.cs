using Comdiv.ThemaLoader.UI;
using Comdiv.ThemaLoader.Wrap;

namespace Comdiv.ThemaLoader {
	public interface IUserThemaTreeBuilder {
		ThemaFactory Factory { get; set; }
		UserThemaTree BuildTree(string usr = null, WrapContext context = null);
	}
}